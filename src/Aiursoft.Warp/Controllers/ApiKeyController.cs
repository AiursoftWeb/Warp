using System.Security.Claims;
using Aiursoft.UiStack.Navigation;
using Aiursoft.Warp.Authorization;
using Aiursoft.Warp.Configuration;
using Aiursoft.Warp.Entities;
using Aiursoft.Warp.Models.ApiKeyViewModels;
using Aiursoft.Warp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Aiursoft.Warp.Controllers;

[Authorize]
public class ApiKeyController(
    WarpDbContext dbContext,
    IOptions<AppSettings> appSettings) : Controller
{
    [HttpGet]
    [RenderInNavBar(
        NavGroupName = "Settings",
        NavGroupOrder = 9998,
        CascadedLinksGroupName = "Personal",
        CascadedLinksIcon = "user-circle",
        CascadedLinksOrder = 1,
        LinkText = "API Keys",
        LinkOrder = 4)]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var keys = await dbContext.WarpApiKeys
            .Where(k => k.UserId == userId)
            .OrderByDescending(k => k.CreationTime)
            .ToListAsync();

        var model = new IndexViewModel
        {
            ApiKeys = keys
        };
        return this.StackView(model);
    }

    [HttpGet]
    [Authorize(Policy = AppPermissionNames.CanViewApiKey)]
    [RenderInNavBar(
        NavGroupName = "Administration",
        NavGroupOrder = 9999,
        CascadedLinksGroupName = "System",
        CascadedLinksIcon = "settings",
        CascadedLinksOrder = 9999,
        LinkText = "Global API Key",
        LinkOrder = 2)]
    public IActionResult Global()
    {
        var apiKey = appSettings.Value.ApiKey;
        if (string.IsNullOrEmpty(apiKey))
        {
            return NotFound("Global API key is not configured in appsettings.json.");
        }

        var model = new GlobalApiKeyViewModel
        {
            ApiKey = apiKey,
            ApiUrl = $"{Request.Scheme}://{Request.Host}/api/links"
        };
        return this.StackView(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return this.StackView(new CreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return this.StackView(model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var apiKey = new WarpApiKey
        {
            Name = model.Name,
            ApiKey = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"),
            ExpireAt = model.ExpireAt,
            UserId = userId!
        };

        dbContext.WarpApiKeys.Add(apiKey);
        await dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var key = await dbContext.WarpApiKeys.FirstOrDefaultAsync(k => k.Id == id && k.UserId == userId);
        if (key != null)
        {
            dbContext.WarpApiKeys.Remove(key);
            await dbContext.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
