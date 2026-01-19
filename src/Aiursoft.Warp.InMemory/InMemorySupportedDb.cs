using Aiursoft.DbTools;
using Aiursoft.DbTools.InMemory;
using Aiursoft.Warp.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Warp.InMemory;

public class InMemorySupportedDb : SupportedDatabaseType<WarpDbContext>
{
    public override string DbType => "InMemory";

    public override IServiceCollection RegisterFunction(IServiceCollection services, string connectionString)
    {
        return services.AddAiurInMemoryDb<InMemoryContext>();
    }

    public override WarpDbContext ContextResolver(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<InMemoryContext>();
    }
}
