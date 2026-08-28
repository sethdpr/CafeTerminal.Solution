namespace CafeTerminal.Api.Data
{
    // This entity represents one physical table in the cafe.
    public class Table
    {
        // Use the table number as the key.
        public int Number { get; set; }
        // Current name assigned to the table; empty means free.
        public string Name { get; set; } = string.Empty;
    }
}
