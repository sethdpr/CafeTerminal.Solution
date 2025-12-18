using CafeTerminal.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CafeTerminal.Maui.Services
{
    public class OrderService
    {
        private readonly HttpClient _http;

        public OrderService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Order>> GetOrdersAsync(int? tableNumber = null)
        {
            string url = "api/orders";
            if (tableNumber.HasValue)
                url += $"?tableNumber={tableNumber.Value}";

            return await _http.GetFromJsonAsync<List<Order>>(url) ?? new List<Order>();
        }

        public async Task<Order?> CreateOrderAsync(Order order)
        {
            var response = await _http.PostAsJsonAsync("api/orders", order);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Order>();
            return null;
        }

        public async Task<bool> UpdateOrderAsync(Order order)
        {
            var response = await _http.PutAsJsonAsync($"api/orders/{order.Id}", order);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/orders/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
