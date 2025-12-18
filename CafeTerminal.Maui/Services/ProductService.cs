using CafeTerminal.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CafeTerminal.Maui.Services
{
    public class ProductService
    {
        private readonly HttpClient _http;

        public ProductService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Product>> GetProductsAsync()
            => await _http.GetFromJsonAsync<List<Product>>("api/products") ?? new List<Product>();

        public async Task<Product?> CreateProductAsync(Product product)
            => await _http.PostAsJsonAsync("api/products", product)
                         .ContinueWith(t => t.Result.IsSuccessStatusCode
                         ? t.Result.Content.ReadFromJsonAsync<Product>().Result
                         : null);

        public async Task<bool> UpdateProductAsync(Product product)
            => (await _http.PutAsJsonAsync($"api/products/{product.Id}", product)).IsSuccessStatusCode;

        public async Task<bool> DeleteProductAsync(int id)
            => (await _http.DeleteAsync($"api/products/{id}")).IsSuccessStatusCode;
    }
}
