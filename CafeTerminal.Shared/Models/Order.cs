using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeTerminal.Shared.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public Table? Table { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsClosed { get; set; }
        public List<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
    }
}
