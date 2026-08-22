using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RVS_Aplikacija.Views.Vozila
{
    // Uloga klase: IzmeniModel grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class IzmeniModel : PageModel
    {
        // OnGet se poziva kada se otvori Razor stranica Vozila/Izmeni; ovde nema dodatnog učitavanja podataka pre prikaza stranice.
        public void OnGet()
        {
        }
    }
}
