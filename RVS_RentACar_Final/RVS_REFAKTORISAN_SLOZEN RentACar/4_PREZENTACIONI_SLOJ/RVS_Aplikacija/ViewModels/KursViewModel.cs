using BibliotekaKlasa.KlasePodataka.Modeli;
using System.ComponentModel.DataAnnotations;
namespace RVS_Aplikacija.ViewModels;
// Uloga klase: KursViewModel grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
public class KursViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Назив курса је обавезан.")]
    [StringLength(100, ErrorMessage = "Назив курса не може бити дужи од 100 карактера.")]
    public string NazivKursa { get; set; }

    [Required(ErrorMessage = "Опис курса је обавезан.")]
    [StringLength(1000, ErrorMessage = "Опис курса не може бити дужи од 1000 карактера.")]
    public string OpisKursa { get; set; }

    public List<CasModel> Casovi { get; set; } = new();
}
