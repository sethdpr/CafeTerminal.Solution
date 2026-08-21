using CafeTerminal.Api.Data;
using CafeTerminal.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CafeTerminal.Api.Services;

public class OrderService : IOrderService
{
    private readonly CafeTerminalDbContext _db;

    public OrderService(CafeTerminalDbContext db)
    {
        _db = db;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var any = await _db.Orders.AnyAsync();
        }
        catch (Microsoft.Data.SqlClient.SqlException ex)
        {
            if (ex.Message.Contains("Invalid object name", StringComparison.OrdinalIgnoreCase))
            {
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

    public async Task<OrderDto> CreateAsync(CreateOrderRequest request)
    {
        var order = new Order
        {
            TableNumber = request.TableNumber,
            CreatedAt = DateTime.UtcNow
        };

        decimal total = 0m;
        foreach (var item in request.Items)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId && p.DeletedAt == null);
            if (product == null) continue;

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

        order.TotalPrice = total;
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

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

    public async Task<List<OrderDto>> GetOrdersForTableAsync(int tableNumber)
    {
        var orders = await _db.Orders
            .Where(o => o.TableNumber == tableNumber)
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

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
}
