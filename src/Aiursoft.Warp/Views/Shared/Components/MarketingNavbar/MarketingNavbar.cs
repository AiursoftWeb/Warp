using Microsoft.AspNetCore.Mvc;
using Aiursoft.Warp.Services;
using Aiursoft.Warp.Services.FileStorage;

namespace Aiursoft.Warp.Views.Shared.Components.MarketingNavbar;

public class MarketingNavbar(
    GlobalSettingsService globalSettingsService,
    StorageService storageService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(MarketingNavbarViewModel? model = null)
    {
        model ??= new MarketingNavbarViewModel();
        model.ProjectName = await globalSettingsService.GetSettingValueAsync("ProjectName");
        var logoPath = await globalSettingsService.GetSettingValueAsync("ProjectLogo");
        if (!string.IsNullOrWhiteSpace(logoPath))
        {
            model.LogoUrl = storageService.RelativePathToInternetUrl(logoPath, HttpContext);
        }
        return View(model);
    }
}
