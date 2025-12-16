using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CafeTerminal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("register")]
        public IActionResult Register()
        {
            return Ok("Register endpoint placeholder");
        }

        [HttpPost("login")]
        public IActionResult Login()
        {
            return Ok("Login endpoint placeholder");
        }
    }
}
