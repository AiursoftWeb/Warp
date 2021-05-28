using Aiursoft.Warp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Warp.Data
{
    public class WarpDbContext : IdentityDbContext<WarpUser>
    {
        public WarpDbContext(DbContextOptions<WarpDbContext> options) : base(options)
        {
        }
    }
}
