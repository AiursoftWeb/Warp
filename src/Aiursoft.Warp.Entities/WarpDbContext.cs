using Aiursoft.DbTools;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Warp.Entities;

public abstract class WarpDbContext(DbContextOptions options) : IdentityDbContext<User>(options), ICanMigrate
{
    public virtual  Task MigrateAsync(CancellationToken cancellationToken) =>
        Database.MigrateAsync(cancellationToken);

    public virtual  Task<bool> CanConnectAsync() =>
        Database.CanConnectAsync();

        public DbSet<ShorterLink> ShorterLinks => Set<ShorterLink>();
    public DbSet<WarpApiKey> WarpApiKeys => Set<WarpApiKey>();
    public DbSet<WarpHit> WarpHits => Set<WarpHit>();
}
