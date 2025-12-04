using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Warp.Models.ManageViewModels;

public class SwitchThemeViewModel
{
    [Required(ErrorMessage = "The {0} is required.")]
    public required string Theme { get; set; }
}
