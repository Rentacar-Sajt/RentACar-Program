using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace RVS_Aplikacija.ViewModels
{
    public class UgovorViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Broj ugovora je obavezan.")]
        [Display(Name = "Broj ugovora")]
        public string BrojUgovora { get; set; } = string.Empty;

        [Required(ErrorMessage = "Datum izdavanja je obavezan.")]
        [DataType(DataType.Date)]
        [Display(Name = "Datum izdavanja")]
        public DateTime DatumIzdavanja { get; set; }

        [Required(ErrorMessage = "Datum preuzimanja je obavezan.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Datum preuzimanja")]
        public DateTime DatumPreuzimanja { get; set; }

        [Required(ErrorMessage = "Datum vraćanja je obavezan.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Datum vraćanja")]
        public DateTime DatumVracanja { get; set; }

        [Required(ErrorMessage = "Mesto preuzimanja je obavezno.")]
        [Display(Name = "Mesto preuzimanja")]
        public string MestoPreuzimanja { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mesto vraćanja je obavezno.")]
        [Display(Name = "Mesto vraćanja")]
        public string MestoVracanja { get; set; } = string.Empty;

        [Required(ErrorMessage = "Način plaćanja je obavezan.")]
        [Display(Name = "Način plaćanja")]
        public string NacinPlacanja { get; set; } = string.Empty;

        [Range(
            typeof(decimal),
            "0",
            "999999999",
            ErrorMessage = "Depozit ne može biti negativan."
        )]
        public decimal Depozit { get; set; }

        [Required(ErrorMessage = "Status ugovora je obavezan.")]
        [Display(Name = "Status ugovora")]
        public string StatusUgovora { get; set; } = "Aktivan";

        public string? Napomena { get; set; }

        [Range(
            typeof(decimal),
            "0",
            "100",
            ErrorMessage = "Popust mora biti između 0 i 100%."
        )]
        [Display(Name = "Popust (%)")]
        public decimal PopustProcenat { get; set; }

        [Range(
            typeof(decimal),
            "0",
            "999999999",
            ErrorMessage = "Ukupan iznos ne može biti negativan."
        )]
        [Display(Name = "Ukupno za plaćanje")]
        public decimal UkupnoZaPlacanje { get; set; }

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Morate izabrati klijenta."
        )]
        [Display(Name = "Klijent")]
        public int KlijentId { get; set; }

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Korisnik je obavezan."
        )]
        [Display(Name = "Korisnik")]
        public int KorisnikId { get; set; }


        public KlijentViewModel? KlijentObjekat { get; set; }

        public KorisnikViewModel? KorisnikObjekat { get; set; }

        public List<StavkaUgovoraViewModel> StavkeUgovora { get; set; }
            = new List<StavkaUgovoraViewModel>();

        // Padajuće liste koje se koriste samo u MVC formi.
        public List<SelectListItem> KlijentiOpcije { get; set; }
            = new List<SelectListItem>();

        public List<SelectListItem> VozilaOpcije { get; set; }
            = new List<SelectListItem>();

        public List<SelectListItem> KorisniciOpcije { get; set; }
            = new List<SelectListItem>();
    }
}