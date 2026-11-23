using System.Net;
using Aiursoft.Warp.Configuration;
using Aiursoft.Warp.Models.ApiModels;
using Aiursoft.Warp.Views.Shared.Components.MarketingFooter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Aiursoft.Warp.Tests.IntegrationTests;

[TestClass]
public class MoreWarpViewsTests : FunctionalTestBase
{
    [TestMethod]
    public async Task SelfHost_Index_ReturnsSuccess()
    {
        var response = await Http.GetAsync("/SelfHost");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task Error_Codes_ReturnsSuccess()
    {
        foreach (var code in new[] { 400, 401, 403, 404, 500, 502 })
        {
            var resp = await Http.GetAsync($"/Error/Code{code}?returnUrl=%2F");
            Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
        }
    }

    [TestMethod]
    public async Task MarketingFooter_Component_InvokesSuccessfully()
    {
        using var scope = Server!.Services.CreateScope();
        var component = ActivatorUtilities.CreateInstance<MarketingFooter>(scope.ServiceProvider);
        var result = await component.InvokeAsync();
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task Links_FullLifecycle_ReturnsSuccess()
    {
        await RegisterAndLoginAsync();

        // Invalid Home POST (triggers !ModelState.IsValid and re-renders view)
        var homeToken = await GetAntiCsrfToken("/Home/Index");
        var invalidHomeResp = await Http.PostAsync("/Home/Index", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "TargetUrl", "" },
            { "__RequestVerificationToken", homeToken }
        }));
        Assert.AreEqual(HttpStatusCode.OK, invalidHomeResp.StatusCode);

