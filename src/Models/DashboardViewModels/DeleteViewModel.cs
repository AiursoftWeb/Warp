using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Warp.Models.DashboardViewModels
{
    public class DeleteViewModel : LayoutViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public DeleteViewModel()
        {
        }

        public DeleteViewModel(WarpUser user) : base(user, "Delete record")
        {
        }

        [FromRoute]
        [Display(Name = "Record to be deleted")]
        public string RecordName { get; set; }

        public void Recover(WarpUser user)
        {
            RootRecover(user, "Delete record");
        }
    }
}
