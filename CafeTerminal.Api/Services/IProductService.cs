using CafeTerminal.Shared.DTOs;

namespace CafeTerminal.Api.Services;

public interface IProductService
{
    Task<List<ProductDto>> GetAllAsync();
    Task<ProductDto?> CreateAsync(ProductDto dto);
    Task InitializeAsync();
    Task<bool> DeleteAsync(int id);
}
