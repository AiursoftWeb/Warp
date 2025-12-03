using Aiursoft.Warp.Authorization;
using Aiursoft.Warp.Entities;
using Aiursoft.UiStack.Layout;
using Microsoft.AspNetCore.Identity;

namespace Aiursoft.Warp.Models.UsersViewModels;

public class DetailsViewModel : UiStackLayoutViewModel
{
    public DetailsViewModel()
    {
        PageTitle = "User Details";
    }

    public required User User { get; init; }

    public required IList<IdentityRole> Roles { get; init; }

    public required List<PermissionDescriptor> Permissions { get; init; }
}
