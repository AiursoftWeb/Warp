using System.Net;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Aiursoft.Warp.Entities;

namespace Aiursoft.Warp.Tests.IntegrationTests;

#pragma warning disable CS8602

[TestClass]
public class LinkTests : FunctionalTestBase
{
    private async Task<string> CreateLinkAsync(string targetUrl, string? password = null, DateTime? expireAt = null, int? maxClicks = null)
    {
        var token = await GetAntiCsrfToken("/Home/Index");
        var content = new Dictionary<string, string>
        {
            { "TargetUrl", targetUrl },
            { "__RequestVerificationToken", token }
        };

        if (!string.IsNullOrEmpty(password))
        {
            content.Add("Password", password);
            content.Add("IsPrivate", "true");
        }

        if (expireAt.HasValue)
        {
            content.Add("ExpireAt", expireAt.Value.ToString("O"));
        }

        if (maxClicks.HasValue)
        {
            content.Add("MaxClicks", maxClicks.Value.ToString());
        }

        var response = await Http.PostAsync("/Home/Index", new FormUrlEncodedContent(content));
        response.EnsureSuccessStatusCode();

        var html = await response.Content.ReadAsStringAsync();
        var match = Regex.Match(html, @"value=""([^""]+)"" id=""short-link-output""");
        Assert.IsTrue(match.Success, "Could not find created link in response HTML.");

        var fullUrl = match.Groups[1].Value;
        var uri = new Uri(fullUrl);
        var code = uri.Segments.Last();

        return code;
    }

    [TestMethod]
    public async Task CreateAndVisitLinkTest()
    {
        await RegisterAndLoginAsync();
        var targetUrl = "https://www.google.com";
        var code = await CreateLinkAsync(targetUrl);

        // Visit the link
        var response = await Http.GetAsync($"/r/{code}");
        Assert.AreEqual(HttpStatusCode.Found, response.StatusCode);
        Assert.AreEqual(targetUrl, response.Headers.Location?.OriginalString);
    }

