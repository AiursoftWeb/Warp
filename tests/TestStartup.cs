using Microsoft.Extensions.Configuration;

namespace Aiursoft.Wrap.Tests
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration) { }
    }
}
