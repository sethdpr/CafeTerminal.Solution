using CafeTerminal.Shared.DTOs;

namespace CafeTerminal.Api.Services;

public class TableService : ITableService
{
    // Simple in-memory store for table names. Key: table number
    private readonly Dictionary<int, string> _tables = new();

    public TableService()
    {
        // initialize 1..10
        for (int i = 1; i <= 10; i++)
            _tables[i] = string.Empty;
    }

    public Task<List<TableDto>> GetAllAsync()
    {
        var list = _tables.Select(kv => new TableDto { Number = kv.Key, Name = kv.Value }).ToList();
        return Task.FromResult(list);
    }

    public Task<bool> SetNameAsync(int number, string name)
    {
        if (!_tables.ContainsKey(number))
            return Task.FromResult(false);

        _tables[number] = name ?? string.Empty;
        return Task.FromResult(true);
    }
}
