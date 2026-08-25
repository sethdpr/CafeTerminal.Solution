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

        public async Task<List<ProductDto>> GetProductsAsync()
        {
            await AttachAuthHeaderAsync();

            var resp = await _httpClient.GetAsync("/api/products");
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<ProductDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ProductDto>();
            return list;
        }

        public async Task<ProductDto?> CreateProductAsync(string name, decimal price)
        {
            await AttachAuthHeaderAsync();

            var payload = new ProductDto { Name = name ?? string.Empty, Price = price };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var resp = await _httpClient.PostAsync("/api/products", content);
            if (!resp.IsSuccessStatusCode)
                return null;

            var responseJson = await resp.Content.ReadAsStringAsync();
            var created = JsonSerializer.Deserialize<ProductDto>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return created;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            await AttachAuthHeaderAsync();

            var resp = await _httpClient.DeleteAsync($"/api/products/{id}");
            return resp.IsSuccessStatusCode;
        }

        public async Task<OrderDto?> CreateOrderAsync(CreateOrderRequest request)
        {
            await AttachAuthHeaderAsync();

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync("/api/orders", content);
            if (!resp.IsSuccessStatusCode) return null;

            var responseJson = await resp.Content.ReadAsStringAsync();
            var created = JsonSerializer.Deserialize<OrderDto>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return created;
        }

        public async Task<List<OrderDto>> GetOrdersForTableAsync(int tableNumber)
        {
            await AttachAuthHeaderAsync();

            var resp = await _httpClient.GetAsync($"/api/orders/table/{tableNumber}");
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<OrderDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<OrderDto>();
            return list;
        }

        public async Task<PaymentSummaryDto?> GetPaymentSummaryAsync(int tableNumber)
        {
            await AttachAuthHeaderAsync();

            var resp = await _httpClient.GetAsync($"/api/orders/table/{tableNumber}/payment-summary");
            if (!resp.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PaymentSummaryDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<bool> CompletePaymentAsync(int tableNumber)
        {
            await AttachAuthHeaderAsync();

            var resp = await _httpClient.PostAsync($"/api/orders/table/{tableNumber}/complete-payment", null);
            return resp.IsSuccessStatusCode;
        }
    }
}