using System;
using System.ComponentModel.DataAnnotations;
using Aiursoft.UiStack.Layout;

namespace Aiursoft.Warp.Models.HomeViewModels;

public class EnterPasswordViewModel: UiStackLayoutViewModel
{
    [Required]
    public string? Code { get; set; }
    
    [Required]
    public Guid LinkId { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
}
