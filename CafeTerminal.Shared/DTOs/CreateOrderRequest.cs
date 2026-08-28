using System.ComponentModel.DataAnnotations;

namespace CafeTerminal.Shared.DTOs
{
    // This DTO represents one product row in a create-order request.
    public class CreateOrderItemRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Kies een geldig product.")]
        // Product identifier to add to the order.
        public int ProductId { get; set; }

        [Range(1, 100, ErrorMessage = "Hoeveelheid moet minstens 1 zijn.")]
        // Quantity of the selected product.
        public int Quantity { get; set; }
    }

    // This DTO represents the full payload sent when creating a new order.
    public class CreateOrderRequest
    {
        [Range(1, 10, ErrorMessage = "Kies een geldige tafel.")]
        // Table number the order belongs to.
        public int TableNumber { get; set; }

        [MinLength(1, ErrorMessage = "Voeg minstens één product toe aan de bestelling.")]
        // Selected products and quantities for the order.
        public List<CreateOrderItemRequest> Items { get; set; } = new();
    }
}
