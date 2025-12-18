using CafeTerminal.Shared.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace CafeTerminal.Maui.Services;

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
    {
        var response = await _http.PostAsJsonAsync("api/products", product);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<Product>();
        return null;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/products/{id}");
        return response.IsSuccessStatusCode;
    }
}