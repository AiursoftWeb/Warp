using Aiursoft.UiStack.Layout;
using Aiursoft.Warp.Entities;

namespace Aiursoft.Warp.Models.AdminViewModels;

public class AllLinksViewModel : UiStackLayoutViewModel
{
    public required List<ShorterLink> AllLinks { get; set; }
}