using CafeTerminal.Api.Services;
using CafeTerminal.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CafeTerminal.Api.Controllers;

// This controller manages product data used by the ordering flow.
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
    // Returns all active products that can still be ordered.
    public async Task<IActionResult> GetAll()
    {
        var list = await _productService.GetAllAsync();
        return Ok(list);
    }

    [HttpPost]
    // Creates a new product with a name, price, and creation timestamp.
    public async Task<IActionResult> Create(ProductDto dto)
    {
        if (dto == null)
            return BadRequest();

        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var created = await _productService.CreateAsync(dto);
        return Ok(created);
    }

    [HttpDelete("{id}")]
    // Soft deletes a product by setting its DeletedAt timestamp.
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _productService.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
