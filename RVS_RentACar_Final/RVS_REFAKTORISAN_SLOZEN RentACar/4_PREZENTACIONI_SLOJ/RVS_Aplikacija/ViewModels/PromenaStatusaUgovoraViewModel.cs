using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace RVS_Aplikacija.ViewModels
{
    // Uloga klase: PromenaStatusaUgovoraViewModel grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
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

        [Display(Name = "Ugovoreni datum vraćanja")]
        public DateTime DatumVracanja { get; set; }

        [Display(Name = "Stvarni datum i vreme vraćanja")]
        public DateTime? StvarniDatumVracanja { get; set; }

        [Display(Name = "Broj naplaćenih dana kašnjenja")]
        public int BrojDanaKasnjenja { get; set; }

        [Display(Name = "Kazna za kašnjenje")]
        public decimal KaznaZaKasnjenje { get; set; }

        public List<SelectListItem> StatusiOpcije { get; set; }
            = new List<SelectListItem>();
    }
}
