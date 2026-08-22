using Microsoft.AspNetCore.Mvc;

namespace RVS_Aplikacija.Controllers
{
    // Uloga klase: TehnologijeAPIController grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class TehnologijeAPIController : Controller
    {
        // Učitava podatke potrebne za listu tehnologijeapi i prosleđuje ih Index prikazu.
        public IActionResult Index()
        {
            return View();
        }

        // Prima podatke iz forme za dodavanje tehnologijeapi, proverava validaciju i šalje zahtev servisu da sačuva novi zapis.
        public IActionResult Dodaj()
        {
            return View();
        }
        // Prima izmenjene podatke tehnologijeapi, proverava validaciju i šalje zahtev servisu da ažurira zapis.
        public IActionResult Izmeni(int id)
        {
            // prosledjuje se samo ID view-u, REST ide preko JS
            ViewData["Id"] = id;
            return View();
        }
    }
}
