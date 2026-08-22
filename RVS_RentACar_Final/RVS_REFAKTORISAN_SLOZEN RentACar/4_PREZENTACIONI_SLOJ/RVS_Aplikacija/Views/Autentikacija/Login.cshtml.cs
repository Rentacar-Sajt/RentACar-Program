using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RVS_Aplikacija.Views.Autentikacija
{
    // Uloga klase: LoginModel grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class LoginModel : PageModel
    {
        // OnGet se poziva kada se otvori Razor stranica Autentikacija/Login; ovde nema dodatnog učitavanja podataka pre prikaza stranice.
        public void OnGet()
        {
        }
    }
}
