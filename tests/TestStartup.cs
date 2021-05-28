using Microsoft.Extensions.Configuration;

namespace Aiursoft.Warp.Tests
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration) { }
    }
}
