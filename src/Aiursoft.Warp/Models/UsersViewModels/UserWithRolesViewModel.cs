using Aiursoft.Warp.Entities;

namespace Aiursoft.Warp.Models.UsersViewModels;

public class UserWithRolesViewModel
{
    public required User User { get; set; }
    public required IList<string> Roles { get; set; }
}
