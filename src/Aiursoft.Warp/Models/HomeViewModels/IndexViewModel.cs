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
    public Guid LinkId { get; init; } = Guid.NewGuid();

    [Required(ErrorMessage = "Please input your target link!")]
    public string TargetUrl { get; init; } = string.Empty;

    [RegularExpression(@"^[a-zA-Z0-9-]{0,30}$", ErrorMessage = "Custom code can only contain letters, numbers, and hyphens, and be up to 30 characters long.")]
    public string? CustomCode { get; init; } = null;

    public DateTime? ExpireAt { get; init; } = null;

    public bool IsPrivate { get; init; } = false;

    public string? Password { get; init; } = null;

    public long? MaxClicks { get; init; }

    [RegularExpression(@"^[a-zA-Z0-9\s.,?!'-]{0,100}$", ErrorMessage = "Title contains invalid characters.")]
    public string? Title { get; init; } = null;

    public string? CreatedShortLink { get; set; }

}
