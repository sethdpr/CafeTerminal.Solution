using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CafeTerminal.Api.Data
{
    public class CafeTerminalDbContext : IdentityDbContext<ApplicationUser>
    {
        public CafeTerminalDbContext(DbContextOptions<CafeTerminalDbContext> options)
            : base(options)
        {
        }

        public DbSet<Table> Tables { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Use Number as the key for Table
            builder.Entity<Table>().HasKey(t => t.Number);
            // Product primary key
            builder.Entity<Product>().HasKey(p => p.Id);
            builder.Entity<Order>().HasKey(o => o.Id);
            builder.Entity<OrderItem>().HasKey(oi => oi.Id);

            builder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade);
        }
    }
}