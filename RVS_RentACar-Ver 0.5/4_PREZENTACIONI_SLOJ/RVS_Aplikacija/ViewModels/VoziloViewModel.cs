using System.ComponentModel.DataAnnotations;

namespace RVS_Aplikacija.ViewModels
{
    public class VoziloViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Marka vozila je obavezna.")]
        [StringLength(50)]
        public string Marka { get; set; } = string.Empty;

        [Required(ErrorMessage = "Model vozila je obavezan.")]
        [StringLength(50)]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "Registracija je obavezna.")]
        [StringLength(20)]
        public string Registracija { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue,
            ErrorMessage = "Cena po danu mora biti veća od nule.")]
        public decimal CenaPoDanu { get; set; }

        [Required(ErrorMessage = "Status vozila je obavezan.")]
        public string StatusVozila { get; set; } = string.Empty;
    }
}