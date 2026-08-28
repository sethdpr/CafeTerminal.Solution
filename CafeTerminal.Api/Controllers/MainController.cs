using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeTerminal.Api.Controllers
{
    // This controller contains simple protected test endpoints for authentication checks.
    [ApiController]
    [Route("api/[controller]")]
    public class MainController : ControllerBase
    {
        [Authorize]
        [HttpGet("test")]
        // Returns a success message when the caller is authenticated with a valid JWT.
        public IActionResult Test()
        {
            return Ok("Je bent correct geauthenticeerd.");
        }
    }
}