    [TestMethod]
    public async Task PrivateLinkTest()
    {
        await RegisterAndLoginAsync();
        var targetUrl = "https://www.google.com";
        var password = "link-password";
        var code = await CreateLinkAsync(targetUrl, password: password);

        // Visit the link - should be redirected to password page or show password form (it returns 200 OK with view)
        var response = await Http.GetAsync($"/r/{code}");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        Assert.IsTrue(html.Contains("password") || html.Contains("Password")); // Simple check

        // Try wrong password
        var token = await GetAntiCsrfToken($"/r/{code}");
        var wrongContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "Password", "wrong" },
            { "__RequestVerificationToken", token }
        });
        var wrongResponse = await Http.PostAsync($"/r/{code}", wrongContent);
        Assert.AreEqual(HttpStatusCode.OK, wrongResponse.StatusCode);
        var wrongHtml = await wrongResponse.Content.ReadAsStringAsync();
        Assert.Contains("Incorrect password", wrongHtml);

        // Try correct password
        var correctContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "Password", password },
            { "__RequestVerificationToken", token }
        });
        var correctResponse = await Http.PostAsync($"/r/{code}", correctContent);
        Assert.AreEqual(HttpStatusCode.Found, correctResponse.StatusCode);
        Assert.AreEqual(targetUrl, correctResponse.Headers.Location?.OriginalString);
    }

    [TestMethod]
    public async Task ExpiredLinkTest()
    {
        await RegisterAndLoginAsync();
        var targetUrl = "https://www.google.com";
        var code = await CreateLinkAsync(targetUrl);

        using (var scope = Server!.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<WarpDbContext>();
            var link = await db.ShorterLinks.FirstAsync(l => l.RedirectTo == code);
            link.ExpireAt = DateTime.UtcNow.AddDays(-1);
            await db.SaveChangesAsync();
        }

        var response = await Http.GetAsync($"/r/{code}");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("Expired", html);
    }

    [TestMethod]
    public async Task EditLinkTest()
    {
        var (email, _) = await RegisterAndLoginAsync();
        var targetUrl = "https://www.google.com";
        var code = await CreateLinkAsync(targetUrl);

        using var scope = Server!.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WarpDbContext>();
        var link = await db.ShorterLinks.FirstAsync(l => l.RedirectTo == code);
        var linkId = link.Id;

        var user = await db.Users.FirstAsync(u => u.Email == email);
        Assert.AreEqual(user.Id, link.UserId, "Link UserID does not match logged in UserID");

        // Get Edit Page
        var editResponse = await Http.GetAsync($"/Links/Edit?id={linkId}");
        editResponse.EnsureSuccessStatusCode();

        // Post Edit
        var newTarget = "https://www.bing.com";
        var token = await GetAntiCsrfToken($"/Links/Edit?id={linkId}");
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "Id", linkId.ToString() },
            { "TargetUrl", newTarget },
            { "Title", "New Title" },
            { "CustomCode", code }, // Keep same code
            { "IsPrivate", "false" },
            { "__RequestVerificationToken", token }
        });

        var postResponse = await Http.PostAsync("/Links/Edit", content);
        Assert.AreEqual(HttpStatusCode.Found, postResponse.StatusCode);

        // Verify change
        var updatedLink = await db.ShorterLinks.AsNoTracking().FirstAsync(l => l.Id == linkId);
        Assert.AreEqual(newTarget, updatedLink.TargetUrl);
    }

    [TestMethod]
    public async Task AdminViewAllLinksTest()
    {
        // Create two users and links
        await RegisterAndLoginAsync(); // User 1
        await CreateLinkAsync("https://user1.com");

        // Log off
        var logOffToken = await GetAntiCsrfToken("/Manage/ChangePassword");
        await Http.PostAsync("/Account/LogOff", new FormUrlEncodedContent(new Dictionary<string, string> { { "__RequestVerificationToken", logOffToken } }));

        await RegisterAndLoginAsync(); // User 2
        await CreateLinkAsync("https://user2.com");

        // Log off
        logOffToken = await GetAntiCsrfToken("/Manage/ChangePassword");
        await Http.PostAsync("/Account/LogOff", new FormUrlEncodedContent(new Dictionary<string, string> { { "__RequestVerificationToken", logOffToken } }));

        // Login as Admin
        var loginToken = await GetAntiCsrfToken("/Account/Login");
        var loginContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "EmailOrUserName", "admin" },
            { "Password", "admin123" },
            { "__RequestVerificationToken", loginToken }
        });
        var loginResponse = await Http.PostAsync("/Account/Login", loginContent);
        Assert.AreEqual(HttpStatusCode.Found, loginResponse.StatusCode);

        // View All Links
        var response = await Http.GetAsync("/Admin/AllLinks");
        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();

        Assert.Contains("user1.com", html);
        Assert.Contains("user2.com", html);
    }

    [TestMethod]
    public async Task AdminDeleteLinkTest()
    {
        await RegisterAndLoginAsync();
        var code = await CreateLinkAsync("https://todelete.com");

        using (var scope = Server!.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<WarpDbContext>();
            var link = await db.ShorterLinks.FirstAsync(l => l.RedirectTo == code);
            var linkId = link.Id;

            // Log off
            var logOffToken = await GetAntiCsrfToken("/Manage/ChangePassword");
            await Http.PostAsync("/Account/LogOff", new FormUrlEncodedContent(new Dictionary<string, string> { { "__RequestVerificationToken", logOffToken } }));

            // Login as Admin
            var loginToken = await GetAntiCsrfToken("/Account/Login");
            var loginContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "EmailOrUserName", "admin" },
                { "Password", "admin123" },
                { "__RequestVerificationToken", loginToken }
            });
            await Http.PostAsync("/Account/Login", loginContent);

            // Delete Link
            var deleteToken = await GetAntiCsrfToken($"/Admin/DeleteLink/{linkId}");
            var deleteContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "__RequestVerificationToken", deleteToken }
            });
            var deleteResponse = await Http.PostAsync($"/Admin/DeleteLink/{linkId}", deleteContent);
            Assert.AreEqual(HttpStatusCode.Found, deleteResponse.StatusCode);

            // Verify
            var deletedLink = await db.ShorterLinks.FirstOrDefaultAsync(l => l.Id == linkId);
            Assert.IsNull(deletedLink);
        }
    }

    [TestMethod]
    public async Task AdminEditLinkOwnerTest()
    {
        // User 1 creates link
        var (__, _) = await RegisterAndLoginAsync();
        var code = await CreateLinkAsync("https://changeowner.com");

        // Log off
        var logOffToken = await GetAntiCsrfToken("/Manage/ChangePassword");
        await Http.PostAsync("/Account/LogOff", new FormUrlEncodedContent(new Dictionary<string, string> { { "__RequestVerificationToken", logOffToken } }));

        // User 2 exists (register them)
        var (user2Email, _) = await RegisterAndLoginAsync();

        // Log off
        logOffToken = await GetAntiCsrfToken("/Manage/ChangePassword");
        await Http.PostAsync("/Account/LogOff", new FormUrlEncodedContent(new Dictionary<string, string> { { "__RequestVerificationToken", logOffToken } }));

        // Login as Admin
        var loginToken = await GetAntiCsrfToken("/Account/Login");
        var loginContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "EmailOrUserName", "admin" },
            { "Password", "admin123" },
            { "__RequestVerificationToken", loginToken }
        });
        await Http.PostAsync("/Account/Login", loginContent);

        using var scope = Server!.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WarpDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<User>>();

        var link = await db.ShorterLinks.FirstAsync(l => l.RedirectTo == code);
        var user2 = await userManager.FindByEmailAsync(user2Email);
        Assert.IsNotNull(user2);

        // Admin changes owner to User 2
        var editToken = await GetAntiCsrfToken($"/Admin/EditLink/{link.Id}");
        var editContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "LinkId", link.Id.ToString() },
            { "SelectedUserId", user2.Id },
            { "TargetUrl", link.TargetUrl },
            { "Title", "Admin Edited" },
            { "CustomCode", code }, // Keep same code
            { "IsPrivate", "false" },
            { "__RequestVerificationToken", editToken }
        });

        var editResponse = await Http.PostAsync("/Admin/EditLink", editContent);
        Assert.AreEqual(HttpStatusCode.Found, editResponse.StatusCode);

        // Verify owner changed
        var updatedLink = await db.ShorterLinks.AsNoTracking().FirstAsync(l => l.Id == link.Id);
        Assert.AreEqual(user2.Id, updatedLink.UserId);
    }
    [TestMethod]
    public async Task CreateLoopLinkTest()
    {
        await RegisterAndLoginAsync();
        var customCode = "loop-test";
        var targetUrl = $"http://localhost:{Port}/r/{customCode}";

        var token = await GetAntiCsrfToken("/Home/Index");
        var content = new Dictionary<string, string>
        {
            { "TargetUrl", targetUrl },
            { "CustomCode", customCode },
            { "__RequestVerificationToken", token }
        };

        var response = await Http.PostAsync("/Home/Index", new FormUrlEncodedContent(content));
        var html = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("The target URL cannot be the same as the shortcut URL", html);
    }

    [TestMethod]
    public async Task EditLoopLinkTest()
    {
        await RegisterAndLoginAsync();
        var targetUrl = "https://www.google.com";
        var code = await CreateLinkAsync(targetUrl);

        using var scope = Server!.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WarpDbContext>();
        var link = await db.ShorterLinks.FirstAsync(l => l.RedirectTo == code);

        var loopUrl = $"http://localhost:{Port}/r/{code}";

        var token = await GetAntiCsrfToken($"/Links/Edit?id={link.Id}");
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "Id", link.Id.ToString() },
            { "TargetUrl", loopUrl },
            { "Title", "Loop Title" },
            { "CustomCode", code },
            { "IsPrivate", "false" },
            { "__RequestVerificationToken", token }
        });

        var response = await Http.PostAsync("/Links/Edit", content);
        var html = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("The target URL cannot be the same as the shortcut URL", html);
    }

    [TestMethod]
    public async Task HitRecordingTest()
    {
        await RegisterAndLoginAsync();
        var targetUrl = "https://www.google.com";
        var code = await CreateLinkAsync(targetUrl);

        // Visit the link
        await Http.GetAsync($"/r/{code}");

        using var scope = Server!.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WarpDbContext>();
        var link = await db.ShorterLinks.FirstAsync(l => l.RedirectTo == code);
        var hits = await db.WarpHits.Where(h => h.LinkId == link.Id).ToListAsync();

        Assert.HasCount(1, hits);
        Assert.IsNotNull(hits[0].IP);
        Assert.IsNotNull(hits[0].Device);
    }
}

#pragma warning restore CS8602
