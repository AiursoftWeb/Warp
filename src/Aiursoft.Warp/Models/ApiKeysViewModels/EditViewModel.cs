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

    [MaxLength(100, ErrorMessage = "The {0} must be at max {1} characters long.")]
    [Display(Name = "Name")]
    public string Name { get; set; }

    [Display(Name = "Expiration Time")]
    public DateTime? ExpireAt { get; set; }

    public DateTime CreationTime { get; set; }
    
    public DateTime? LastUsedAt { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public int UsageCount { get; set; }

}
