using CafeTerminal.Api.Services;
using CafeTerminal.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace CafeTerminal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _service;

        public OrdersController(OrderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? tableNumber)
        {
            if (tableNumber.HasValue)
            {
                var ordersByTable = await _service.GetByTableIdAsync(tableNumber.Value);
                return Ok(ordersByTable);
            }

            var allOrders = await _service.GetAllAsync();
            return Ok(allOrders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _service.GetByIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Order order)
        {
            order.CreatedAt = DateTime.UtcNow;
            var created = await _service.CreateAsync(order);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Order order)
        {
            if (id != order.Id) return BadRequest();

            var updated = await _service.UpdateAsync(id, order);
            if (updated == null) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}