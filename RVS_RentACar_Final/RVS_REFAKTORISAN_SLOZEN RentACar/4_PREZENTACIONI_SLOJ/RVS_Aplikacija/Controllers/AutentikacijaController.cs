using Microsoft.AspNetCore.Mvc;
using RVS_MVC.ViewModels;
using System.Net;
using System.Net.Http.Json;

namespace RVS_MVC.Controllers
{
    // Uloga klase: AutentikacijaController grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class AutentikacijaController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AutentikacijaController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        // Prikazuje formu za prijavu; ako je korisnik već prijavljen preusmerava ga na početnu stranicu.
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("KorisnikId") != null)
            {
                return RedirectToAction(
                    "Index",
                    "Pocetna"
                );
            }

            return View(new LoginModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Prima e-mail i lozinku iz forme, poziva servis za autentikaciju i čuva podatke prijavljenog korisnika u sesiji kada je prijava uspešna.
        public async Task<IActionResult> Login(
            LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                HttpClient httpClient =
                    _httpClientFactory.CreateClient(
                        "AutentikacijaServis"
                    );

                HttpResponseMessage odgovor =
                    await httpClient.PostAsJsonAsync(
                        "api/Autentikacija/Login",
                        model
                    );

                if (odgovor.StatusCode ==
                    HttpStatusCode.Unauthorized)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Email ili lozinka nisu ispravni."
                    );

                    return View(model);
                }

                if (odgovor.StatusCode ==
                    HttpStatusCode.Forbidden)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Nemate administratorska prava."
                    );

                    return View(model);
                }

                if (!odgovor.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Došlo je do greške prilikom prijave."
                    );

                    return View(model);
                }

                LoginOdgovor? korisnik =
                    await odgovor.Content
                        .ReadFromJsonAsync<LoginOdgovor>();

                if (korisnik == null)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "REST API nije vratio podatke korisnika."
                    );

                    return View(model);
                }

                HttpContext.Session.SetInt32(
                    "KorisnikId",
                    korisnik.Id
                );

                HttpContext.Session.SetString(
                    "ImePrezime",
                    $"{korisnik.Ime} {korisnik.Prezime}"
                        .Trim()
                );

                HttpContext.Session.SetString(
                    "Email",
                    korisnik.Email
                );

                HttpContext.Session.SetString(
                    "Uloga",
                    korisnik.Uloga
                );

                return RedirectToAction(
                    "Index",
                    "Pocetna"
                );
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Nije moguće povezivanje sa REST API servisom."
                );

                return View(model);
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Dogodila se neočekivana greška."
                );

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Briše podatke prijavljenog korisnika iz sesije i preusmerava na stranicu za prijavu.
        public IActionResult Odjava()
        {
            HttpContext.Session.Clear();

            return RedirectToAction(
                nameof(Login)
            );
        }
    }
}
