namespace CafeTerminal.Shared.DTOs
{
    // This DTO contains the payment overview for one table.
    public class PaymentSummaryDto
    {
        // Table number for the payment summary.
        public int TableNumber { get; set; }
        // Current assigned table name.
        public string TableName { get; set; } = string.Empty;
        // Unpaid orders that belong to the table.
        public List<OrderDto> Orders { get; set; } = new();
        // Combined total price of all unpaid orders.
        public decimal TotalPrice { get; set; }
    }
}
