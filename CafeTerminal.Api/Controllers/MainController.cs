using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeTerminal.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MainController : ControllerBase
    {
        [Authorize]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("Je bent correct geauthenticeerd.");
        }
    }
}