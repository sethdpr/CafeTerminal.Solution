namespace CafeTerminal.Api.Data
{
    // This entity represents one order placed for a specific table.
    public class Order
    {
        // Primary key of the order.
        public int Id { get; set; }
        // Table number the order belongs to.
        public int TableNumber { get; set; }
        // Total price of all items in the order.
        public decimal TotalPrice { get; set; }
        // UTC timestamp when the order was created.
        public DateTime CreatedAt { get; set; }
        // UTC timestamp when the order was paid; null means still active.
        public DateTime? PaymentDate { get; set; }

        // Collection of product rows that belong to this order.
        public List<OrderItem> Items { get; set; } = new();
    }
}
