using System.ComponentModel.DataAnnotations;

namespace Assig1.ViewModels
{
    public class OffenceDetailViewModel
    {

        [Display(Name = "Offence Code")]
        public string OffenceCode { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Section Code")]
        public string SectionCode { get; set; }

        [Display(Name = "Total Expiations")]
        public int TotalExpiations { get; set; }

        // List of filtered expiations
        public List<ExpiationDetailViewModel> Expiations { get; set; }
 
        [Display(Name = "Average Fee Per Expiation")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double AverageFee { get; set; }

        // Expiations per month for the dashboard
        [Display(Name = "Expiations Per Month")]
        public Dictionary<string, int> ExpiationsPerMonth { get; set; }
        public string StatusFilter { get; set; }
        public int? Year { get; set; }


    }
}
