namespace CafeTerminal.Shared.DTOs
{
    public class PaymentSummaryDto
    {
        public int TableNumber { get; set; }
        public string TableName { get; set; } = string.Empty;
        public List<OrderDto> Orders { get; set; } = new();
        public decimal TotalPrice { get; set; }
    }
}
