using CafeTerminal.Shared.DTOs;

namespace CafeTerminal.Api.Services;

public interface IOrderService
{
    Task<OrderDto> CreateAsync(CreateOrderRequest request);
    Task<List<OrderDto>> GetOrdersForTableAsync(int tableNumber);
    Task InitializeAsync();
}
