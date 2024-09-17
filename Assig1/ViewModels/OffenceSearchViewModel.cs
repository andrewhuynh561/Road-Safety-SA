using Assig1.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Assig1.ViewModels

{
    public class OffenceSearchViewModel
    {
        public string SearchText {  get; set; }

        public int? CategoryId { get; set; }

        public SelectList CategoryList { get; set; }

        public List<Offence> Offences { get; set; }
    }
}
