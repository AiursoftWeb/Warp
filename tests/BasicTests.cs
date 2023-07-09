using Aiursoft.Scanner;
using Aiursoft.Warp.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Aiursoft.CSTools.Tools;
using Aiursoft.DbTools;
using static Aiursoft.WebTools.Extends;

namespace Aiursoft.Warp.Tests
{
    [TestClass]
    public class BasicTests
    {
        private readonly int _port;
        private readonly string _endpointUrl;
        private IHost _server;
        private HttpClient _http;

        public BasicTests()
        {
            _port = Network.GetAvailablePort();
            _endpointUrl = $"http://localhost:{_port}";
        }

        [TestInitialize]
        public async Task CreateServer()
        {
            _server = await App<TestStartup>(port: _port).UpdateDbAsync<WarpDbContext>(UpdateMode.RecreateThenUse);
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            _http = new HttpClient(handler);
            await _server.StartAsync();

            var services = new ServiceCollection();
            services.AddHttpClient();
            services.AddLibraryDependencies();
        }

        [TestMethod]
        public async Task RequestTest()
        {
            var response = await _http.GetAsync(_endpointUrl);

            Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
            Assert.AreEqual(
                "https://directory.aiursoft.com" + $"/oauth/authorize?force-confirm=&try-auth=True&appid=&redirect_uri=http%3A%2F%2Flocalhost%3A{_port}%2FAuth%2FAuthResult&state=%2FDashboard%2FIndex"
                , response.Headers.Location?.OriginalString);

        }
    }
}
