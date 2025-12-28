using Aiursoft.Warp.Entities;
using Aiursoft.Warp.Models.ApiModels;
using Aiursoft.Warp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aiursoft.Warp.Controllers;

[Route("api")]
[ApiController]
public class ApiController(
    WarpDbContext dbContext,
    PasswordService passwordService) : ControllerBase
{
    private async Task<User?> GetUserByApiKey()
    {
        if (!Request.Headers.TryGetValue("X-API-Key", out var apiKeyValues))
        {
            return null;
        }

        var apiKeyStr = apiKeyValues.ToString();
        var apiKey = await dbContext.WarpApiKeys
            .Include(k => k.User)
            .FirstOrDefaultAsync(k => k.ApiKey == apiKeyStr);

        if (apiKey == null)
        {
            return null;
        }

        if (apiKey.ExpireAt.HasValue && apiKey.ExpireAt < DateTime.UtcNow)
        {
            return null;
        }

        return apiKey.User;
    }

    [HttpGet("links")]
    public async Task<IActionResult> ListLinks()
    {
        var user = await GetUserByApiKey();
        if (user == null) return Unauthorized();

        var links = await dbContext.ShorterLinks
            .Where(l => l.UserId == user.Id)
            .OrderByDescending(l => l.CreationTime)
            .ToListAsync();

        return Ok(links);
    }

    [HttpPost("links")]
    public async Task<IActionResult> CreateLink([FromBody] CreateLinkApiModel model)
    {
        var user = await GetUserByApiKey();
        if (user == null) return Unauthorized();

        if (!ModelState.IsValid) return BadRequest(ModelState);

        var code = model.CustomCode;
        if (string.IsNullOrEmpty(code))
        {
            code = Guid.NewGuid().ToString().Substring(0, 6);
        }

        // Check if custom code already exists
        if (await dbContext.ShorterLinks.AnyAsync(l => l.RedirectTo == code))
        {
            return Conflict(new { message = "This custom code is already in use." });
        }

        var shorterLink = new ShorterLink
        {
            Title = model.Title,
            TargetUrl = model.TargetUrl,
            RedirectTo = code,
            ExpireAt = model.ExpireAt,
            IsCustom = !string.IsNullOrEmpty(model.CustomCode),
            IsPrivate = model.IsPrivate,
            MaxClicks = model.MaxClicks,
            UserId = user.Id
        };

        if (model.IsPrivate && !string.IsNullOrEmpty(model.Password))
        {
            shorterLink.Password = passwordService.HashPassword(model.Password);
        }

        dbContext.ShorterLinks.Add(shorterLink);
        await dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetLink), new { id = shorterLink.Id }, shorterLink);
    }

    [HttpGet("links/{id:guid}")]
    public async Task<IActionResult> GetLink(Guid id)
    {
        var user = await GetUserByApiKey();
        if (user == null) return Unauthorized();

        var link = await dbContext.ShorterLinks.FirstOrDefaultAsync(l => l.Id == id && l.UserId == user.Id);
        if (link == null) return NotFound();

        return Ok(link);
    }

    [HttpPatch("links/{id:guid}")]
    public async Task<IActionResult> UpdateLink(Guid id, [FromBody] UpdateLinkApiModel model)
    {
        var user = await GetUserByApiKey();
        if (user == null) return Unauthorized();

        var link = await dbContext.ShorterLinks.FirstOrDefaultAsync(l => l.Id == id && l.UserId == user.Id);
        if (link == null) return NotFound();

        if (model.Title != null) link.Title = model.Title;
        if (model.TargetUrl != null) link.TargetUrl = model.TargetUrl;
        if (model.ExpireAt != null) link.ExpireAt = model.ExpireAt;
        if (model.MaxClicks != null) link.MaxClicks = model.MaxClicks;

        if (model.IsPrivate.HasValue)
        {
            link.IsPrivate = model.IsPrivate.Value;
            if (link.IsPrivate && !string.IsNullOrEmpty(model.Password))
            {
                link.Password = passwordService.HashPassword(model.Password);
            }
            else if (!link.IsPrivate)
            {
                link.Password = null;
            }
        }

        if (!string.IsNullOrEmpty(model.CustomCode) && model.CustomCode != link.RedirectTo)
        {
            if (await dbContext.ShorterLinks.AnyAsync(l => l.RedirectTo == model.CustomCode))
            {
                return Conflict(new { message = "This custom code is already in use." });
            }
            link.RedirectTo = model.CustomCode;
            link.IsCustom = true;
        }

        dbContext.ShorterLinks.Update(link);
        await dbContext.SaveChangesAsync();

        return Ok(link);
    }

    [HttpDelete("links/{id:guid}")]
    public async Task<IActionResult> DeleteLink(Guid id)
    {
        var user = await GetUserByApiKey();
        if (user == null) return Unauthorized();

        var link = await dbContext.ShorterLinks.FirstOrDefaultAsync(l => l.Id == id && l.UserId == user.Id);
        if (link != null)
        {
            dbContext.ShorterLinks.Remove(link);
            await dbContext.SaveChangesAsync();
        }

        return NoContent();
    }
}
