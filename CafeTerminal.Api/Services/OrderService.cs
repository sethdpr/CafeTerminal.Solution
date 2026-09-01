using CafeTerminal.Api.Data;
using CafeTerminal.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CafeTerminal.Api.Services;

// This service contains the business logic for orders and payments.
public class OrderService : IOrderService
{
    private readonly CafeTerminalDbContext _db;

    public OrderService(CafeTerminalDbContext db)
    {
        _db = db;
    }

    // Creates the order tables if they do not exist yet.
    public async Task InitializeAsync()
    {
        try
        {
            // Probe the Orders table so missing-table errors can be handled explicitly.
            var any = await _db.Orders.AnyAsync();
        }
        catch (Microsoft.Data.SqlClient.SqlException ex)
        {
            if (ex.Message.Contains("Invalid object name", StringComparison.OrdinalIgnoreCase))
            {
                // Create the Orders and OrderItems tables for legacy databases.
                var createSql = @"IF OBJECT_ID(N'dbo.Orders', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Orders](
        [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [TableNumber] int NOT NULL,
        [TotalPrice] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [PaymentDate] datetime2 NULL
    );
END
IF OBJECT_ID(N'dbo.OrderItems', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[OrderItems](
        [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [OrderId] int NOT NULL,
        [ProductId] int NOT NULL,
        [Quantity] int NOT NULL,
        [UnitPrice] decimal(18,2) NOT NULL
    );
END";

                await _db.Database.ExecuteSqlRawAsync(createSql);
            }
            else
            {
                throw;
            }
        }
    }

    // Creates a new order from the selected products and quantities.
    public async Task<OrderDto> CreateAsync(CreateOrderRequest request)
    {
        // Start a new order for the requested table.
        var order = new Order
        {
            TableNumber = request.TableNumber,
            CreatedAt = DateTime.UtcNow
        };

        // Build order lines from the selected products and calculate the total.
        decimal total = 0m;
        foreach (var item in request.Items)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId && p.DeletedAt == null);
            if (product == null) continue;

            // Copy the current product price into the order item.
            var unitPrice = product.Price;
            var orderItem = new OrderItem
            {
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = unitPrice
            };

            order.Items.Add(orderItem);
            total += unitPrice * item.Quantity;
        }

        // Save the order and its items to the database.
        order.TotalPrice = total;
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        // Map the persisted order back to the DTO returned by the API.
        return new OrderDto
        {
            Id = order.Id,
            TableNumber = order.TableNumber,
            CreatedAt = order.CreatedAt,
            TotalPrice = order.TotalPrice,
            Items = order.Items.Select(oi => new OrderItemDto
            {
                ProductId = oi.ProductId,
                ProductName = _db.Products.First(p => p.Id == oi.ProductId).Name,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList()
        };
    }

    // Returns the active unpaid orders for one table.
    public async Task<List<OrderDto>> GetOrdersForTableAsync(int tableNumber)
    {
        // Load all unpaid orders for the requested table, newest first.
        var orders = await _db.Orders
            .Where(o => o.TableNumber == tableNumber && o.PaymentDate == null)
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        // Convert the entity graph into DTOs for the API response.
        return orders.Select(o => new OrderDto
        {
            Id = o.Id,
            TableNumber = o.TableNumber,
            CreatedAt = o.CreatedAt,
            PaymentDate = o.PaymentDate,
            TotalPrice = o.TotalPrice,
            Items = o.Items.Select(oi => new OrderItemDto
            {
                ProductId = oi.ProductId,
                ProductName = _db.Products.First(p => p.Id == oi.ProductId).Name,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList()
        }).ToList();
    }

    // Builds a payment overview for one table.
    public async Task<PaymentSummaryDto> GetPaymentSummaryAsync(int tableNumber)
    {
        // Load the current table information for the payment header.
        var table = await _db.Tables.FirstOrDefaultAsync(t => t.Number == tableNumber);

        // Reuse the order query so the summary contains the latest unpaid orders.
        var orders = await GetOrdersForTableAsync(tableNumber);

        // Combine the order list and the grand total into one response.
        return new PaymentSummaryDto
        {
            TableNumber = tableNumber,
            TableName = table?.Name ?? string.Empty,
            Orders = orders,
            TotalPrice = orders.Sum(o => o.TotalPrice)
        };
    }

    // Completes the payment by timestamping all unpaid orders and freeing the table.
    public async Task<bool> CompletePaymentAsync(int tableNumber)
    {
        // Load all unpaid orders that belong to the selected table.
        var orders = await _db.Orders
            .Where(o => o.TableNumber == tableNumber && o.PaymentDate == null)
            .ToListAsync();

        if (orders.Count == 0)
        {
            // When there are no open orders, still clear the table name if the table exists.
            var existingTable = await _db.Tables.FirstOrDefaultAsync(t => t.Number == tableNumber);
            if (existingTable == null)
            {
                return false;
            }

            existingTable.Name = string.Empty;
            await _db.SaveChangesAsync();
            return true;
        }

        // Stamp one shared payment timestamp on every unpaid order.
        var paymentDate = DateTime.UtcNow;
        foreach (var order in orders)
        {
            order.PaymentDate = paymentDate;
        }

        // Clear the table name so it becomes available again.
        var table = await _db.Tables.FirstOrDefaultAsync(t => t.Number == tableNumber);
        if (table == null)
        {
            return false;
        }

        table.Name = string.Empty;
        await _db.SaveChangesAsync();
        return true;
    }
}
