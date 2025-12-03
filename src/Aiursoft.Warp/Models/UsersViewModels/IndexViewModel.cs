using Aiursoft.Warp.Entities;
using Aiursoft.UiStack.Layout;

namespace Aiursoft.Warp.Models.UsersViewModels;

public class UserWithRolesViewModel
{
    public required User User { get; init; }
    public required IList<string> Roles { get; init; }
}

public class IndexViewModel : UiStackLayoutViewModel
{
    public IndexViewModel()
    {
        PageTitle = "Users";
    }

    public required List<UserWithRolesViewModel> Users { get; init; }
}
