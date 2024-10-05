using System.ComponentModel.DataAnnotations;

namespace Assig1.ViewModels
{
    public class ExpiationInsightsViewModel
    {

        public string StatusFilter { get; set; }
        public DateOnly? Year { get; set; }
        public DateOnly? Month { get; set; }

        [Display(Name="Maximum Fine")]
        [DisplayFormat(DataFormatString ="{0:C}")]
        public int MaxFine {  get; set; }


        [Display(Name = "Min Fine")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public int MinFine { get; set; }

        public double AvgFine { get; set; }

        public int MaxSpeed { get; set; }

        public double AvgSpeed { get; set; }


        [Display(Name = "Frequency")]
        public int Frequency { get; set; }
    }
}
