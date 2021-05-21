using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Wrap.Models.DashboardViewModels
{
    public class DeleteViewModel : LayoutViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public DeleteViewModel()
        {
        }

        public DeleteViewModel(WrapUser user) : base(user, "Delete record")
        {
        }

        [FromRoute]
        [Display(Name = "Record to be deleted")]
        public string RecordName { get; set; }

        public void Recover(WrapUser user)
        {
            RootRecover(user, "Delete record");
        }
    }
}
