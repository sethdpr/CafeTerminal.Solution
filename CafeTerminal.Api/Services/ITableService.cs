using CafeTerminal.Shared.DTOs;

namespace CafeTerminal.Api.Services;

public interface ITableService
{
    Task<List<TableDto>> GetAllAsync();
    Task<bool> SetNameAsync(int number, string name);
    Task InitializeAsync();
    Task<bool> ClearNameAsync(int number);
}
