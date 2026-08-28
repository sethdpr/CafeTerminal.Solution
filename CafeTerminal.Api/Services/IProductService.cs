using CafeTerminal.Shared.DTOs;

namespace CafeTerminal.Api.Services;

// This interface describes all product-related operations used by the API.
public interface IProductService
{
    // Returns all active products.
    Task<List<ProductDto>> GetAllAsync();
    // Creates one new product.
    Task<ProductDto?> CreateAsync(ProductDto dto);
    // Ensures the required database table and columns exist.
    Task InitializeAsync();
    // Soft deletes a product.
    Task<bool> DeleteAsync(int id);
}
