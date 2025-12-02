using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Warp.Models.ManageViewModels;

public class SwitchThemeViewModel
{
    [Required]
    public required string Theme { get; set; }
}
