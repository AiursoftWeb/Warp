using System.ComponentModel.DataAnnotations;
using Aiursoft.UiStack.Layout;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Aiursoft.Warp.Models.AdminViewModels;

public class EditLinkViewModel : UiStackLayoutViewModel
{
    public Guid LinkId { get; set; }

    [MaxLength(100)]
    public string? Title { get; set; }

    [Required(ErrorMessage = "Please input your markdown content!")]
    [MaxLength(65535)]
    [Display(Name = "Markdown Content")]
    public string TargetUrl { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select the owner of this Link!")]
    [Display(Name = "Owner")]
    public string SelectedUserId { get; set; } = string.Empty;

    public List<SelectListItem> AllUsers { get; set; } = new();

    public bool SavedSuccessfully { get; set; }
}