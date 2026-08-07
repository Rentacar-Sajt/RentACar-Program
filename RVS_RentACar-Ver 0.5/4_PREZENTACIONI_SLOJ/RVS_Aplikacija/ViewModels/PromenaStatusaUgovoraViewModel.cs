using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace RVS_Aplikacija.ViewModels
{
    public class PromenaStatusaUgovoraViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Broj ugovora")]
        public string BrojUgovora { get; set; } = string.Empty;

        [Display(Name = "Trenutni status")]
        public string TrenutniStatus { get; set; } = string.Empty;

        [Required(ErrorMessage = "Novi status je obavezan.")]
        [Display(Name = "Novi status")]
        public string NoviStatus { get; set; } = string.Empty;

        public List<SelectListItem> StatusiOpcije { get; set; }
            = new List<SelectListItem>();
    }
}