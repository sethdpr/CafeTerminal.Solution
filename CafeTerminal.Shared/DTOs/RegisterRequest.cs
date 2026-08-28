using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeTerminal.Shared.DTOs
{
    // This DTO is sent to the API when a user registers.
    public class RegisterRequest
    {
        [Required]
        // Username chosen by the user.
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        // Email address entered during registration.
        public string Email { get; set; }
        [Required]
        // Password chosen by the user.
        public string Password { get; set; }
    }
}
