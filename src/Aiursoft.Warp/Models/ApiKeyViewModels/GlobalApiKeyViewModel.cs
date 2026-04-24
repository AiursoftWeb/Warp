using Aiursoft.UiStack.Layout;

namespace Aiursoft.Warp.Models.ApiKeyViewModels;

public class GlobalApiKeyViewModel : UiStackLayoutViewModel
{
    public GlobalApiKeyViewModel()
    {
        PageTitle = "Global API Key";
    }

    public required string ApiKey { get; init; }
    public required string ApiUrl { get; init; }
}
