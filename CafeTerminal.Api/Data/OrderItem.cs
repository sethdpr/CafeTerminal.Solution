namespace CafeTerminal.Api.Data
{
    // This entity represents one product line inside an order.
    public class OrderItem
    {
        // Primary key of the order item.
        public int Id { get; set; }
        // Foreign key to the parent order.
        public int OrderId { get; set; }
        // Navigation property to the parent order.
        public Order? Order { get; set; }

        // Product that was ordered.
        public int ProductId { get; set; }
        // Quantity of that product in the order.
        public int Quantity { get; set; }
        // Unit price copied at the time of ordering.
        public decimal UnitPrice { get; set; }
    }
}
