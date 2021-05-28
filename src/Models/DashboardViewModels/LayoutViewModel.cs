using System;

namespace Aiursoft.Warp.Models.DashboardViewModels
{
    public class LayoutViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public LayoutViewModel() { }
        public LayoutViewModel(WarpUser user, string title)
        {
            RootRecover(user, title);
        }

        public bool JustHaveUpdated { get; set; } = false;

        public void RootRecover(WarpUser user, string title)
        {
            UserName = user.NickName;
            EmailConfirmed = user.EmailConfirmed;
            Title = title;
        }
        public string UserName { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Title { get; set; }
    }
}
