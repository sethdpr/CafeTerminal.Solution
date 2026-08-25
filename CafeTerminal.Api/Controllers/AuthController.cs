using CafeTerminal.Api.Data;
using CafeTerminal.Api.Services;
using CafeTerminal.Shared.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CafeTerminal.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtService _jwtService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            JwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest) //this endpoint uses the RegisterRequest DTO. The Api validates the request input and saves it to the database
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var user = new ApplicationUser
            {
                UserName = registerRequest.Username,
                Email = registerRequest.Email,
                CreatedAt = DateTime.UtcNow
            }; //new instance of ApplicationUser gets created with the data from the RegisterRequest DTO

            var result = await _userManager.CreateAsync(user, registerRequest.Password); //the UserManager service creates the user in the database with the provided password

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors); //if the creation fails, return the errors
            }

            var token = _jwtService.GenerateToken(user); // Generate a JWT token for the newly registered user

            return Ok(new
            {
                message = "Gebruiker succesvol geregistreerd",
                token = token
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest) //this endpoint uses the LoginRequest DTO. The Api validates the request input and checks the credentials
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var user = await _userManager.FindByNameAsync(loginRequest.Username); //find the user by username

            if (user == null)
            {
                return Unauthorized("Ongeldige gebruikersnaam of wachtwoord"); //if user not found, return unauthorized
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, loginRequest.Password); //check if the provided password is correct

            if (!passwordValid)
            {
                return Unauthorized("Ongeldige gebruikersnaam of wachtwoord"); //if password is incorrect, return unauthorized
            }

            var token = _jwtService.GenerateToken(user); // Generate a JWT token for the authenticated user

            return Ok(new
            {
                message = "Inloggen succesvol",
                token = token
            });
        }
    }
}