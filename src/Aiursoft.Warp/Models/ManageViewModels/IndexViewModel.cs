using Aiursoft.UiStack.Layout;

namespace Aiursoft.Warp.Models.ManageViewModels;

public class IndexViewModel: UiStackLayoutViewModel
{
    public IndexViewModel()
    {
        PageTitle = "Manage";
    }

    public string? StatusMessage { get; set; }

    public bool AllowUserAdjustNickname { get; set; }
}
