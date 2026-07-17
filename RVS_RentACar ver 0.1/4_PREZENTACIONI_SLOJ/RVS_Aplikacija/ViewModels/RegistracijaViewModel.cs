using System.ComponentModel.DataAnnotations;

namespace RVS_Aplikacija.ViewModels
{
    public class RegistracijaViewModel
    {
        [Required(ErrorMessage = "Име је обавезно.")]
        [StringLength(50, ErrorMessage = "Име не може бити дужи од 50 карактера.")]
        public string Ime { get; set; }

        [Required(ErrorMessage = "Презиме је обавезно.")]
        [StringLength(50, ErrorMessage = "Презиме не може бити дужи од 50 карактера.")]
        public string Prezime { get; set; }

        [Required(ErrorMessage = "Емаил је обавезан.")]
        [EmailAddress(ErrorMessage = "Унесите валидан емаил.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Лозинка је обавезна.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Лозинка мора имати минимум 6 карактера.")]
        [DataType(DataType.Password)]
        public string Lozinka { get; set; }

        [Required(ErrorMessage = "Потврда лозинке је обавезна.")]
        [DataType(DataType.Password)]
        [Compare("Lozinka", ErrorMessage = "Лозинка и потврда лозинке се не подударају.")]
        public string LozinkaPotvrda { get; set; }
    }
}
