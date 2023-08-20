using System.ComponentModel.DataAnnotations;
using Aiursoft.CSTools.Attributes;

namespace Aiursoft.Warp.Models.DashboardViewModels
{
    public class IndexViewModel : LayoutViewModel
    {
        [Obsolete(error: true, message: "This method is only for framework!")]
        public IndexViewModel()
        {
        }

        public IndexViewModel(WarpUser user) : base(user, "Create new record")
        {

        }

        public void Recover(WarpUser user)
        {
            RootRecover(user, "Create new record");
        }

        [Display(Name = "Your shorten URL")]
        [Required]
        [ValidDomainName]
        [MaxLength(50)]
        [MinLength(5)]
        public string NewRecordName { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 7);

        [Required]
        [MaxLength(1000)]
        [MinLength(5)]
        [Url]
        public string Url { get; set; }
    }
}
