using Aiursoft.UiStack.Layout;
using Aiursoft.Warp.Entities;

namespace Aiursoft.Warp.Models.AdminViewModels;

public class UserLinksViewModel : UiStackLayoutViewModel
{
    public required User User { get; set; }
    public required List<ShorterLink> UserLinks { get; set; }
}