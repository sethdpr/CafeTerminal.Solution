using CafeTerminal.Api.Services;
using CafeTerminal.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CafeTerminal.Api.Controllers;

// This controller manages the tables that are shown in the MAUI app.
[ApiController]
[Route("api/[controller]")]
public class TablesController : ControllerBase
{
    private readonly ITableService _tableService;

    public TablesController(ITableService tableService)
    {
        _tableService = tableService;
    }

    [HttpGet]
    // Returns all tables with their number and current assigned name.
    public async Task<IActionResult> GetAll()
    {
        var list = await _tableService.GetAllAsync();
        return Ok(list);
    }

    [HttpPut("{number}")]
    // Assigns or updates the name of one table.
    public async Task<IActionResult> SetName(int number, [FromBody] TableDto payload)
    {
        if (payload == null)
            return BadRequest();

        var success = await _tableService.SetNameAsync(number, payload.Name);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
