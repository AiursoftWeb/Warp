using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.Warp.Entities;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Aiursoft.Warp.Models.LinksViewModels;
using Aiursoft.WebTools.Attributes;
using Aiursoft.CSTools.Tools;
using Aiursoft.Warp.Services;
using Aiursoft.UiStack.Navigation;

namespace Aiursoft.Warp.Controllers;

[Authorize]
[Route("[controller]/[action]")]
public class LinksController : Controller
{
    private readonly TemplateDbContext _dbContext;

    public LinksController(TemplateDbContext dbContext)
    {
        _dbContext = dbContext;
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
            TargetUrl = link.TargetUrl
        };

        return this.StackView(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // return View(model);
            return this.StackView(model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var link = await _dbContext.ShorterLinks.FirstOrDefaultAsync(l => l.Id == model.Id && l.UserId == userId);
        if (link == null)
        {
            return NotFound();
        }

        link.Title = model.Title;
        link.TargetUrl = model.TargetUrl;
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
