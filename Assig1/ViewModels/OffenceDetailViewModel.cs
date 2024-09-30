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

        //// Summary statistics
        //[Display(Name = "Total Fees Collected")]
        //[DisplayFormat(DataFormatString = "{0:C}")]
        //public int TotalFeesCollected => Expiations.Sum(e => e.TotalFee ?? 0);

        //[Display(Name = "Average Fees per Expiation")]
        //[DisplayFormat(DataFormatString = "{0:C}")]
        //public double AverageFee => Expiations.Count > 0 ? Expiations.Average(e => e.TotalFee ?? 0) : 0;

        //[Display(Name = "Expiation Status Frequency")]
        //public Dictionary<string, int> StatusFrequency => Expiations
        //    .GroupBy(e => e.Status)
        //    .ToDictionary(g => g.Key, g => g.Count());

        // Selected filters
        public string StatusFilter { get; set; }
        public int? Year { get; set; }


    }
}
