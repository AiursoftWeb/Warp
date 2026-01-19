using Aiursoft.Warp.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Warp.InMemory;

public class InMemoryContext(DbContextOptions<InMemoryContext> options) : WarpDbContext(options)
{
    public override Task MigrateAsync(CancellationToken cancellationToken)
    {
        return Database.EnsureCreatedAsync(cancellationToken);
    }

    public override Task<bool> CanConnectAsync()
    {
        return Task.FromResult(true);
    }
}
