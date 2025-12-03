using Microsoft.AspNetCore.Authorization;
using Aiursoft.Warp.Models.HomeViewModels;
using Aiursoft.Warp.Services;
using Aiursoft.WebTools.Attributes;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.Warp.Entities;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Aiursoft.UiStack.Navigation;

namespace Aiursoft.Warp.Controllers;

[LimitPerMin]
// [Authorize]
public class HomeController(PasswordService passwordService, TemplateDbContext dbContext)
    : Controller
{
    [Authorize]
    [RenderInNavBar(
        NavGroupName = "Home",
        NavGroupOrder = 1,
        CascadedLinksGroupName = "Home",
        CascadedLinksIcon = "home",
        CascadedLinksOrder = 1,
        LinkText = "Link Shorter",
        LinkOrder = 1)]
    public IActionResult Index()
    {
        return this.StackView(new IndexViewModel());
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(IndexViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return this.StackView(model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var shorterLink = new ShorterLink
        {
            Id = model.LinkId,
            Title = model.Title,
            TargetUrl = model.TargetUrl,
            RedirectTo = model.CustomCode ?? Guid.NewGuid().ToString().Substring(0, 6), // Generate random code if not custom
            ExpireAt = model.ExpireAt,
            IsCustom = !string.IsNullOrEmpty(model.CustomCode),
            IsPrivate = model.IsPrivate,
            MaxClicks = model.MaxClicks,
            // CreationTime = DateTime.UtcNow,
            UserId = userId
        };

        if (model.IsPrivate && !string.IsNullOrEmpty(model.Password))
        {
            shorterLink.Password = passwordService.HashPassword(model.Password);
        }
        else
        {
            shorterLink.Password = null;
        }

        // Check if custom code already exists
        if (shorterLink.IsCustom && await dbContext.ShorterLinks.AnyAsync(l => l.RedirectTo == shorterLink.RedirectTo))
        {
            ModelState.AddModelError(nameof(model.CustomCode), "This custom code is already in use. Please choose another one.");
            return this.StackView(model);
        }

        dbContext.ShorterLinks.Add(shorterLink);
        await dbContext.SaveChangesAsync();

        var fullUrl = $"{Request.Scheme}://{Request.Host}/r/{shorterLink.RedirectTo}";
        model.CreatedShortLink = fullUrl;

        // Clear the model state to reset the form for the next creation.
        ModelState.Clear();
        var newModel = new IndexViewModel
        {
            CreatedShortLink = fullUrl
        };

        return this.StackView(newModel);
    }

    [HttpGet("r/{code}")]
    public async Task<IActionResult> Go(string code)
    {
        var link = await dbContext.ShorterLinks.FirstOrDefaultAsync(l => l.RedirectTo == code);
        if (link == null)
        {
            return NotFound();
        }

        if (link.ExpireAt.HasValue && link.ExpireAt < DateTime.UtcNow)
        {
            return this.SimpleView(new ExpiredViewModel(), "Expired");
        }

        if (link.MaxClicks.HasValue && link.MaxClicks <= link.Clicks)
        {
            return this.SimpleView(new ExpiredViewModel(), "Expired");
        }

        if (link.IsPrivate)
        {
            return this.SimpleView(new EnterPasswordViewModel { Code = code }, "EnterPassword");
        }

        link.Clicks++;
        await dbContext.SaveChangesAsync();
        return Redirect(link.TargetUrl);
    }

    [HttpPost("r/{code}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unlock(string code, EnterPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Code = code;
            return this.SimpleView(model,"EnterPassword");
        }

        var link = await dbContext.ShorterLinks.FirstOrDefaultAsync(l => l.RedirectTo == code);
        if (link == null)
        {
            return NotFound();
        }

        if (link.ExpireAt.HasValue && link.ExpireAt < DateTime.UtcNow)
        {
            return this.SimpleView(new ExpiredViewModel(), "Expired");
        }

        if (!link.IsPrivate)
        {
            return Redirect(link.TargetUrl);
        }

        if (!string.IsNullOrEmpty(link.Password) &&
            !string.IsNullOrEmpty(model.Password) &&
            passwordService.VerifyPassword(link.Password, model.Password))
        {
            link.Clicks++;
            await dbContext.SaveChangesAsync();
            return Redirect(link.TargetUrl);
        }
        else
        {
            ModelState.AddModelError(nameof(model.Password), "Incorrect password.");
            model.Code = code;
            return this.SimpleView(model, "EnterPassword");
        }
    }
}
