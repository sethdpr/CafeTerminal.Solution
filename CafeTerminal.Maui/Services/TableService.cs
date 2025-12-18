using CafeTerminal.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CafeTerminal.Maui.Services
{
    public class TableService
    {
        private readonly HttpClient _http;

        public TableService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Table>> GetTablesAsync()
        {
            return await _http.GetFromJsonAsync<List<Table>>("api/tables") ?? new List<Table>();
        }

        public async Task<Table?> CreateTableAsync(Table table)
        {
            var response = await _http.PostAsJsonAsync("api/tables", table);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Table>();
            return null;
        }

        public async Task<bool> UpdateTableAsync(Table table)
        {
            var response = await _http.PutAsJsonAsync($"api/tables/{table.Id}", table);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTableAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/tables/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
