﻿using Aiursoft.Warpgate.SDK.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Warp.Models.DashboardViewModels
{
    public class EditViewModel : LayoutViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public EditViewModel()
        {
        }

        public EditViewModel(WarpUser user) : base(user, "Edit record")
        {
        }

        public void Recover(WarpUser user)
        {
            RootRecover(user, "Edit record");
        }

        [FromRoute]
        public string RecordName { get; set; }

        [Display(Name = "Rename record")]
        public string NewRecordName { get; set; }

        public RecordType Type { get; set; }

        [Display(Name = "Target URL")]
        public string URL { get; set; }

        public bool Enabled { get; set; }
    }
}
