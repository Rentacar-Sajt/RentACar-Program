using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using RVS_Aplikacija.ViewModels;
using BibliotekaKlasa.KlasePodataka.Modeli;
using BibliotekaKlasa.KlasePodataka.Repozitorijumi;
using BibliotekaKlasa.TehnoloskeKlase.PomocneFunkcije;
using BibliotekaKlasa.TehnoloskeKlase;

namespace RVS_Aplikacija.Controllers.Nalog
{
    // Uloga klase: NalogController grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class NalogController : Controller
    {
        private readonly KorisnikRepo _korisnikRepo;
        private KonekcijaKlasa _konekcijaObjekat;

        public NalogController(IConfiguration konfiguracija)
        {
            string _konekcioniString = konfiguracija.GetConnectionString("KonekcioniString");
            _konekcijaObjekat = new KonekcijaKlasa(_konekcioniString);
            _korisnikRepo = new KorisnikRepo(_konekcijaObjekat);
        }

        // GET: /Nalog/Registracija
        [HttpGet]
        // Otvara formu za registraciju i prosleđuje prazan RegistracijaViewModel prikazu.
        public IActionResult Registracija()
        {
            return View(new RegistracijaViewModel());
        }

        // POST: /Nalog/Registracija
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Prima podatke iz forme za registraciju, proverava validaciju i kreira novi korisnički nalog ako su podaci ispravni.
        public IActionResult Registracija(RegistracijaViewModel registracijaViewModel)
        {
            if (!ModelState.IsValid)
                return View(registracijaViewModel);

            if (registracijaViewModel.Lozinka != registracijaViewModel.LozinkaPotvrda)
            {
                ModelState.AddModelError("", "Лозинке се не подударају.");
                return View(registracijaViewModel);
            }

            var postojeciKorisnikObjekat = _korisnikRepo.DajSve().FirstOrDefault(korisnik => korisnik.Email == registracijaViewModel.Email);
            if (postojeciKorisnikObjekat != null)
            {
                ModelState.AddModelError("", "Корисник са овим емаил-ом већ постоји!");
                return View(registracijaViewModel);
            }

            var salt = FunkcijeLozinke.GenerisiSalt();
            var hash = FunkcijeLozinke.IzracunajHash(registracijaViewModel.Lozinka, salt);

            var korisnikObjekat = new KorisnikModel
            {
                Ime = registracijaViewModel.Ime,
                Prezime = registracijaViewModel.Prezime,
                Email = registracijaViewModel.Email,
                LozinkaSalt = salt,
                LozinkaHash = hash,
                Uloga = "Korisnik"
            };

            _korisnikRepo.Dodaj(korisnikObjekat);

            TempData["Poruka"] = "Регистрација је успешна! Можете се пријавити!";
            return RedirectToAction("Prijava");
        }

        // GET: /Nalog/Prijava
        [HttpGet]
        // Otvara formu za prijavu korisnika i prosleđuje prazan PrijavaViewModel prikazu.
        public IActionResult Prijava()
        {
            return View(new PrijavaViewModel());
        }

        // POST: /Nalog/Prijava
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Prima e-mail i lozinku iz forme, proverava korisnika i lozinku, a nakon uspešne prijave kreira autentikacioni cookie.
        public async Task<IActionResult> Prijava(PrijavaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var korisnikObjekat = _korisnikRepo.DajSve().FirstOrDefault(k => k.Email == model.Email);
            if (korisnikObjekat == null || !FunkcijeLozinke.ProveriLozinku(model.Lozinka, korisnikObjekat.LozinkaSalt, korisnikObjekat.LozinkaHash))
            {
                ModelState.AddModelError("", "Неисправан емаил или лозинка.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, korisnikObjekat.Ime),
                new Claim(ClaimTypes.Email, korisnikObjekat.Email),
                new Claim(ClaimTypes.Role, korisnikObjekat.Uloga),
                new Claim(ClaimTypes.NameIdentifier, korisnikObjekat.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            HttpContext.Session.SetString("KorisnikEmail", korisnikObjekat.Email);
            HttpContext.Session.SetString("KorisnikUloga", korisnikObjekat.Uloga);

            return RedirectToAction("Index", "Pocetna");
        }

        // POST: /Nalog/OdjaviSe
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Odjavljuje korisnika iz aplikacije, uklanja autentikacionu sesiju i vraća ga na početnu stranicu.
        public async Task<IActionResult> OdjaviSe()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Pocetna");
        }
    }
}
