using System.Net;
using System.Text.RegularExpressions;

namespace Aiursoft.Warp.Tests.IntegrationTests;

public abstract class FunctionalTestBase
{
    protected int Port => Http.BaseAddress!.Port;
    protected HttpClient Http;
    protected IHost? Server;

    protected FunctionalTestBase()
    {
        var cookieContainer = new CookieContainer();
        var handler = new HttpClientHandler
        {
            CookieContainer = cookieContainer,
            AllowAutoRedirect = false
        };
        Http = new HttpClient(handler);
    }

    [TestInitialize]
    public async Task CreateServer()
    {
        using var reservation = new TestPortReservation();
        Http.BaseAddress = new Uri($"http://localhost:{reservation.Port}");
        Server = await AppAsync<Startup>([], port: reservation.Port);
        try
        {
            reservation.ConfigureServer(Server);
            await Server.UpdateDbAsync<WarpDbContext>();
            await Server.SeedAsync();
            await Server.StartAsync();
        }
        catch
        {
            Server.Dispose();
            Server = null;
            throw;
        }
    }

    [TestCleanup]
    public async Task CleanServer()
    {
        if (Server == null) return;
        await Server.StopAsync();
        Server.Dispose();
    }

    protected async Task<string> GetAntiCsrfToken(string url)
    {
        var response = await Http.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        var match = Regex.Match(html,
            @"<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)"" />");
        if (!match.Success)
        {
            throw new InvalidOperationException($"Could not find anti-CSRF token on page: {url}");
        }

        return match.Groups[1].Value;
    }

    protected async Task<(string email, string password)> RegisterAndLoginAsync()
    {
        var email = $"test-{Guid.NewGuid()}@aiursoft.com";
        var password = "Test-Password-123";

        var registerToken = await GetAntiCsrfToken("/Account/Register");
        var registerContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "Email", email },
            { "Password", password },
            { "ConfirmPassword", password },
            { "__RequestVerificationToken", registerToken }
        });
        var registerResponse = await Http.PostAsync("/Account/Register", registerContent);
        Assert.AreEqual(HttpStatusCode.Found, registerResponse.StatusCode);

        return (email, password);
    }
}
