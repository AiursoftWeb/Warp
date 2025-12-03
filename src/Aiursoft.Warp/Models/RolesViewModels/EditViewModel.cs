// ... other using statements

using System.ComponentModel.DataAnnotations;
using Aiursoft.UiStack.Layout;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Warp.Models.RolesViewModels;

public class RoleClaimViewModel
{
    public required string Key { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public bool IsSelected { get; set; }
}

public class EditViewModel: UiStackLayoutViewModel
{
    public EditViewModel()
    {
        PageTitle = "Edit Role";
        Claims = [];
    }

    [Required(ErrorMessage = "The {0} is required.")]
    [FromRoute]
    public required string Id { get; init; }

    [Required(ErrorMessage = "The {0} is required.")]
    [Display(Name = "Role Name")]
    public required string RoleName { get; init; }

    public List<RoleClaimViewModel> Claims { get; init; }
}
