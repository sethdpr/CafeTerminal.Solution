using CafeTerminal.Shared.DTOs;
using CafeTerminal.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace CafeTerminal.Api.Services;

// This service contains the business logic for products.
public class ProductService : IProductService
{
    private readonly CafeTerminalDbContext _db;

    public ProductService(CafeTerminalDbContext db)
    {
        _db = db;
    }

    // Creates the Products table and missing columns for older databases.
    public async Task InitializeAsync()
    {
        try
        {
            // Test access
            var any = await _db.Products.AnyAsync();
        }
        catch (Microsoft.Data.SqlClient.SqlException ex)
        {
            if (ex.Message.Contains("Invalid object name", StringComparison.OrdinalIgnoreCase))
            {
                var createSql = @"IF OBJECT_ID(N'dbo.Products', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Products](
        [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name] nvarchar(max) NULL,
        [Price] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [DeletedAt] datetime2 NULL
    );
END";

                await _db.Database.ExecuteSqlRawAsync(createSql);
            }
            else
            {
                throw;
            }
        }

        // Ensure CreatedAt and DeletedAt columns exist (for databases created before these columns were added)
        var alterSql = @"
IF OBJECT_ID(N'dbo.Products', N'U') IS NOT NULL
BEGIN
    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Products') AND name = 'CreatedAt')
    BEGIN
        ALTER TABLE dbo.Products ADD CreatedAt datetime2 NOT NULL CONSTRAINT DF_Products_CreatedAt DEFAULT (GETUTCDATE());
    END
    IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.Products') AND name = 'DeletedAt')
    BEGIN
        ALTER TABLE dbo.Products ADD DeletedAt datetime2 NULL;
    END
END
";

        await _db.Database.ExecuteSqlRawAsync(alterSql);
    }

    // Returns all active products that were not soft deleted.
    public async Task<List<ProductDto>> GetAllAsync()
    {
        return await _db.Products
            .Where(p => p.DeletedAt == null)
            .OrderBy(p => p.Id)
            .Select(p => new ProductDto { Id = p.Id, Name = p.Name, Price = p.Price, CreatedAt = p.CreatedAt, DeletedAt = p.DeletedAt })
            .ToListAsync();
    }

    // Creates a new product and sets its creation timestamp.
    public async Task<ProductDto?> CreateAsync(ProductDto dto)
    {
        var product = new Product { Name = dto.Name ?? string.Empty, Price = dto.Price, CreatedAt = DateTime.UtcNow };
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        dto.Id = product.Id;
        dto.CreatedAt = product.CreatedAt;
        return dto;
    }

    // Soft deletes a product by setting DeletedAt.
    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return false;
        if (product.DeletedAt != null) return false; // already deleted

        product.DeletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }
}
