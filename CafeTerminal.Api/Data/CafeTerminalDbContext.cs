using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CafeTerminal.Api.Data
{
    public class CafeTerminalDbContext : IdentityDbContext<ApplicationUser>
    {
        public CafeTerminalDbContext(DbContextOptions<CafeTerminalDbContext> options)
            : base(options)
        {
        }
    }
}