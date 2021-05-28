using Aiursoft.Warpgate.SDK.Models;
using System.Collections.Generic;

namespace Aiursoft.Warp.Models.DashboardViewModels
{
    public class RecordsViewModel : LayoutViewModel
    {
        public RecordsViewModel(WarpUser user, List<WarpRecord> records) : base(user, "My records")
        {
            Records = records;
        }

        public List<WarpRecord> Records { get; }
    }
}
