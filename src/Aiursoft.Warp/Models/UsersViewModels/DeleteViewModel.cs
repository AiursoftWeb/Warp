using Aiursoft.Warp.Entities;
using Aiursoft.UiStack.Layout;

namespace Aiursoft.Warp.Models.UsersViewModels;

public class DeleteViewModel : UiStackLayoutViewModel
{
    public DeleteViewModel()
    {
        PageTitle = "Delete User";
    }

    public required User User { get; init; }
}
