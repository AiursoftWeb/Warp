using Aiursoft.Warp.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Warp.Sqlite;

public class SqliteContext(DbContextOptions<SqliteContext> options) : TemplateDbContext(options)
{
    public override Task<bool> CanConnectAsync()
    {
        return Task.FromResult(true);
    }
}
