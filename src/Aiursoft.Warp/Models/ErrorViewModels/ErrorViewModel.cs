using Aiursoft.UiStack.Layout;

namespace Aiursoft.Warp.Models.ErrorViewModels;

public class ErrorViewModel: UiStackLayoutViewModel
{
    public ErrorViewModel()
    {
        PageTitle = "Error";
    }

    public required string RequestId { get; init; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
