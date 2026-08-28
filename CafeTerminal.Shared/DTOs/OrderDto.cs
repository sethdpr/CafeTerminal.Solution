namespace CafeTerminal.Shared.DTOs
{
    // This DTO represents one order returned by the API.
    public class OrderDto
    {
        // Unique identifier of the order.
        public int Id { get; set; }
        // Table number the order belongs to.
        public int TableNumber { get; set; }
        // Product rows inside the order.
        public List<OrderItemDto> Items { get; set; } = new();
        // Total price of the full order.
        public decimal TotalPrice { get; set; }
        // UTC timestamp when the order was created.
        public DateTime CreatedAt { get; set; }
        // UTC timestamp when the order was paid; null means unpaid.
        public DateTime? PaymentDate { get; set; }
    }
}
