using Aiursoft.UiStack.Layout;
using Aiursoft.Warp.Entities;

namespace Aiursoft.Warp.Models.AdminViewModels;

public class DeleteLinkViewModel : UiStackLayoutViewModel
{
    public DeleteLinkViewModel()
    {
        PageTitle = "Delete Link";
    }
    public required ShorterLink Link { get; set; }
}