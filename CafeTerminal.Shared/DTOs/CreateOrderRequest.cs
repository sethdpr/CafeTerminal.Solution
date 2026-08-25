using System.ComponentModel.DataAnnotations;

namespace CafeTerminal.Shared.DTOs
{
    public class CreateOrderItemRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Kies een geldig product.")]
        public int ProductId { get; set; }

        [Range(1, 100, ErrorMessage = "Hoeveelheid moet minstens 1 zijn.")]
        public int Quantity { get; set; }
    }

    public class CreateOrderRequest
    {
        [Range(1, 10, ErrorMessage = "Kies een geldige tafel.")]
        public int TableNumber { get; set; }

        [MinLength(1, ErrorMessage = "Voeg minstens één product toe aan de bestelling.")]
        public List<CreateOrderItemRequest> Items { get; set; } = new();
    }
}
