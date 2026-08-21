namespace CafeTerminal.Api.Data
{
    public class Order
    {
        public int Id { get; set; }
        public int TableNumber { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaymentDate { get; set; }

        public List<OrderItem> Items { get; set; } = new();
    }
}
