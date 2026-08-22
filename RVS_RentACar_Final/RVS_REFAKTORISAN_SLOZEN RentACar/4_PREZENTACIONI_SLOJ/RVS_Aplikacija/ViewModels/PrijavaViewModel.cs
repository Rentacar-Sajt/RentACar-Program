using System.ComponentModel.DataAnnotations;

namespace RVS_Aplikacija.ViewModels
{
    // Uloga klase: PrijavaViewModel grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class PrijavaViewModel
    {
        [Required(ErrorMessage = "Емаил је обавезан.")]
        [EmailAddress(ErrorMessage = "Унесите валидан емаил.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Лозинка је обавезна.")]
        [DataType(DataType.Password)]
        public string Lozinka { get; set; }
    }
}
