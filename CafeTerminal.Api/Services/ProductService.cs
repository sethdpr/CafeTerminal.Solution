using CafeTerminal.Api.Data;
using CafeTerminal.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeTerminal.Api.Services
{
    public class ProductService
    {
        private readonly CafeTerminalDbContext _context;

        public ProductService(CafeTerminalDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> UpdateAsync(Product updated)
        {
            var existing = await _context.Products.FindAsync(updated.Id);
            if (existing == null) return null;

            existing.Name = updated.Name;
            existing.Price = updated.Price;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Products.FindAsync(id);
            if (existing == null) return false;

            _context.Products.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
