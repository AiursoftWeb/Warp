using Aiursoft.DbTools;
using Aiursoft.Warp.Data;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Warp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var app = App<Startup>(args);
            await app.UpdateDbAsync<WarpDbContext>(UpdateMode.MigrateThenUse);
            await app.RunAsync();
        }
    }
}

