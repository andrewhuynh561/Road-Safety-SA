using System.ComponentModel.DataAnnotations;

namespace Assig1.ViewModels
{
    public class ExpiationInsightsViewModel
    {
        [Display(Name="Maximum Fine")]
        [DisplayFormat(DataFormatString ="{0:C}")]
        public int MaxFine {  get; set; }


        [Display(Name = "Min Fine")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public int MinFine { get; set; }

        public double AvgFine { get; set; }

        public int MaxSpeed { get; set; }

        public int MinSpeed { get; set; }


        [Display(Name = "Frequency")]
        public int Frequency { get; set; }
    }
}
