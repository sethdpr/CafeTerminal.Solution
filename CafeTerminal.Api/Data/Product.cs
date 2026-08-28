namespace CafeTerminal.Api.Data
{
    // This entity represents one product that can be ordered in the app.
    public class Product
    {
        // Primary key of the product.
        public int Id { get; set; }
        // Product name shown in the UI.
        public string Name { get; set; } = string.Empty;
        // Current price of the product.
        public decimal Price { get; set; }
        // UTC timestamp when the product was created.
        public DateTime CreatedAt { get; set; }
        // UTC timestamp for soft delete; null means still active.
        public DateTime? DeletedAt { get; set; }
    }
}
