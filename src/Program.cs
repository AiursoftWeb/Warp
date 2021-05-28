using Aiursoft.SDK;
using Aiursoft.Warp.Data;
using Microsoft.Extensions.Hosting;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Warp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            App<Startup>(args).Update<WarpDbContext>().Run();
        }

        // For EF
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return BareApp<Startup>(args);
        }
    }
}

