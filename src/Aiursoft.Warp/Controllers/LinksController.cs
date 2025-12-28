using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.Warp.Entities;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Aiursoft.Warp.Models.LinksViewModels;
using Aiursoft.Warp.Services;
using Aiursoft.UiStack.Navigation;

using Microsoft.Extensions.Localization;

namespace Aiursoft.Warp.Controllers;

[Authorize]
public class LinksController : Controller
{
    private readonly TemplateDbContext _dbContext;
    private readonly PasswordService _passwordService;
    private readonly IStringLocalizer<LinksController> _localizer;

    public LinksController(
        TemplateDbContext dbContext, 
        PasswordService passwordService,
        IStringLocalizer<LinksController> localizer)
    {
        _dbContext = dbContext;
        _passwordService = passwordService;
        _localizer = localizer;
    }

    [HttpGet]
    [RenderInNavBar(
        NavGroupName = "Home",
        NavGroupOrder = 1,
        CascadedLinksGroupName = "Home",
        CascadedLinksIcon = "home",
        CascadedLinksOrder = 1,
        LinkText = "My Links",
        LinkOrder = 1)]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var links = await _dbContext.ShorterLinks
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.CreationTime)
            .ToListAsync();

        var model = new IndexViewModel
        {
            Links = links
        };
        // return View(model);
        return this.StackView(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var link = await _dbContext.ShorterLinks.FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);
        if (link == null)
        {
            return NotFound();
        }

        var model = new EditViewModel
        {
            Id = link.Id,
            Title = link.Title,
            TargetUrl = link.TargetUrl,
            CustomCode = link.RedirectTo,
            ExpireAt = link.ExpireAt,
            MaxClicks = link.MaxClicks,
            IsPrivate = link.IsPrivate,
            CreationTime = link.CreationTime,
            Clicks = link.Clicks,
            ShortLink = $"{Request.Scheme}://{Request.Host}/r/{link.RedirectTo}"
        };

        return this.StackView(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var linkInDb = await _dbContext.ShorterLinks.AsNoTracking().FirstOrDefaultAsync(l => l.Id == model.Id && l.UserId == currentUserId);
            if (linkInDb != null)
            {
                model.CreationTime = linkInDb.CreationTime;
                model.Clicks = linkInDb.Clicks;
                model.ShortLink = $"{Request.Scheme}://{Request.Host}/r/{linkInDb.RedirectTo}";
            }
            // return View(model);
            return this.StackView(model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var link = await _dbContext.ShorterLinks.FirstOrDefaultAsync(l => l.Id == model.Id && l.UserId == userId);
        if (link == null)
        {
            return NotFound();
        }

        // Determine effective code
        var effectiveCode = link.RedirectTo;
        if (!string.IsNullOrEmpty(model.CustomCode))
        {
            effectiveCode = model.CustomCode;
        }

        // Check loop
        var finalUrl = $"{Request.Scheme}://{Request.Host}/r/{effectiveCode}";
        if (model.TargetUrl.TrimEnd('/').Equals(finalUrl.TrimEnd('/'), StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(model.TargetUrl), _localizer["The target URL cannot be the same as the shortcut URL."]);
            model.CreationTime = link.CreationTime;
            model.Clicks = link.Clicks;
            model.ShortLink = $"{Request.Scheme}://{Request.Host}/r/{link.RedirectTo}";
            return this.StackView(model);
        }

        // Check if custom code is changed and already exists
        if (link.RedirectTo != model.CustomCode && !string.IsNullOrEmpty(model.CustomCode))
        {
            if (await _dbContext.ShorterLinks.AnyAsync(l => l.RedirectTo == model.CustomCode))
            {
                ModelState.AddModelError(nameof(model.CustomCode), _localizer["This custom code is already in use. Please choose another one."]);
                model.CreationTime = link.CreationTime;
                model.Clicks = link.Clicks;
                model.ShortLink = $"{Request.Scheme}://{Request.Host}/r/{link.RedirectTo}";
                return this.StackView(model);
            }
        }

        link.Title = model.Title;
        link.TargetUrl = model.TargetUrl;

        // Only update RedirectTo if CustomCode is provided and different
        if (!string.IsNullOrEmpty(model.CustomCode))
        {
            link.RedirectTo = model.CustomCode;
            link.IsCustom = true;
        }

        link.ExpireAt = model.ExpireAt;
        link.MaxClicks = model.MaxClicks;
        link.IsPrivate = model.IsPrivate;

        if (model.IsPrivate && !string.IsNullOrEmpty(model.Password))
        {
            link.Password = _passwordService.HashPassword(model.Password);
        }
        else if (!model.IsPrivate)
        {
            link.Password = null;
        }
        // If IsPrivate is true but Password is empty, keep the old password (if any) or it's a mistake?
        // Usually "Leave empty to keep unchanged" is a good pattern for passwords.
        // But here, if the user switches to Private, they might expect to set a password.
        // If they are already Private and leave it empty, we should probably keep the old one.
        // Let's assume: if IsPrivate is true and Password is NOT empty, update it.
        // If IsPrivate is true and Password IS empty, keep old password.
        // If IsPrivate is false, clear password.

        _dbContext.ShorterLinks.Update(link);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var link = await _dbContext.ShorterLinks.FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);
        if (link == null)
        {
            return NotFound();
        }

        var model = new DeleteViewModel
        {
            Id = link.Id,
            Title = link.Title,
            TargetUrl = link.TargetUrl,
            CreationTime = link.CreationTime,
            UserId = link.UserId,
            Clicks = link.Clicks,
            ExpireAt = link.ExpireAt,
            IsCustom = link.IsCustom,
            IsPrivate = link.IsPrivate,
            MaxClicks = link.MaxClicks,
            Password = link.Password,
            RedirectTo = link.RedirectTo
        };
        return this.StackView(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var link = await _dbContext.ShorterLinks.FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);
        if (link != null)
        {
            _dbContext.ShorterLinks.Remove(link);
            await _dbContext.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
