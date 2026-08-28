using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeTerminal.Shared.DTOs
{
    // This DTO is sent to the API when a user logs in.
    public class LoginRequest
    {
        [Required]
        // Username entered on the login page.
        public string Username { get; set; }
        [Required]
        // Password entered on the login page.
        public string Password { get; set; }
    }
}
