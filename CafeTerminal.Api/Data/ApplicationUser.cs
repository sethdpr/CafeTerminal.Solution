using Microsoft.AspNetCore.Identity;

namespace CafeTerminal.Api.Data
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}