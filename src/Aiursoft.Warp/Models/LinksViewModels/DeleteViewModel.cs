using Aiursoft.UiStack.Layout;

namespace Aiursoft.Warp.Models.LinksViewModels;

public class DeleteViewModel : UiStackLayoutViewModel
{
    public DeleteViewModel()
    {
        PageTitle = "delete Link";
    }


    public Guid Id { get; init; }


    public string? Title { get; init; }


    public required string TargetUrl { get; init; }


    public required string RedirectTo { get; init; }

    public DateTime? ExpireAt { get; init; }

    public bool IsCustom { get; init; }

    public bool IsPrivate { get; init; }



    public string? Password { get; init; }

    public long? MaxClicks { get; init; }

    public long Clicks { get; init; }

    public DateTime CreationTime { get; init; } = DateTime.UtcNow;

    public required string UserId { get; init; }
}
