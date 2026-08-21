namespace CafeTerminal.Shared.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int TableNumber { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}
