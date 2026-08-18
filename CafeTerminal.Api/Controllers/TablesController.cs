using CafeTerminal.Api.Services;
using CafeTerminal.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CafeTerminal.Api.Controllers;

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
    public async Task<IActionResult> GetAll()
    {
        var list = await _tableService.GetAllAsync();
        return Ok(list);
    }

    [HttpPut("{number}")]
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
