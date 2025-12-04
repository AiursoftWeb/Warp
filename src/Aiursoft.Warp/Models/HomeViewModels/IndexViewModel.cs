using System.ComponentModel.DataAnnotations;
using Aiursoft.UiStack.Layout;

namespace Aiursoft.Warp.Models.HomeViewModels;

public class IndexViewModel : UiStackLayoutViewModel
{
    public IndexViewModel()
    {
        PageTitle = "Link Shorter";
    }

    [Required(ErrorMessage = "Something went wrong, please try again later.")]
    public Guid LinkId { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "Please input your target link!")]
    [Url(ErrorMessage = "The {0} is not a valid URL.")]
    [Display(Name = "Target URL")]
    public string TargetUrl { get; set; } = string.Empty;

    [RegularExpression(@"^[a-zA-Z0-9-]{0,30}$", ErrorMessage = "Custom code can only contain letters, numbers, and hyphens, and be up to 30 characters long.")]
    [Display(Name = "Custom Code")]
    public string? CustomCode { get; set; }

    [Display(Name = "Expiration Time")]
    public DateTime? ExpireAt { get; set; }

    [Display(Name = "Private Link")]
    public bool IsPrivate { get; set; }

    [Display(Name = "Password")]
    public string? Password { get; set; }

    [Display(Name = "Max Clicks")]
    public long? MaxClicks { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s.,?!'-]{0,100}$", ErrorMessage = "Title contains invalid characters.")]
    [Display(Name = "Title")]
    public string? Title { get; set; }

    public string? CreatedShortLink { get; set; }

}
