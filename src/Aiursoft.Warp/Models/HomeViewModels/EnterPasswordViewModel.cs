using System.ComponentModel.DataAnnotations;
using Aiursoft.UiStack.Layout;

namespace Aiursoft.Warp.Models.HomeViewModels;

public class EnterPasswordViewModel : UiStackLayoutViewModel
{
    public EnterPasswordViewModel()
    {
        PageTitle = "Enter Password";
    }

    [Required(ErrorMessage = "The {0} is required.")]
    public string? Code { get; set; }

    [Required(ErrorMessage = "The {0} is required.")]
    public Guid LinkId { get; set; }

    [Required(ErrorMessage = "The {0} is required.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string? Password { get; set; }
}
