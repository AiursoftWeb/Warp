using System.Diagnostics.CodeAnalysis;
using Aiursoft.DbTools;
using Aiursoft.Warp.Entities;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Warp;

[ExcludeFromCodeCoverage]
public abstract class Program
{
    public static async Task Main(string[] args)
    {
        var app = await AppAsync<Startup>(args);
        await app.UpdateDbAsync<WarpDbContext>();
        await app.SeedAsync();
        await app.CopyAvatarFileAsync();
        await app.RunAsync();
    }
}
