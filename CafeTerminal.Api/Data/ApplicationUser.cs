using Microsoft.AspNetCore.Identity;

namespace CafeTerminal.Api.Data
{
    // This extends the default Identity user with the account creation date.
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}