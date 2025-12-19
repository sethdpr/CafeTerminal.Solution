using CafeTerminal.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeTerminal.Api.Data
{
    public class CafeTerminalDbContext : DbContext
    {
        public CafeTerminalDbContext(DbContextOptions<CafeTerminalDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Table> Tables => Set<Table>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderLine> OrderLines => Set<OrderLine>();
    }
}