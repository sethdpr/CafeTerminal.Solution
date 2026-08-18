using System.Net.Http.Headers;
using CafeTerminal.Shared.DTOs;
using System.Text.Json;
using System.Text;

namespace CafeTerminal.Maui.Services
{
    public class ApiService /*This service bundles some functionality:
                             1. Uses DI-provided HttpClient (configured in MauiProgram)
                             2. Adds JWT token from SecureStorage to requests
                             3. Provides strongly-typed methods for API endpoints used by the app*/
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private async Task AttachAuthHeaderAsync()
        {
            var token = await SecureStorage.Default.GetAsync("auth_token");
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<TableDto>> GetTablesAsync()
        {
            await AttachAuthHeaderAsync();

            var resp = await _httpClient.GetAsync("/api/tables");
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<TableDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<TableDto>();
            return list;
        }

        public async Task<bool> SetTableNameAsync(int number, string name)
        {
            await AttachAuthHeaderAsync();

            var payload = new TableDto { Number = number, Name = name };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var resp = await _httpClient.PutAsync($"/api/tables/{number}", content);
            return resp.IsSuccessStatusCode;
        }
    }
}