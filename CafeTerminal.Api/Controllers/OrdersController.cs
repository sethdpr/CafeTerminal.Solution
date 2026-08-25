using CafeTerminal.Api.Services;
using CafeTerminal.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CafeTerminal.Api.Controllers;

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
    public async Task<IActionResult> Create(CreateOrderRequest request)
    {
        if (request == null) return BadRequest();

        var created = await _orderService.CreateAsync(request);
        return CreatedAtAction(nameof(GetForTable), new { tableNumber = created.TableNumber }, created);
    }

    [HttpGet("table/{tableNumber}")]
    public async Task<IActionResult> GetForTable(int tableNumber)
    {
        var list = await _orderService.GetOrdersForTableAsync(tableNumber);
        return Ok(list);
    }

    [HttpGet("table/{tableNumber}/payment-summary")]
    public async Task<IActionResult> GetPaymentSummary(int tableNumber)
    {
        var summary = await _orderService.GetPaymentSummaryAsync(tableNumber);
        return Ok(summary);
    }

    [HttpPost("table/{tableNumber}/complete-payment")]
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
