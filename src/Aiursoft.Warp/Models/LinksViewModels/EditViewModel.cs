using System.ComponentModel.DataAnnotations;
using Aiursoft.UiStack.Layout;
using Aiursoft.Warp.Entities;

namespace Aiursoft.Warp.Models.LinksViewModels;

public class EditViewModel : UiStackLayoutViewModel
{
    public EditViewModel()
    {
        PageTitle = "Edit Link";
    }
    public Guid Id { get; set; }

    [MaxLength(100, ErrorMessage = "The {0} must be at max {1} characters long.")]
    [Display(Name = "Title")]
    public string? Title { get; set; }

    [Required(ErrorMessage = "The {0} is required.")]
    [MaxLength(65535, ErrorMessage = "The {0} must be at max {1} characters long.")]
    [Url(ErrorMessage = "The {0} is not a valid URL.")]
    [Display(Name = "Target URL")]
    public string TargetUrl { get; set; } = string.Empty;

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

    public List<WarpHit> RecentHits { get; set; } = new();
}
