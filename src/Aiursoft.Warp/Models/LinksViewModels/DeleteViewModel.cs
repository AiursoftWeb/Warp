using Aiursoft.UiStack.Layout;

namespace Aiursoft.Warp.Models.LinksViewModels;

public class DeleteViewModel : UiStackLayoutViewModel
{
    public DeleteViewModel()
    {
        PageTitle = "Delete Link";
    }


    public Guid Id { get; set; }


    public string? Title { get; set; }


    public required string TargetUrl { get; set; }


    public required string RedirectTo { get; set; }

    public DateTime? ExpireAt { get; set; }

    public bool IsCustom { get; set; }

    public bool IsPrivate { get; set; }


    public string? Password { get; set; }

    public long? MaxClicks { get; set; }

    public long Clicks { get; set; }

    public DateTime CreationTime { get; init; } = DateTime.UtcNow;


    public required string UserId { get; set; }


}
