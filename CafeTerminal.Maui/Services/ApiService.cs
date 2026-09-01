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

        // Builds a default client for cases where dependency injection is not available yet.
        public static HttpClient CreateFallbackHttpClient()
        {
            return new HttpClient
            {
#if ANDROID
                BaseAddress = new Uri("http://10.0.2.2:5006/")
#else
                BaseAddress = new Uri("https://localhost:7232/")
#endif
            };
        }

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Reads the stored JWT and attaches it to the next outgoing API request.
        private async Task AttachAuthHeaderAsync()
        {
            var token = await SecureStorage.Default.GetAsync("auth_token");
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // Loads all tables from the API for the table overview page.
        public async Task<List<TableDto>> GetTablesAsync()
        {
            await AttachAuthHeaderAsync();

            // Request the current table list from the backend.
            var resp = await _httpClient.GetAsync("/api/tables");
            resp.EnsureSuccessStatusCode();

            // Deserialize the JSON payload into shared DTOs for the UI.
            var json = await resp.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<TableDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<TableDto>();
            return list;
        }

        // Sends an updated display name for a specific table.
        public async Task<bool> SetTableNameAsync(int number, string name)
        {
            await AttachAuthHeaderAsync();

            // Build the table payload expected by the API.
            var payload = new TableDto { Number = number, Name = name };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Submit the update and return whether the API accepted it.
            var resp = await _httpClient.PutAsync($"/api/tables/{number}", content);
            return resp.IsSuccessStatusCode;
        }

        // Loads all active products from the API for product and order screens.
        public async Task<List<ProductDto>> GetProductsAsync()
        {
            await AttachAuthHeaderAsync();

            // Request the active product catalog.
            var resp = await _httpClient.GetAsync("/api/products");
            resp.EnsureSuccessStatusCode();

            // Convert the JSON response into product DTOs.
            var json = await resp.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<ProductDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ProductDto>();
            return list;
        }

        // Creates a new product and returns the created product from the API.
        public async Task<ProductDto?> CreateProductAsync(string name, decimal price)
        {
            await AttachAuthHeaderAsync();

            // Serialize the new product data into the request body.
            var payload = new ProductDto { Name = name ?? string.Empty, Price = price };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Send the create request and surface API errors with their original message.
            var resp = await _httpClient.PostAsync("/api/products", content);
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(error)
                    ? "De API kon het product niet aanmaken."
                    : error);
            }

            // Parse the created product returned by the backend.
            var responseJson = await resp.Content.ReadAsStringAsync();
            var created = JsonSerializer.Deserialize<ProductDto>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return created;
        }

        // Soft deletes an existing product through the API.
        public async Task<bool> DeleteProductAsync(int id)
        {
            await AttachAuthHeaderAsync();

            var resp = await _httpClient.DeleteAsync($"/api/products/{id}");
            return resp.IsSuccessStatusCode;
        }

        // Sends a new order request for the selected table and products.
        public async Task<OrderDto?> CreateOrderAsync(CreateOrderRequest request)
        {
            await AttachAuthHeaderAsync();

            // Serialize the order payload and submit it to the backend.
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync("/api/orders", content);
            if (!resp.IsSuccessStatusCode) return null;

            // Convert the created order response into a DTO for the UI.
            var responseJson = await resp.Content.ReadAsStringAsync();
            var created = JsonSerializer.Deserialize<OrderDto>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return created;
        }

        // Loads all active orders for one table.
        public async Task<List<OrderDto>> GetOrdersForTableAsync(int tableNumber)
        {
            await AttachAuthHeaderAsync();

            // Request the unpaid orders linked to the selected table.
            var resp = await _httpClient.GetAsync($"/api/orders/table/{tableNumber}");
            resp.EnsureSuccessStatusCode();

            // Deserialize the order list so it can be shown in the app.
            var json = await resp.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<OrderDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<OrderDto>();
            return list;
        }

        // Loads the combined payment summary for one table.
        public async Task<PaymentSummaryDto?> GetPaymentSummaryAsync(int tableNumber)
        {
            await AttachAuthHeaderAsync();

            // Ask the backend for unpaid orders and the grand total.
            var resp = await _httpClient.GetAsync($"/api/orders/table/{tableNumber}/payment-summary");
            if (!resp.IsSuccessStatusCode)
            {
                return null;
            }

            // Deserialize the payment summary for display in the payment screen.
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PaymentSummaryDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // Marks all unpaid orders for a table as paid.
        public async Task<bool> CompletePaymentAsync(int tableNumber)
        {
            await AttachAuthHeaderAsync();

            var resp = await _httpClient.PostAsync($"/api/orders/table/{tableNumber}/complete-payment", null);
            return resp.IsSuccessStatusCode;
        }
    }
}