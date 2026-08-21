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
}
