using CafeTerminal.Shared.DTOs;

namespace CafeTerminal.Api.Services;

// This interface describes all order-related operations used by the API.
public interface IOrderService
{
    // Creates a new order for a table.
    Task<OrderDto> CreateAsync(CreateOrderRequest request);
    // Returns the unpaid orders for one table.
    Task<List<OrderDto>> GetOrdersForTableAsync(int tableNumber);
    // Ensures the required database tables exist.
    Task InitializeAsync();
    // Returns the payment summary for one table.
    Task<PaymentSummaryDto> GetPaymentSummaryAsync(int tableNumber);
    // Completes payment for one table and closes its active orders.
    Task<bool> CompletePaymentAsync(int tableNumber);
}
