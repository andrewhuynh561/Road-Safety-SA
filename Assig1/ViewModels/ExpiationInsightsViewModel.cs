using System.ComponentModel.DataAnnotations;

namespace Assig1.ViewModels
{
    public class ExpiationInsightsViewModel
    {

        public string StatusFilter { get; set; }
        public DateOnly? Year { get; set; }

        [Display(Name="Maximum Fine")]
        [DisplayFormat(DataFormatString ="{0:C}")]
        public int MaxFine {  get; set; }


        [Display(Name = "Min Fine")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public int MinFine { get; set; }

        [Display(Name = "Average Fine")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double AvgFine { get; set; }

        [Display(Name = "Minimum BAC Level")]
        public decimal? MinBAC { get; set; }

        [Display(Name = "Maximum BAC Level")]
        public decimal? MaxBAC { get; set; }

        [Display(Name = "Average BAC Level")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal? AvgBAC { get; set; }

        public int MaxSpeed { get; set; }

        public double AvgSpeed { get; set; }


        [Display(Name = "Frequency")]
        public int Frequency { get; set; }
    }
}
