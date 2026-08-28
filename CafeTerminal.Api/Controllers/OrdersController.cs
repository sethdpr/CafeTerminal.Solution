using CafeTerminal.Api.Services;
using CafeTerminal.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CafeTerminal.Api.Controllers;

// This controller exposes API endpoints for creating orders, reading table orders,
// showing payment summaries, and completing payments.
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    // Creates a new order for a table based on the selected products and quantities.
    public async Task<IActionResult> Create(CreateOrderRequest request)
    {
        if (request == null) return BadRequest();

        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var created = await _orderService.CreateAsync(request);
        return CreatedAtAction(nameof(GetForTable), new { tableNumber = created.TableNumber }, created);
    }

    [HttpGet("table/{tableNumber}")]
    // Returns the active unpaid orders for a specific table.
    public async Task<IActionResult> GetForTable(int tableNumber)
    {
        var list = await _orderService.GetOrdersForTableAsync(tableNumber);
        return Ok(list);
    }

    [HttpGet("table/{tableNumber}/payment-summary")]
    // Returns all unpaid orders for a table together with one combined total price.
    public async Task<IActionResult> GetPaymentSummary(int tableNumber)
    {
        var summary = await _orderService.GetPaymentSummaryAsync(tableNumber);
        return Ok(summary);
    }

    [HttpPost("table/{tableNumber}/complete-payment")]
    // Marks all unpaid orders for a table as paid and frees the table for the next client.
    public async Task<IActionResult> CompletePayment(int tableNumber)
    {
        var success = await _orderService.CompletePaymentAsync(tableNumber);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}
