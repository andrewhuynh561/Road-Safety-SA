using System.ComponentModel.DataAnnotations;

namespace Assig1.ViewModels
{
    public class ExpiationDetailViewModel
    {
        [Display(Name = "Incident Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]

        public DateOnly IncidentStartDate { get; set; }

        [Display(Name = "Issue Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateOnly? IssueDate { get; set; }

        [Display(Name = "Total Fee")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public int? TotalFee { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }
    }
}
