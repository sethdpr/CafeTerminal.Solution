using CafeTerminal.Api.Services;
using CafeTerminal.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CafeTerminal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _productService.GetAllAsync();
        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductDto dto)
    {
        if (dto == null)
            return BadRequest();

        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var created = await _productService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetAll), new { id = created?.Id }, created);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _productService.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
