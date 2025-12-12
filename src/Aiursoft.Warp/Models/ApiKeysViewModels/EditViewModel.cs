using System.ComponentModel.DataAnnotations;
using Aiursoft.UiStack.Layout;

namespace Aiursoft.Warp.Models.ApiKeysViewModels;

public class EditViewModel : UiStackLayoutViewModel
{
    public EditViewModel()
    {
        PageTitle = "Edit ApiKey";
    }
    public Guid Id { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9\s.,?!'-]{0,100}$", ErrorMessage = "Name contains invalid characters.")]
    [Required(ErrorMessage = "Please input your api key name!")]
    [Display(Name = "Name")]
    public required string Name { get; set; }

    [Display(Name = "Expiration Time")]
    public DateTime? ExpireAt { get; set; }

    public DateTime CreationTime { get; set; }
    
    public DateTime? LastUsedAt { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public int UsageCount { get; set; }

}
