namespace CafeTerminal.Shared.DTOs
{
    // This DTO represents one product line inside an order.
    public class OrderItemDto
    {
        // Product identifier.
        public int ProductId { get; set; }
        // Product name shown in the UI.
        public string ProductName { get; set; } = string.Empty;
        // Ordered quantity of the product.
        public int Quantity { get; set; }
        // Unit price used for this order line.
        public decimal UnitPrice { get; set; }
    }
}
