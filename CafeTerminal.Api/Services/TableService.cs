using CafeTerminal.Api.Data;
using CafeTerminal.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeTerminal.Api.Services
{
    public class TableService
    {
        private readonly CafeTerminalDbContext _context;

        public TableService(CafeTerminalDbContext context)
        {
            _context = context;
        }

        public async Task<List<Table>> GetAllAsync()
        {
            return await _context.Tables.ToListAsync();
        }

        public async Task<Table?> GetByIdAsync(int id)
        {
            return await _context.Tables.FindAsync(id);
        }

        public async Task<Table> CreateAsync(Table table)
        {
            _context.Tables.Add(table);
            await _context.SaveChangesAsync();
            return table;
        }

        public async Task<Table?> UpdateAsync(int id, Table updated)
        {
            var table = await _context.Tables.FindAsync(id);
            if (table == null) return null;

            table.Name = updated.Name;
            table.Number = updated.Number;

            await _context.SaveChangesAsync();
            return table;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var table = await _context.Tables.FindAsync(id);
            if (table == null) return false;

            _context.Tables.Remove(table);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}