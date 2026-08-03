using System.ComponentModel.DataAnnotations;

namespace RVS_Aplikacija.ViewModels
{
    public class StavkaUgovoraViewModel
    {
        public int Id { get; set; }

        public int UgovorId { get; set; }

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Morate izabrati vozilo."
        )]
        [Display(Name = "Vozilo")]
        public int VoziloId { get; set; }

        [Range(
            1,
            3650,
            ErrorMessage = "Broj dana mora biti veći od nule."
        )]
        [Display(Name = "Broj dana")]
        public int BrojDana { get; set; }

        [Range(
            typeof(decimal),
            "0.01",
            "999999999",
            ErrorMessage = "Cena po danu mora biti veća od nule."
        )]
        [Display(Name = "Cena po danu")]
        public decimal CenaPoDanu { get; set; }

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
        public decimal Ukupno { get; set; }

        public VoziloViewModel? VoziloObjekat { get; set; }
    }
}