using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CafeTerminal.Api.Data
{
    // This Entity Framework Core context contains both Identity tables
    // and the custom application tables used by the app.
    public class CafeTerminalDbContext : IdentityDbContext<ApplicationUser>
    {
        public CafeTerminalDbContext(DbContextOptions<CafeTerminalDbContext> options)
            : base(options)
        {
        }

        // Tables shown in the tables overview.
        public DbSet<Table> Tables { get; set; }
        // Products that can be ordered.
        public DbSet<Product> Products { get; set; }
        // Orders created for tables.
        public DbSet<Order> Orders { get; set; }
        // Individual product rows inside an order.
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Use Number as the key for Table.
            builder.Entity<Table>().HasKey(t => t.Number);
            // Standard integer primary keys for the other entities.
            builder.Entity<Product>().HasKey(p => p.Id);
            builder.Entity<Order>().HasKey(o => o.Id);
            builder.Entity<OrderItem>().HasKey(oi => oi.Id);

            // One order contains many order items.
            builder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade);
        }
    }
}