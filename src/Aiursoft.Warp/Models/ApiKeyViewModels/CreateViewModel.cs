using System.ComponentModel.DataAnnotations;
using Aiursoft.UiStack.Layout;

namespace Aiursoft.Warp.Models.ApiKeyViewModels;

public class CreateViewModel : UiStackLayoutViewModel
{
    public CreateViewModel()
    {
        PageTitle = "Create API Key";
    }

    [MaxLength(100)]
    [Display(Name = "Key Name")]
    public string? Name { get; set; }

    [Display(Name = "Expiration")]
    [DataType(DataType.DateTime)]
    public DateTime? ExpireAt { get; set; }
}
