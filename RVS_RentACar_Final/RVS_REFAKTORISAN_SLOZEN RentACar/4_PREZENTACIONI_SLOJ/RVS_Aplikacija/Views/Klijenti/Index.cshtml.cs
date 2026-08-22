using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RVS_Aplikacija.Views.Klijenti
{
    // Uloga klase: IndexModel grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class IndexModel : PageModel
    {
        // OnGet se poziva kada se otvori Razor stranica Klijenti/Index; ovde nema dodatnog učitavanja podataka pre prikaza stranice.
        public void OnGet()
        {
        }
    }
}
