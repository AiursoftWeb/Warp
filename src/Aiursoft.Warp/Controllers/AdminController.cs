using Aiursoft.CSTools.Tools;
using Aiursoft.Warp.Authorization;
using Aiursoft.Warp.Entities;
using Aiursoft.Warp.Services;
using Aiursoft.UiStack.Navigation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Aiursoft.Warp.Models.AdminViewModels;

namespace Aiursoft.Warp.Controllers;

/// <summary>
/// This controller is used for administrative actions related to links.
/// </summary>
[Authorize]
public class AdminController(
    IStringLocalizer<AdminController> localizer,
    UserManager<User> userManager,
    TemplateDbContext context)
    : Controller
{
    /// <summary>
    /// Displays a list of all shorter links in the system.
    /// This action requires the 'CanReadAllLinks' permission.
    /// </summary>
    [Authorize(Policy = AppPermissionNames.CanReadAllLinks)]
    [RenderInNavBar(
        NavGroupName = "Administration",
        NavGroupOrder = 9999,
        CascadedLinksGroupName = "Links",
        CascadedLinksIcon = "server",
        CascadedLinksOrder = 1,
        LinkText = "All Links",
        LinkOrder = 1)]
    public async Task<IActionResult> AllLinks()
    {
        var allLinks = await context.ShorterLinks
            .Include(d => d.User)
            .OrderByDescending(d => d.CreationTime)
            .ToListAsync();

        return this.StackView(new AllLinksViewModel
        {
            AllLinks = allLinks
        });
    }

    /// <summary>
    /// Displays a list of shorter links for a specific user.
    /// This action requires the 'CanReadAllLinks' permission.
    /// </summary>
    [Authorize(Policy = AppPermissionNames.CanReadAllLinks)]
    public async Task<IActionResult> UserLinks([FromRoute] string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound("User ID is required.");
        }

        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var links = await context.ShorterLinks
            .Where(d => d.UserId == id)
            .OrderByDescending(d => d.CreationTime)
            .ToListAsync();

        var model = new UserLinksViewModel
        {
            User = user,
            UserLinks = links
        };

        return this.StackView(model);
    }

    /// <summary>
    /// Allows an administrator to edit any link, including its owner.
    /// This action requires the 'CanEditAnyLink' permission.
    /// </summary>
    [Authorize(Policy = AppPermissionNames.CanEditAnyLink)]
    public async Task<IActionResult> EditLink([FromRoute] Guid id, [FromQuery] bool? saved = false)
    {
        var link = await context.ShorterLinks
            .FirstOrDefaultAsync(d => d.Id == id);

        if (link == null)
        {
            return NotFound("Link not found.");
        }

        var allUsers = await userManager.Users
            .OrderBy(u => u.UserName)
            .ToListAsync();

        var model = new EditLinkViewModel
        {
            LinkId = link.Id,
            Title = link.Title  ?? string.Empty,
            TargetUrl = link.TargetUrl,
            SelectedUserId = link.UserId,
            AllUsers = allUsers.Select(user => new SelectListItem
            {
                Value = user.Id,
                Text = user.UserName,
                Selected = user.Id == link.UserId
            }).ToList(),
            SavedSuccessfully = saved ?? false
        };

        return this.StackView(model);
    }

    /// <summary>
    /// Saves the changes to a link from an administrator, including the owner.
    /// This action requires the 'CanEditAnyLink' permission.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AppPermissionNames.CanEditAnyLink)]
    public async Task<IActionResult> EditLink(EditLinkViewModel model)
    {
        var newOwner = await userManager.FindByIdAsync(model.SelectedUserId);
        if (!ModelState.IsValid || newOwner == null)
        {
            if (newOwner == null)
            {
                ModelState.AddModelError(nameof(model.SelectedUserId), localizer["The selected new owner does not exist."]);
            }
            var allUsers = await userManager.Users.OrderBy(u => u.UserName).ToListAsync();
            model.AllUsers = allUsers.Select(user => new SelectListItem
            {
                Value = user.Id,
                Text = user.UserName,
                Selected = user.Id == model.SelectedUserId
            }).ToList();
            return this.StackView(model);
        }

        var linkInDb = await context.ShorterLinks.FirstOrDefaultAsync(d => d.Id == model.LinkId);
        if (linkInDb == null)
        {
            return NotFound("Link not found.");
        }
        linkInDb.TargetUrl = model.TargetUrl.SafeSubstring(65535);
        linkInDb.Title = model.Title;
        linkInDb.UserId = model.SelectedUserId;

        await context.SaveChangesAsync();
        return RedirectToAction(nameof(EditLink), new { id = model.LinkId, saved = true });
    }

    /// <summary>
    /// Displays a confirmation page before an administrator deletes a link.
    /// This action requires the 'CanDeleteAnyLink' permission.
    /// </summary>
    [Authorize(Policy = AppPermissionNames.CanDeleteAnyLink)]
    public async Task<IActionResult> DeleteLink([FromRoute] Guid id)
    {
        var link = await context.ShorterLinks
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (link == null)
        {
            return NotFound("Link not found.");
        }

        return this.StackView(new DeleteLinkViewModel
        {
            Link = link
        });
    }

    /// <summary>
    /// Deletes a link from the database.
    /// This action requires the 'CanDeleteAnyLink' permission.
    /// </summary>
    [HttpPost, ActionName("DeleteLink")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = AppPermissionNames.CanDeleteAnyLink)]
    public async Task<IActionResult> DeleteLinkConfirmed([FromRoute] Guid id)
    {
        var link = await context.ShorterLinks.FirstOrDefaultAsync(d => d.Id == id);
        if (link == null)
        {
            return NotFound("Link not found.");
        }

        context.ShorterLinks.Remove(link);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(AllLinks));
    }
}