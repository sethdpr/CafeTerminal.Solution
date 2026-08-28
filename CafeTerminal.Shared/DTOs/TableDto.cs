namespace CafeTerminal.Shared.DTOs
{
    // This DTO represents one table in the cafe.
    public class TableDto
    {
        // Table number shown on the tables page.
        public int Number { get; set; }
        // Current assigned name; empty means no active client assigned.
        public string Name { get; set; } = string.Empty;
    }
}
