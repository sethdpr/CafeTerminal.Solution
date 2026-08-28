using CafeTerminal.Shared.DTOs;

namespace CafeTerminal.Api.Services;

// This interface describes all table-related operations used by the API.
public interface ITableService
{
    // Returns all tables.
    Task<List<TableDto>> GetAllAsync();
    // Sets the name of one table.
    Task<bool> SetNameAsync(int number, string name);
    // Ensures the required database table and default rows exist.
    Task InitializeAsync();
    // Clears the table name after payment is completed.
    Task<bool> ClearNameAsync(int number);
}
