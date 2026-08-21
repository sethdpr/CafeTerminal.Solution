namespace CafeTerminal.Shared.DTOs
{
    public class CreateOrderItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateOrderRequest
    {
        public int TableNumber { get; set; }
        public List<CreateOrderItemRequest> Items { get; set; } = new();
    }
}
