using CafeTerminal.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CafeTerminal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TablesController : ControllerBase
    {
        private static readonly List<Table> _tables = new();

        [HttpGet]
        public IActionResult GetAll() => Ok(_tables);

        [HttpPost]
        public IActionResult Create(Table table)
        {
            table.Id = _tables.Count + 1;
            _tables.Add(table);
            return Ok(table);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Table table)
        {
            var existing = _tables.FirstOrDefault(t => t.Id == id);
            if (existing == null) return NotFound();

            existing.Name = table.Name;
            existing.Number = table.Number;

            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var table = _tables.FirstOrDefault(t => t.Id == id);
            if (table == null) return NotFound();

            _tables.Remove(table);
            return NoContent();
        }
    }
}
