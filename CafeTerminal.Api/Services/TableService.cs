using CafeTerminal.Shared.DTOs;
using CafeTerminal.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace CafeTerminal.Api.Services;

// This service contains the business logic for cafe tables.
public class TableService : ITableService
{
    private readonly CafeTerminalDbContext _db;

    public TableService(CafeTerminalDbContext db)
    {
        _db = db;
    }

    // Creates the Tables table if needed and ensures tables 1 through 10 exist.
    public async Task InitializeAsync()
    {
        // Create the table if it does not exist (handles databases created before this entity was added)
        try
        {
            // Probe the table so missing-table errors can be handled explicitly.
            // Try a simple query to see if the table exists
            var any = await _db.Tables.AnyAsync();
        }
        catch (Microsoft.Data.SqlClient.SqlException ex)
        {
            // If the table doesn't exist, create it with a simple SQL statement
            if (ex.Message.Contains("Invalid object name", StringComparison.OrdinalIgnoreCase))
            {
                // Create the Tables table for legacy databases that do not have it yet.
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
            // Seed the fixed set of cafe tables when one is missing.
            var exists = await _db.Tables.AnyAsync(t => t.Number == i);
            if (!exists)
            {
                _db.Tables.Add(new Table { Number = i, Name = string.Empty });
            }
        }

        await _db.SaveChangesAsync();
    }

    // Returns all tables ordered by table number.
    public async Task<List<TableDto>> GetAllAsync()
    {
        var list = await _db.Tables
            .OrderBy(t => t.Number)
            .Select(t => new TableDto { Number = t.Number, Name = t.Name })
            .ToListAsync();

        return list;
    }

    // Updates the name assigned to one table.
    public async Task<bool> SetNameAsync(int number, string name)
    {
        // Find the table that should receive the new display name.
        var table = await _db.Tables.FirstOrDefaultAsync(t => t.Number == number);
        if (table == null)
            return false;

        // Store the new name and persist the change.
        table.Name = name ?? string.Empty;
        await _db.SaveChangesAsync();
        return true;
    }

    // Clears the name of one table after payment is finished.
    public async Task<bool> ClearNameAsync(int number)
    {
        // Find the table that should be reset after payment.
        var table = await _db.Tables.FirstOrDefaultAsync(t => t.Number == number);
        if (table == null)
            return false;

        // Clear the assigned name so the table appears free again.
        table.Name = string.Empty;
        await _db.SaveChangesAsync();
        return true;
    }
}
