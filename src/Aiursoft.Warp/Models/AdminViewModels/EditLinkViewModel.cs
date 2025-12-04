using System.ComponentModel.DataAnnotations;
using Aiursoft.UiStack.Layout;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Aiursoft.Warp.Models.AdminViewModels;

public class EditLinkViewModel : UiStackLayoutViewModel
{
    public EditLinkViewModel()
    {
        PageTitle = "Edit Link";
    }

    public Guid LinkId { get; set; }

    [MaxLength(100, ErrorMessage = "The {0} must be at max {1} characters long.")]
    public string? Title { get; set; }

    [Required(ErrorMessage = "The {0} is required.")]
    [MaxLength(65535, ErrorMessage = "The {0} must be at max {1} characters long.")]
    [Url(ErrorMessage = "The {0} is not a valid URL.")]
    public string TargetUrl { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select the owner of this Link!")]
    [Display(Name = "Owner")]
    public string SelectedUserId { get; set; } = string.Empty;

    public List<SelectListItem> AllUsers { get; set; } = new();

    public bool SavedSuccessfully { get; set; }

    [MaxLength(32, ErrorMessage = "The {0} must be at max {1} characters long.")]
    [Display(Name = "Custom Code")]
    public string? CustomCode { get; set; }

    [Display(Name = "Expiration Time")]
    public DateTime? ExpireAt { get; set; }

    [Display(Name = "Max Clicks")]
    public long? MaxClicks { get; set; }

    [Display(Name = "Private Link")]
    public bool IsPrivate { get; set; }

    [MaxLength(100, ErrorMessage = "The {0} must be at max {1} characters long.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string? Password { get; set; }

    public DateTime CreationTime { get; set; }
    public long Clicks { get; set; }
    public string? ShortLink { get; set; }
}
