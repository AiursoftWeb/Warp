using System.ComponentModel.DataAnnotations;
using Aiursoft.UiStack.Layout;

namespace Aiursoft.Warp.Models.ApiKeysViewModels;

public class CreateViewModel : UiStackLayoutViewModel
{
    public CreateViewModel()
    {
        PageTitle = "create ApiKey";
    }

    [Required(ErrorMessage = "Something went wrong, please try again later.")]
    public Guid ApiKeyId { get; set; } = Guid.NewGuid();

    [Display(Name = "Expiration Time")]
    public DateTime? ExpireAt { get; set; }


    [RegularExpression(@"^[a-zA-Z0-9\s.,?!'-]{0,100}$", ErrorMessage = "Name contains invalid characters.")]
    [Required(ErrorMessage = "Please input your api key name!")]
    [Display(Name = "Name")]
    public string Name { get; set; }

    public string? CreatedApiKey { get; set; }

}
