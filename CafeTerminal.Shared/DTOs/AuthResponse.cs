using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeTerminal.Shared.DTOs
{
    // This DTO is returned after a successful login or registration.
    public class AuthResponse
    {
        // Human-readable status message from the API.
        public string Message { get; set; } = string.Empty;
        // JWT token used for authenticated API calls.
        public string Token { get; set; } = string.Empty;
    }
}
