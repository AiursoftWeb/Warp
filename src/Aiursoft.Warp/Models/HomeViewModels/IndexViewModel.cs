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
    public string TargetUrl { get; set; } = string.Empty;
    
    [RegularExpression(@"^[a-zA-Z0-9-]{0,30}$", ErrorMessage = "Custom code can only contain letters, numbers, and hyphens, and be up to 30 characters long.")]
    public string? CustomCode { get; set; } = null;
    
    public DateTime? ExpireAt { get; set; } = null;
    
    public bool IsPrivate { get; set; } = false;
    
    public string? Password { get; set; } = null;
    
    public long? MaxClicks { get; set; }
    
    [RegularExpression(@"^[a-zA-Z0-9\s.,?!'-]{0,100}$", ErrorMessage = "Title contains invalid characters.")]
    public string? Title { get; set; } = null;

    public string? CreatedShortLink { get; set; }

}