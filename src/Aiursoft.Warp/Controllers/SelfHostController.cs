using Aiursoft.Warp.Models.SelfHostViewModels;
using Aiursoft.Warp.Services;
using Aiursoft.UiStack.Navigation;
using Aiursoft.WebTools.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Warp.Controllers;

/// <summary>
/// This controller handles the self-host deployment information page.
/// </summary>
[AllowAnonymous]
[LimitPerMin]
public class SelfHostController : Controller
{
    [RenderInNavBar(
        NavGroupName = "Self Host",
        NavGroupOrder = 5,
        CascadedLinksGroupName = "Self Host",
        CascadedLinksIcon = "cloud-upload",
        CascadedLinksOrder = 1,
        LinkText = "Self host a new server",
        LinkOrder = 1)]
    public IActionResult Index()
    {
        return this.StackView(new IndexViewModel());
    }
}
