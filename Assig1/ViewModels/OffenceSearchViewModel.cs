using Assig1.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Assig1.ViewModels

{
    public class OffenceSearchViewModel
    {
        [Display(Name  = "Search Text")]
        public string SearchText {  get; set; }

        [Display(Name = "Expiation Category")]
        public int? CategoryId { get; set; }

        [Display(Name = "Categories")]
        public SelectList CategoryList { get; set; }

        [Display(Name = "Offences")]
        public List<Offence> Offences { get; set; }



        //aggregate date

        [Display(Name = "Total offences found")]
        public int? TotalOffences { get; set; }

        [Display(Name = "Total fee collected")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public int? TotalFees { get; set; }

    }
}
