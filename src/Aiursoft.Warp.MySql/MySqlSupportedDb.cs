using Aiursoft.DbTools;
using Aiursoft.DbTools.MySql;
using Aiursoft.Warp.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.Warp.MySql;

public class MySqlSupportedDb(bool allowCache, bool splitQuery) : SupportedDatabaseType<WarpDbContext>
{
    public override string DbType => "MySql";

    public override IServiceCollection RegisterFunction(IServiceCollection services, string connectionString)
    {
        return services.AddAiurMySqlWithCache<MySqlContext>(
            connectionString,
            splitQuery: splitQuery,
            allowCache: allowCache);
    }

    public override WarpDbContext ContextResolver(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<MySqlContext>();
    }
}
