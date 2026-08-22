using System.ComponentModel.DataAnnotations;

namespace RVS_Aplikacija.ViewModels
{
    // Uloga klase: TehnologijaViewModel grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class TehnologijaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Назив технологије је обавезан!")]
        [StringLength(64, ErrorMessage = "Назив технологије може имати највише 64 карактера!")]
        public string NazivTehnologije { get; set; }
    }
}
