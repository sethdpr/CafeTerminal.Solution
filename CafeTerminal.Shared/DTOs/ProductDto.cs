using System.ComponentModel.DataAnnotations;

namespace CafeTerminal.Shared.DTOs
{
    // This DTO represents one product exchanged between API and MAUI app.
    public class ProductDto
    {
        // Unique identifier of the product.
        public int Id { get; set; }

        [Required(ErrorMessage = "Productnaam is verplicht.")]
        // Product name shown to the user.
        public string Name { get; set; } = string.Empty;

        [Range(0.01, 9999999.0, ErrorMessage = "Prijs moet groter zijn dan 0.")]
        // Product price.
        public decimal Price { get; set; }

        // UTC timestamp when the product was created.
        public DateTime CreatedAt { get; set; }
        // UTC timestamp for soft delete; null means active.
        public DateTime? DeletedAt { get; set; }
    }
}
