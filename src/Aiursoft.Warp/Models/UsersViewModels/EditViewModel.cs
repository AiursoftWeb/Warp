using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Aiursoft.CSTools.Attributes;
using Aiursoft.UiStack.Layout;
using Microsoft.AspNetCore.Mvc;

namespace Aiursoft.Warp.Models.UsersViewModels;

// Manage if a role is selected or not in the UI.
public class UserRoleViewModel
{
    public required string RoleName { get; init; }
    public bool IsSelected { get; init; }
}

public class EditViewModel : UiStackLayoutViewModel
{
    public EditViewModel()
    {
        PageTitle = "Edit User";
        AllRoles = [];
    }

    [Required(ErrorMessage = "The {0} is required.")]
    [Display(Name = "User name")]
    [ValidDomainName]
    public required string UserName { get; init; }

    [Required(ErrorMessage = "The {0} is required.")]
    [Display(Name = "Name")]
    public required string DisplayName { get; init; }

    [Required(ErrorMessage = "The {0} is required.")]
    [EmailAddress(ErrorMessage = "The {0} is not a valid email address.")]
    [Display(Name = "Email Address")]
    public required string Email { get; init; }

    [DataType(DataType.Password)]
    [Display(Name = "Reset Password (leave empty to keep the same password)")]
    public string? Password { get; init; }

    [NotNull]
    [Display(Name = "Avatar file")]
    [Required(ErrorMessage = "The avatar file is required.")]
    [RegularExpression(@"^Workspace/avatar.*", ErrorMessage = "The avatar file is invalid. Please upload it again.")]
    [MaxLength(150)]
    [MinLength(2)]
    public string? AvatarUrl { get; init; }

    [Required(ErrorMessage = "The {0} is required.")]
    [FromRoute]
    public required string Id { get; init; }

    public List<UserRoleViewModel> AllRoles { get; init; }
}
