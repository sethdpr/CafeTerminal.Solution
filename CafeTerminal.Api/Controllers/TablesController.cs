using CafeTerminal.Api.Services;
using CafeTerminal.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace CafeTerminal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TablesController : ControllerBase
    {
        private readonly TableService _service;

        public TablesController(TableService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tables = await _service.GetAllAsync();
            return Ok(tables);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var table = await _service.GetAllAsync(); // optioneel: voeg GetByIdAsync toe in service
            var t = table.FirstOrDefault(x => x.Id == id);
            if (t == null) return NotFound();
            return Ok(t);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Table table)
        {
            var created = await _service.CreateAsync(table);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Table table)
        {
            if (id != table.Id) return BadRequest();

            var updated = await _service.UpdateAsync(id, table);
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