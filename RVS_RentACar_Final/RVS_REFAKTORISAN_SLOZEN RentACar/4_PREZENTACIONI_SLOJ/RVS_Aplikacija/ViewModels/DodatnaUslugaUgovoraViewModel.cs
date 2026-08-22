using System.ComponentModel.DataAnnotations;

namespace RVS_Aplikacija.ViewModels
{
    // Uloga klase: DodatnaUslugaUgovoraViewModel grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class DodatnaUslugaUgovoraViewModel
    {
        public int Id { get; set; }

        public int DodatnaUslugaId { get; set; }

        public string NazivUsluge { get; set; } = string.Empty;

        public bool Izabrana { get; set; }

        [Range(
            typeof(decimal),
            "0",
            "999999999",
            ErrorMessage = "Cena ne može biti negativna."
        )]
        [Display(Name = "Cena po danu")]
        public decimal CenaPoDanu { get; set; }

        [Range(
            1,
            10000,
            ErrorMessage = "Broj dana mora biti najmanje 1."
        )]
        [Display(Name = "Broj dana")]
        public int BrojDana { get; set; }

        [Display(Name = "Ukupno")]
        public decimal Ukupno { get; set; }
    }
}
