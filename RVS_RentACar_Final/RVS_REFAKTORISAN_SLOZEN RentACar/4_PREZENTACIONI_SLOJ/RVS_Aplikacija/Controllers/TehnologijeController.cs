using Microsoft.AspNetCore.Mvc;
using BibliotekaKlasa.KlasePodatakaEF.ModeliEF;
using RVS_Aplikacija.ViewModels;
using BibliotekaKlasa.KlasePodatakaEF.RepozitorijumiEF;
using System.Linq;

namespace RVS_Aplikacija.Controllers
{
    // Uloga klase: TehnologijeController grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class TehnologijeController : Controller
    {
        private readonly TehnologijaRepo _tehnologijaRepoEFObjekat;

        // konstruktor sa DI
        public TehnologijeController(TehnologijaRepo tehnologijaRepoEFObjekat)
        {
            _tehnologijaRepoEFObjekat = tehnologijaRepoEFObjekat;
        }

        // GET: /Tehnologije
        // Učitava podatke potrebne za listu tehnologije i prosleđuje ih Index prikazu.
        public IActionResult Index(string filter)
        {
            var tehnologijeListaObjekata = string.IsNullOrWhiteSpace(filter)?
                _tehnologijaRepoEFObjekat.DajSve():_tehnologijaRepoEFObjekat.DajSve()
                .Where(tehnologija => tehnologija.NazivTehnologije.Contains(filter))
                .ToList();

            ViewData["Filter"] = filter;

            return View(tehnologijeListaObjekata);
        }

        // GET: /Tehnologije/Dodaj
        [HttpGet]
        // Otvara formu za dodavanje tehnologije i priprema početni model/podatke potrebne za prikaz forme.
        public IActionResult Dodaj()
        {
            return View(new TehnologijaViewModel());
        }

        // POST: /Tehnologije/Dodaj
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Prima podatke iz forme za dodavanje tehnologije, proverava validaciju i šalje zahtev servisu da sačuva novi zapis.
        public IActionResult Dodaj(TehnologijaViewModel tehnologijaViewModelObjekat)
        {
            if (!ModelState.IsValid)
                return View(tehnologijaViewModelObjekat);

            // mapiranje VM -> Entity
            var TehnologijaEFObjekat = new TehnologijaEntityModel
            {
                NazivTehnologije = tehnologijaViewModelObjekat.NazivTehnologije
            };

            _tehnologijaRepoEFObjekat.Dodaj(TehnologijaEFObjekat);

            TempData["Poruka"] = "Tehnologija je uspešno dodata!";
            TempData["PorukaTip"] = "uspeh";

            return RedirectToAction("Index");
        }

        // GET: /Tehnologije/Izmeni
        [HttpGet]
        // Učitava postojeće podatke tehnologije po ID-u i otvara formu za njihovu izmenu.
        public IActionResult Izmeni(int id)
        {
            var tehnologijaEFObjekat = _tehnologijaRepoEFObjekat.DajPoId(id);
            if (tehnologijaEFObjekat == null) return NotFound();

            var tehnologijaViewModelObjekat = new TehnologijaViewModel
            {
                Id = tehnologijaEFObjekat.Id,
                NazivTehnologije = tehnologijaEFObjekat.NazivTehnologije
            };

            return View(tehnologijaViewModelObjekat);
        }

        // POST: /Tehnologije/Izmeni
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Prima izmenjene podatke tehnologije, proverava validaciju i šalje zahtev servisu da ažurira zapis.
        public IActionResult Izmeni(TehnologijaViewModel tehnologijaViewModelObjekat)
        {
            if (!ModelState.IsValid)
                return View(tehnologijaViewModelObjekat);

            // mapiranje VM -> Entity
            var TehnologijaEFObjekat = new TehnologijaEntityModel
            {
                Id = tehnologijaViewModelObjekat.Id,
                NazivTehnologije = tehnologijaViewModelObjekat.NazivTehnologije
            };

            _tehnologijaRepoEFObjekat.Izmeni(TehnologijaEFObjekat);

            TempData["Poruka"] = "Tehnologija je uspešno izmenjena!";
            TempData["PorukaTip"] = "uspeh";

            return RedirectToAction("Index");
        }

        // POST: /Tehnologije/Obrisi
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Učitava podatke tehnologije koji treba obrisati i prikazuje stranicu za potvrdu brisanja.
        public IActionResult Obrisi(int id)
        {
            _tehnologijaRepoEFObjekat.Obrisi(id);

            TempData["Poruka"] = "Tehnologija je uspešno obrisana!";
            TempData["PorukaTip"] = "uspeh";

            return RedirectToAction("Index");
        }
    }
}
