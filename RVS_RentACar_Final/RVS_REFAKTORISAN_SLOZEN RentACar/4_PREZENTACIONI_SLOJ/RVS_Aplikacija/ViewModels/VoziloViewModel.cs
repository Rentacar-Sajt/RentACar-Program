using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RVS_Aplikacija.ViewModels
{
    // Uloga klase: VoziloViewModel grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
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

        [Range(0.01, double.MaxValue, ErrorMessage = "Cena po danu mora biti veća od nule.")]
        [Display(Name = "Cena po danu")]
        public decimal CenaPoDanu { get; set; }

        [Required(ErrorMessage = "Status vozila je obavezan.")]
        [Display(Name = "Status vozila")]
        public string StatusVozila { get; set; } = string.Empty;

        [Range(1950, 2100, ErrorMessage = "Godište mora biti između 1950. i 2100. godine.")]
        [Display(Name = "Godište")]
        public int? Godiste { get; set; }

        [StringLength(30)]
        public string? Gorivo { get; set; }

        [StringLength(30)]
        [Display(Name = "Menjač")]
        public string? Menjac { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Kilometraža ne može biti negativna.")]
        [Display(Name = "Kilometraža")]
        public int? Kilometraza { get; set; }

        [StringLength(30)]
        public string? Boja { get; set; }

        [Range(1, 100, ErrorMessage = "Broj sedišta mora biti veći od nule.")]
        [Display(Name = "Broj sedišta")]
        public int? BrojSedista { get; set; }

        [Range(0.1, 20, ErrorMessage = "Zapremina motora mora biti između 0,1 i 20 litara.")]
        [Display(Name = "Zapremina motora (l)")]
        public decimal? ZapreminaMotora { get; set; }

        [Range(1, 2000, ErrorMessage = "Snaga motora mora biti veća od nule.")]
        [Display(Name = "Snaga motora (kW)")]
        public int? SnagaMotora { get; set; }

        [StringLength(300)]
        public string? SlikaPutanja { get; set; }

        [Display(Name = "Fotografija vozila")]
        [JsonIgnore]
        public IFormFile? SlikaFajl { get; set; }
    }
}
