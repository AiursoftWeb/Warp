using Aiursoft.Wrap;
using Microsoft.Extensions.Configuration;

namespace Aiursoft.Wrapgate.Tests
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration) { }
    }
}
