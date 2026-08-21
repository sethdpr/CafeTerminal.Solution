using CafeTerminal.Shared.DTOs;
using CafeTerminal.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace CafeTerminal.Api.Services;

public class TableService : ITableService
{
    private readonly CafeTerminalDbContext _db;

    public TableService(CafeTerminalDbContext db)
    {
        _db = db;
    }

    public async Task InitializeAsync()
    {
        // Create the table if it does not exist (handles databases created before this entity was added)
        try
        {
            // Try a simple query to see if the table exists
            var any = await _db.Tables.AnyAsync();
        }
        catch (Microsoft.Data.SqlClient.SqlException ex)
        {
            // If the table doesn't exist, create it with a simple SQL statement
            if (ex.Message.Contains("Invalid object name", StringComparison.OrdinalIgnoreCase))
            {
                var createSql = @"IF OBJECT_ID(N'dbo.Tables', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Tables](
        [Number] int NOT NULL PRIMARY KEY,
        [Name] nvarchar(max) NULL
    );
END";

                await _db.Database.ExecuteSqlRawAsync(createSql);
            }
            else
            {
                throw;
            }
        }

        // Ensure rows 1..10 exist
        for (int i = 1; i <= 10; i++)
        {
            var exists = await _db.Tables.AnyAsync(t => t.Number == i);
            if (!exists)
            {
                _db.Tables.Add(new Table { Number = i, Name = string.Empty });
            }
        }

        await _db.SaveChangesAsync();
    }

    public async Task<List<TableDto>> GetAllAsync()
    {
        var list = await _db.Tables
            .OrderBy(t => t.Number)
            .Select(t => new TableDto { Number = t.Number, Name = t.Name })
            .ToListAsync();

        return list;
    }

    public async Task<bool> SetNameAsync(int number, string name)
    {
        var table = await _db.Tables.FirstOrDefaultAsync(t => t.Number == number);
        if (table == null)
            return false;

        table.Name = name ?? string.Empty;
        await _db.SaveChangesAsync();
        return true;
    }
}
