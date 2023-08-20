using Aiursoft.Identity;
using Aiursoft.SDK;
using Aiursoft.Warp.Data;
using Aiursoft.Warp.Models;
using Aiursoft.Warpgate.SDK;
using Microsoft.AspNetCore.Identity;

namespace Aiursoft.Warp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextForInfraApps<WarpDbContext>(Configuration.GetConnectionString("DatabaseConnection"));

            services.AddIdentity<WarpUser, IdentityRole>()
                .AddEntityFrameworkStores<WarpDbContext>()
                .AddDefaultTokenProviders();

            services.AddAiurMvc();
            services.AddAiursoftWarpgate(Configuration.GetSection("AiursoftWarpgate"));
            services.AddAiursoftIdentity<WarpUser>(
                probeConfig: Configuration.GetSection("AiursoftProbe"),
                authenticationConfig: Configuration.GetSection("AiursoftAuthentication"),
                observerConfig: Configuration.GetSection("AiursoftObserver"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAiursoftHandler(env.IsDevelopment());
            app.UseAiursoftAppRouters();
        }
    }
}
