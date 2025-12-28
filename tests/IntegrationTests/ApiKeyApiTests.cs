using System.Net;
using Aiursoft.Warp.Entities;
using Aiursoft.Warp.Models.ApiModels;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Warp.Tests.IntegrationTests;

[TestClass]
public class ApiKeyApiTests : FunctionalTestBase
{
    private async Task<string> CreateApiKeyAsync(string name)
    {
        var token = await GetAntiCsrfToken("/ApiKey/Create");
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "Name", name },
            { "__RequestVerificationToken", token }
        });

        var response = await Http.PostAsync("/ApiKey/Create", content);
        Assert.AreEqual(HttpStatusCode.Found, response.StatusCode);

        using var scope = Server!.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WarpDbContext>();
        var key = await db.WarpApiKeys.OrderByDescending(k => k.CreationTime).FirstAsync();
        return key.ApiKey;
    }

    [TestMethod]
    public async Task FullApiWorkflowTest()
    {
        await RegisterAndLoginAsync();
        var apiKey = await CreateApiKeyAsync("Test API Key");

        // 1. List links (should be empty)
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/links");
        request.Headers.Add("X-API-Key", apiKey);
        var response = await Http.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var links = await response.Content.ReadFromJsonAsync<List<ShorterLink>>();
        Assert.IsNotNull(links);
        Assert.IsEmpty(links);

        // 2. Create link
        var createModel = new CreateLinkApiModel
        {
            Title = "API Link",
            TargetUrl = "https://www.bing.com",
            CustomCode = "api-link"
        };
        request = new HttpRequestMessage(HttpMethod.Post, "/api/links");
        request.Headers.Add("X-API-Key", apiKey);
        request.Content = JsonContent.Create(createModel);
        response = await Http.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        var createdLink = await response.Content.ReadFromJsonAsync<ShorterLink>();
        Assert.IsNotNull(createdLink);
        Assert.AreEqual("api-link", createdLink.RedirectTo);

        // 3. Get link
        request = new HttpRequestMessage(HttpMethod.Get, $"/api/links/{createdLink.Id}");
        request.Headers.Add("X-API-Key", apiKey);
        response = await Http.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var fetchedLink = await response.Content.ReadFromJsonAsync<ShorterLink>();
        Assert.IsNotNull(fetchedLink);
        Assert.AreEqual(createdLink.Id, fetchedLink.Id);

        // 4. Update link
        var updateModel = new UpdateLinkApiModel
        {
            Title = "Updated API Link",
            TargetUrl = "https://www.google.com"
        };
        request = new HttpRequestMessage(HttpMethod.Patch, $"/api/links/{createdLink.Id}");
        request.Headers.Add("X-API-Key", apiKey);
        request.Content = JsonContent.Create(updateModel);
        response = await Http.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var updatedLink = await response.Content.ReadFromJsonAsync<ShorterLink>();
        Assert.IsNotNull(updatedLink);
        Assert.AreEqual("Updated API Link", updatedLink.Title);
        Assert.AreEqual("https://www.google.com", updatedLink.TargetUrl);

        // 5. Delete link
        request = new HttpRequestMessage(HttpMethod.Delete, $"/api/links/{createdLink.Id}");
        request.Headers.Add("X-API-Key", apiKey);
        response = await Http.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);

        // 6. Verify deleted
        request = new HttpRequestMessage(HttpMethod.Get, $"/api/links/{createdLink.Id}");
        request.Headers.Add("X-API-Key", apiKey);
        response = await Http.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task UnauthorizedApiTest()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/links");
        request.Headers.Add("X-API-Key", "invalid-key");
        var response = await Http.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task ExpiredApiKeyTest()
    {
        await RegisterAndLoginAsync();
        var apiKeyStr = await CreateApiKeyAsync("To Expire");

        using (var scope = Server!.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<WarpDbContext>();
            var key = await db.WarpApiKeys.FirstAsync(k => k.ApiKey == apiKeyStr);
            key.ExpireAt = DateTime.UtcNow.AddDays(-1);
            await db.SaveChangesAsync();
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/links");
        request.Headers.Add("X-API-Key", apiKeyStr);
        var response = await Http.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
