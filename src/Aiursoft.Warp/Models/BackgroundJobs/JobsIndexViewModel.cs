using Aiursoft.UiStack.Layout;

namespace Aiursoft.Warp.Models.BackgroundJobs;

public class JobsIndexViewModel : UiStackLayoutViewModel
{
    public IEnumerable<JobInfo> AllRecentJobs { get; init; } = [];
}
