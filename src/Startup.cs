using Aiursoft.Identity;
using Aiursoft.SDK;
using Aiursoft.Warp.Data;
using Aiursoft.Warp.Models;
using Aiursoft.Warpgate.SDK;
using Aiursoft.WebTools.Models;
using Microsoft.AspNetCore.Identity;

namespace Aiursoft.Warp
{
    public class Startup : IWebStartup
    {
        public void ConfigureServices(IConfiguration configuration, IWebHostEnvironment environment, IServiceCollection services)
        {
            services.AddDbContextForInfraApps<WarpDbContext>(configuration.GetConnectionString("DatabaseConnection"));

            services.AddIdentity<WarpUser, IdentityRole>()
                .AddEntityFrameworkStores<WarpDbContext>()
                .AddDefaultTokenProviders();

            services.AddAiursoftWebFeatures();
            services.AddAiursoftWarpgate(configuration.GetSection("AiursoftWarpgate"));
            services.AddAiursoftIdentity<WarpUser>(
                probeConfig: configuration.GetSection("AiursoftProbe"),
                authenticationConfig: configuration.GetSection("AiursoftAuthentication"),
                observerConfig: configuration.GetSection("AiursoftObserver"));
        }

        public void Configure(WebApplication app)
        {
            app.UseAiursoftHandler(app.Environment.IsDevelopment());
            app.UseAiursoftAppRouters();
        }
    }
}
