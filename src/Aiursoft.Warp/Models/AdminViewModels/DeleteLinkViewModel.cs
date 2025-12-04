using Aiursoft.UiStack.Layout;
using Aiursoft.Warp.Entities;

namespace Aiursoft.Warp.Models.AdminViewModels;

public class DeleteLinkViewModel : UiStackLayoutViewModel
{
    public required ShorterLink Link { get; set; }
}