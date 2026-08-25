using System.ComponentModel.DataAnnotations;

namespace CafeTerminal.Shared.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Productnaam is verplicht.")]
        public string Name { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "9999999", ErrorMessage = "Prijs moet groter zijn dan 0.")]
        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
