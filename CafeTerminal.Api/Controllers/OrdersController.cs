using CafeTerminal.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CafeTerminal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private static readonly List<Order> _orders = new();

        [HttpGet]
        public IActionResult GetAll([FromQuery] int? tableNumber)
        {
            if (tableNumber == null)
                return Ok(_orders);

            return Ok(_orders.Where(o => o.TableId == tableNumber));
        }

        [HttpPost]
        public IActionResult Create(Order order)
        {
            order.Id = _orders.Count + 1;
            order.CreatedAt = DateTime.UtcNow;
            _orders.Add(order);
            return Ok(order);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Order order)
        {
            var existing = _orders.FirstOrDefault(o => o.Id == id);
            if (existing == null) return NotFound();

            existing.TableId = order.TableId;
            existing.OrderLines = order.OrderLines;
            existing.IsClosed = order.IsClosed;

            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var order = _orders.FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();

            _orders.Remove(order);
            return NoContent();
        }
    }
}
