using System.ComponentModel.DataAnnotations;
using Aiursoft.UiStack.Layout;

namespace Aiursoft.Warp.Models.LinksViewModels;

public class EditViewModel : UiStackLayoutViewModel
{
    public EditViewModel()
    {
        PageTitle = "Edit Link";
    }
    public Guid Id { get; init; }

    [MaxLength(100)]
    public string? Title { get; init; }

    [Required]
    [MaxLength(65535)]
    [Url]
    public string TargetUrl { get; init; } = string.Empty;
}