        // 1. Create a link
        var token = await GetAntiCsrfToken("/Home/Index");
        var postResp = await Http.PostAsync("/Home/Index", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "TargetUrl", "https://example.com/test-lifecycle" },
            { "__RequestVerificationToken", token }
        }));
        postResp.EnsureSuccessStatusCode();

        Guid linkId;
        using (var scope = Server!.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<WarpDbContext>();
            var link = await db.ShorterLinks.FirstAsync(l => l.TargetUrl == "https://example.com/test-lifecycle");
            linkId = link.Id;
        }

        // 2. Visit /Links (Index) with rows rendered
        var indexResp = await Http.GetAsync("/Links");
        Assert.AreEqual(HttpStatusCode.OK, indexResp.StatusCode);

        // 3. Visit /Links/Edit
        var editGet = await Http.GetAsync($"/Links/Edit?id={linkId}");
        Assert.AreEqual(HttpStatusCode.OK, editGet.StatusCode);

        // 4. Post invalid /Links/Edit (triggers !ModelState.IsValid and re-renders view)
        var editToken = await GetAntiCsrfToken($"/Links/Edit?id={linkId}");
        var invalidEditResp = await Http.PostAsync("/Links/Edit", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "Id", linkId.ToString() },
            { "TargetUrl", "" }, // empty is invalid
            { "__RequestVerificationToken", editToken }
        }));
        Assert.AreEqual(HttpStatusCode.OK, invalidEditResp.StatusCode);

        // 5. Post valid /Links/Edit
        var validEditResp = await Http.PostAsync("/Links/Edit", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "Id", linkId.ToString() },
            { "TargetUrl", "https://example.com/user-updated-link" },
            { "Title", "User Title" },
            { "CustomCode", "user-updated-code" },
            { "__RequestVerificationToken", editToken }
        }));
        Assert.AreEqual(HttpStatusCode.Found, validEditResp.StatusCode);

        // 6. Visit /Links/Delete
        var deleteGet = await Http.GetAsync($"/Links/Delete?id={linkId}");
        Assert.AreEqual(HttpStatusCode.OK, deleteGet.StatusCode);

        // 7. Post /Links/Delete
        var deleteToken = await GetAntiCsrfToken($"/Links/Delete?id={linkId}");
        var deletePost = await Http.PostAsync($"/Links/Delete?id={linkId}", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "__RequestVerificationToken", deleteToken }
        }));
        Assert.AreEqual(HttpStatusCode.Found, deletePost.StatusCode);

        // 8. 404 tests for LinksController
        var notFoundLinkEdit = await Http.GetAsync($"/Links/Edit?id={Guid.NewGuid()}");
        Assert.AreEqual(HttpStatusCode.NotFound, notFoundLinkEdit.StatusCode);

        var notFoundLinkDelete = await Http.GetAsync($"/Links/Delete?id={Guid.NewGuid()}");
        Assert.AreEqual(HttpStatusCode.NotFound, notFoundLinkDelete.StatusCode);
    }

    [TestMethod]
    public async Task Admin_ViewsAndJobs_ReturnsSuccess()
    {
        var (email, _) = await RegisterAndLoginAsync();

        // Create a link
        var token = await GetAntiCsrfToken("/Home/Index");
        var postResp = await Http.PostAsync("/Home/Index", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "TargetUrl", "https://example.com/admin-test-link" },
            { "__RequestVerificationToken", token }
        }));
        postResp.EnsureSuccessStatusCode();

        string userId;
        Guid linkId;
        using (var scope = Server!.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<WarpDbContext>();
            var user = await db.Users.FirstAsync(u => u.Email == email);
            userId = user.Id;
            var link = await db.ShorterLinks.FirstAsync(l => l.TargetUrl == "https://example.com/admin-test-link");
            linkId = link.Id;
        }

        // Login as admin
        var loginToken = await GetAntiCsrfToken("/Account/Login");
        var loginResp = await Http.PostAsync("/Account/Login", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "EmailOrUserName", "admin" },
            { "Password", "Admin@123456!" },
            { "__RequestVerificationToken", loginToken }
        }));
        Assert.AreEqual(HttpStatusCode.Found, loginResp.StatusCode);

        // Visit Admin UserLinks
        var userLinksResp = await Http.GetAsync($"/Admin/UserLinks/{userId}");
        Assert.AreEqual(HttpStatusCode.OK, userLinksResp.StatusCode);

        // Visit Admin AllLinks
        var allLinksResp = await Http.GetAsync("/Admin/AllLinks");
        Assert.AreEqual(HttpStatusCode.OK, allLinksResp.StatusCode);

        // Visit Admin EditLink with saved=true
        var editLinkResp = await Http.GetAsync($"/Admin/EditLink/{linkId}?saved=true");
        Assert.AreEqual(HttpStatusCode.OK, editLinkResp.StatusCode);

        // Post invalid Admin EditLink (triggers !ModelState.IsValid and re-renders view)
        var adminEditToken = await GetAntiCsrfToken($"/Admin/EditLink/{linkId}");
        var invalidAdminEditResp = await Http.PostAsync("/Admin/EditLink", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "LinkId", linkId.ToString() },
            { "TargetUrl", "" }, // invalid
            { "__RequestVerificationToken", adminEditToken }
        }));
        Assert.AreEqual(HttpStatusCode.OK, invalidAdminEditResp.StatusCode);

        // Post valid Admin EditLink
        var validAdminEditResp = await Http.PostAsync("/Admin/EditLink", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "LinkId", linkId.ToString() },
            { "SelectedUserId", userId },
            { "TargetUrl", "https://example.com/admin-updated-link" },
            { "Title", "Admin Title" },
            { "CustomCode", "admin-custom-code" },
            { "__RequestVerificationToken", adminEditToken }
        }));
        Assert.AreEqual(HttpStatusCode.Found, validAdminEditResp.StatusCode);

        // Visit Admin DeleteLink
        var deleteLinkResp = await Http.GetAsync($"/Admin/DeleteLink/{linkId}");
        Assert.AreEqual(HttpStatusCode.OK, deleteLinkResp.StatusCode);

        // Visit Jobs
        var jobsResp = await Http.GetAsync("/Jobs");
        Assert.AreEqual(HttpStatusCode.OK, jobsResp.StatusCode);

        // Visit GlobalSettings
        var settingsResp = await Http.GetAsync("/GlobalSettings");
        Assert.AreEqual(HttpStatusCode.OK, settingsResp.StatusCode);

        // Post GlobalSettings Edit
        var settingsToken = await GetAntiCsrfToken("/GlobalSettings");
        var settingsEditResp = await Http.PostAsync("/GlobalSettings/Edit", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "Key", SettingsMap.ProjectName },
            { "Value", "Aiursoft Warp Updated" },
            { "__RequestVerificationToken", settingsToken }
        }));
        Assert.AreEqual(HttpStatusCode.Found, settingsEditResp.StatusCode);

        // 404 tests for AdminController
        var notFoundUser = await Http.GetAsync($"/Admin/UserLinks/{Guid.NewGuid()}");
        Assert.AreEqual(HttpStatusCode.NotFound, notFoundUser.StatusCode);

        var notFoundEdit = await Http.GetAsync($"/Admin/EditLink/{Guid.NewGuid()}");
        Assert.AreEqual(HttpStatusCode.NotFound, notFoundEdit.StatusCode);

        var notFoundDelete = await Http.GetAsync($"/Admin/DeleteLink/{Guid.NewGuid()}");
        Assert.AreEqual(HttpStatusCode.NotFound, notFoundDelete.StatusCode);
    }

    [TestMethod]
    public async Task ApiKey_FullFlow_ReturnsSuccess()
    {
        await RegisterAndLoginAsync();

        // Visit Create
        var createGetResp = await Http.GetAsync("/ApiKey/Create");
        Assert.AreEqual(HttpStatusCode.OK, createGetResp.StatusCode);

        // Post Create
        var createToken = await GetAntiCsrfToken("/ApiKey/Create");
        var createPostResp = await Http.PostAsync("/ApiKey/Create", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "Name", "Flow API Key" },
            { "__RequestVerificationToken", createToken }
        }));
        Assert.AreEqual(HttpStatusCode.Found, createPostResp.StatusCode);

        Guid keyId;
        string apiKey;
        using (var scope = Server!.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<WarpDbContext>();
            var key = await db.WarpApiKeys.FirstAsync(k => k.Name == "Flow API Key");
            keyId = key.Id;
            apiKey = key.ApiKey;
        }

        // Visit Index (now renders rows)
        var indexResp = await Http.GetAsync("/ApiKey");
        Assert.AreEqual(HttpStatusCode.OK, indexResp.StatusCode);

        // Test ApiController conflict & private update
        var createModel = new CreateLinkApiModel
        {
            Title = "API Conflict Link",
            TargetUrl = "https://www.example.com",
            CustomCode = "conflict-code"
        };
        var req = new HttpRequestMessage(HttpMethod.Post, "/api/links");
        req.Headers.Add("X-API-Key", apiKey);
        req.Content = JsonContent.Create(createModel);
        var res = await Http.SendAsync(req);
        Assert.AreEqual(HttpStatusCode.Created, res.StatusCode);

        var link = await res.Content.ReadFromJsonAsync<ShorterLink>();
        Assert.IsNotNull(link);

        // Conflict check
        var reqConflict = new HttpRequestMessage(HttpMethod.Post, "/api/links");
        reqConflict.Headers.Add("X-API-Key", apiKey);
        reqConflict.Content = JsonContent.Create(createModel);
        var resConflict = await Http.SendAsync(reqConflict);
        Assert.AreEqual(HttpStatusCode.Conflict, resConflict.StatusCode);

        // Private update check
        var updateModel = new UpdateLinkApiModel
        {
            IsPrivate = true,
            Password = "mypassword"
        };
        var reqUpdate = new HttpRequestMessage(HttpMethod.Patch, $"/api/links/{link.Id}");
        reqUpdate.Headers.Add("X-API-Key", apiKey);
        reqUpdate.Content = JsonContent.Create(updateModel);
        var resUpdate = await Http.SendAsync(reqUpdate);
        Assert.AreEqual(HttpStatusCode.OK, resUpdate.StatusCode);

        // CreateLink with empty CustomCode and private password
        var privateModel = new CreateLinkApiModel
        {
            Title = "Private API Link",
            TargetUrl = "https://example.com/private",
            IsPrivate = true,
            Password = "private-pass",
            ExpireAt = DateTime.UtcNow.AddDays(7),
            MaxClicks = 100
        };
        var reqPrivate = new HttpRequestMessage(HttpMethod.Post, "/api/links");
        reqPrivate.Headers.Add("X-API-Key", apiKey);
        reqPrivate.Content = JsonContent.Create(privateModel);
        var resPrivate = await Http.SendAsync(reqPrivate);
        Assert.AreEqual(HttpStatusCode.Created, resPrivate.StatusCode);
        var privateLink = await resPrivate.Content.ReadFromJsonAsync<ShorterLink>();
        Assert.IsNotNull(privateLink);

        // UpdateLink: clear private password and update maxClicks
        var unprivateModel = new UpdateLinkApiModel
        {
            IsPrivate = false,
            MaxClicks = 200,
            TargetUrl = "https://example.com/updated-private",
            ExpireAt = DateTime.UtcNow.AddDays(14)
        };
        var reqUnprivate = new HttpRequestMessage(HttpMethod.Patch, $"/api/links/{privateLink.Id}");
        reqUnprivate.Headers.Add("X-API-Key", apiKey);
        reqUnprivate.Content = JsonContent.Create(unprivateModel);
        var resUnprivate = await Http.SendAsync(reqUnprivate);
        Assert.AreEqual(HttpStatusCode.OK, resUnprivate.StatusCode);

        // UpdateLink: custom code conflict
        var reqCodeConflict = new HttpRequestMessage(HttpMethod.Patch, $"/api/links/{privateLink.Id}");
        reqCodeConflict.Headers.Add("X-API-Key", apiKey);
        reqCodeConflict.Content = JsonContent.Create(new UpdateLinkApiModel
        {
            CustomCode = "conflict-code"
        });
        var resCodeConflict = await Http.SendAsync(reqCodeConflict);
        Assert.AreEqual(HttpStatusCode.Conflict, resCodeConflict.StatusCode);

        // 404 checks for ApiController
        var reqNotFoundGet = new HttpRequestMessage(HttpMethod.Get, $"/api/links/{Guid.NewGuid()}");
        reqNotFoundGet.Headers.Add("X-API-Key", apiKey);
        var resNotFoundGet = await Http.SendAsync(reqNotFoundGet);
        Assert.AreEqual(HttpStatusCode.NotFound, resNotFoundGet.StatusCode);

        var reqNotFoundPatch = new HttpRequestMessage(HttpMethod.Patch, $"/api/links/{Guid.NewGuid()}");
        reqNotFoundPatch.Headers.Add("X-API-Key", apiKey);
        reqNotFoundPatch.Content = JsonContent.Create(new UpdateLinkApiModel { Title = "X" });
        var resNotFoundPatch = await Http.SendAsync(reqNotFoundPatch);
        Assert.AreEqual(HttpStatusCode.NotFound, resNotFoundPatch.StatusCode);

        var reqNotFoundDelete = new HttpRequestMessage(HttpMethod.Delete, $"/api/links/{Guid.NewGuid()}");
        reqNotFoundDelete.Headers.Add("X-API-Key", apiKey);
        var resNotFoundDelete = await Http.SendAsync(reqNotFoundDelete);
        Assert.AreEqual(HttpStatusCode.NoContent, resNotFoundDelete.StatusCode);

        // Post Delete API Key now that API calls are done
        var deleteToken = await GetAntiCsrfToken("/ApiKey");
        var deletePostResp = await Http.PostAsync($"/ApiKey/Delete/{keyId}", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "__RequestVerificationToken", deleteToken }
        }));
        Assert.AreEqual(HttpStatusCode.Found, deletePostResp.StatusCode);

        // Login as admin for Global
        var loginToken = await GetAntiCsrfToken("/Account/Login");
        var loginResp = await Http.PostAsync("/Account/Login", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "EmailOrUserName", "admin" },
            { "Password", "Admin@123456!" },
            { "__RequestVerificationToken", loginToken }
        }));
        Assert.AreEqual(HttpStatusCode.Found, loginResp.StatusCode);

        using (var scope = Server!.Services.CreateScope())
        {
            var options = scope.ServiceProvider.GetRequiredService<IOptions<AppSettings>>();
            typeof(AppSettings).GetProperty(nameof(AppSettings.ApiKey))?.SetValue(options.Value, "test-global-key");
        }

        var globalResp = await Http.GetAsync("/ApiKey/Global");
        Assert.AreEqual(HttpStatusCode.OK, globalResp.StatusCode);

        // Global API key calls on ApiController
        var globalListReq = new HttpRequestMessage(HttpMethod.Get, "/api/links");
        globalListReq.Headers.Add("X-API-Key", "test-global-key");
        var globalListResp = await Http.SendAsync(globalListReq);
        Assert.AreEqual(HttpStatusCode.OK, globalListResp.StatusCode);

        var globalCreateReq = new HttpRequestMessage(HttpMethod.Post, "/api/links");
        globalCreateReq.Headers.Add("X-API-Key", "test-global-key");
        globalCreateReq.Content = JsonContent.Create(new CreateLinkApiModel
        {
            Title = "Global API Link",
            TargetUrl = "https://example.com/global",
            CustomCode = "global-link-" + Guid.NewGuid().ToString().Substring(0, 5)
        });
        var globalCreateResp = await Http.SendAsync(globalCreateReq);
        Assert.AreEqual(HttpStatusCode.Created, globalCreateResp.StatusCode);
    }
}
