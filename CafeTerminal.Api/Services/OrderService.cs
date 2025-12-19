using CafeTerminal.Api.Data;
using CafeTerminal.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeTerminal.Api.Services
{
    public class OrderService
    {
        private readonly CafeTerminalDbContext _context;

        public OrderService(CafeTerminalDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Product)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Order>> GetByTableIdAsync(int tableId)
        {
            return await _context.Orders
                .Where(o => o.TableId == tableId)
                .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Product)
                .ToListAsync();
        }

        public async Task<Order> CreateAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> UpdateAsync(int id, Order updated)
        {
            var order = await _context.Orders
                .Include(o => o.OrderLines)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return null;

            order.TableId = updated.TableId;

            _context.OrderLines.RemoveRange(order.OrderLines);
            order.OrderLines = updated.OrderLines;

            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}