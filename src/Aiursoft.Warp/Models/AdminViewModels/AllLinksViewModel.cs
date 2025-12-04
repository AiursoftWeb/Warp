using Aiursoft.UiStack.Layout;
using Aiursoft.Warp.Entities;

namespace Aiursoft.Warp.Models.AdminViewModels;

public class AllLinksViewModel : UiStackLayoutViewModel
{
    public AllLinksViewModel()
    {
        PageTitle = "All Links";
    }

    public required List<ShorterLink> AllLinks { get; set; }
}