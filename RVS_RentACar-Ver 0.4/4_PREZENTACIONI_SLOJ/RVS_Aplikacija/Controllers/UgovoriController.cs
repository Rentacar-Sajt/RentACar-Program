using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RVS_Aplikacija.ViewModels;
using System.Net;
using System.Net.Http.Json;

namespace RVS_Aplikacija.Controllers
{
    public class UgovoriController : Controller
    {
        private readonly HttpClient _httpClient;

        public UgovoriController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(
                "RestCrudServis"
            );
        }

        // GET: Ugovori
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                List<UgovorViewModel> ugovori =
                    await _httpClient.GetFromJsonAsync<
                        List<UgovorViewModel>>(
                            "api/Ugovori"
                        )
                    ?? new List<UgovorViewModel>();

                List<KlijentViewModel> klijenti =
                    await _httpClient.GetFromJsonAsync<
                        List<KlijentViewModel>>(
                            "api/Klijenti"
                        )
                    ?? new List<KlijentViewModel>();

                List<KorisnikViewModel> korisnici =
                    await _httpClient.GetFromJsonAsync<
                        List<KorisnikViewModel>>(
                            "api/Korisnici"
                        )
                    ?? new List<KorisnikViewModel>();

                foreach (UgovorViewModel ugovor in ugovori)
                {
                    ugovor.KlijentObjekat =
                        klijenti.FirstOrDefault(
                            k => k.Id == ugovor.KlijentId
                        );

                    ugovor.KorisnikObjekat =
                        korisnici.FirstOrDefault(
                            k => k.Id == ugovor.KorisnikId
                        );
                }

                return View(ugovori);
            }
            catch (HttpRequestException ex)
            {
                ViewBag.Greska =
                    "Nije moguće povezati se sa REST CRUD servisom. " +
                    ex.Message;

                return View(
                    new List<UgovorViewModel>()
                );
            }
        }

        // GET: Ugovori/Detalji/5
        [HttpGet]
        public async Task<IActionResult> Detalji(int id)
        {
            HttpResponseMessage odgovor =
                await _httpClient.GetAsync(
                    $"api/Ugovori/{id}"
                );

            if (odgovor.StatusCode ==
                HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            if (!odgovor.IsSuccessStatusCode)
            {
                string tekstGreske =
                    await odgovor.Content.ReadAsStringAsync();

                return StatusCode(
                    (int)odgovor.StatusCode,
                    tekstGreske
                );
            }

            UgovorViewModel? ugovor =
                await odgovor.Content
                    .ReadFromJsonAsync<UgovorViewModel>();

            if (ugovor == null)
            {
                return NotFound();
            }

            try
            {
                KlijentViewModel? klijent =
                    await _httpClient.GetFromJsonAsync<
                        KlijentViewModel>(
                            $"api/Klijenti/{ugovor.KlijentId}"
                        );

                ugovor.KlijentObjekat = klijent;
            }
            catch
            {
                ugovor.KlijentObjekat = null;
            }

            try
            {
                KorisnikViewModel? korisnik =
                    await _httpClient.GetFromJsonAsync<
                        KorisnikViewModel>(
                            $"api/Korisnici/{ugovor.KorisnikId}"
                        );

                ugovor.KorisnikObjekat = korisnik;
            }
            catch
            {
                ugovor.KorisnikObjekat = null;
            }

            return View(ugovor);
        }

        // GET: Ugovori/Dodaj
        [HttpGet]
        public async Task<IActionResult> Dodaj()
        {
            UgovorViewModel ugovor =
                new UgovorViewModel
                {
                    DatumIzdavanja = DateTime.Today,

                    DatumPreuzimanja =
                        DateTime.Today.AddDays(1)
                            .AddHours(10),

                    DatumVracanja =
                        DateTime.Today.AddDays(4)
                            .AddHours(10),

                    StatusUgovora = "Aktivan",

                    StavkeUgovora =
                        new List<StavkaUgovoraViewModel>
                        {
                            new StavkaUgovoraViewModel
                            {
                                BrojDana = 3,
                                PopustProcenat = 0
                            }
                        }
                };

            await PopuniPadajuceListe(ugovor);

            return View(ugovor);
        }

        // POST: Ugovori/Dodaj
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dodaj(
            UgovorViewModel ugovor)
        {
            if (ugovor.DatumVracanja <=
                ugovor.DatumPreuzimanja)
            {
                ModelState.AddModelError(
                    nameof(ugovor.DatumVracanja),
                    "Datum vraćanja mora biti posle " +
                    "datuma preuzimanja."
                );
            }

            if (ugovor.StavkeUgovora == null ||
                ugovor.StavkeUgovora.Count == 0)
            {
                ModelState.AddModelError(
                    nameof(ugovor.StavkeUgovora),
                    "Ugovor mora imati najmanje jednu stavku."
                );
            }

            if (ugovor.StavkeUgovora != null &&
    ugovor.StavkeUgovora.Count > 0)
            {
                IzracunajStavkeIUgovor(ugovor);
                for (int i = 0; i < ugovor.StavkeUgovora.Count; i++)
                {
                    ModelState.Remove(
                        $"StavkeUgovora[{i}].BrojDana"
                    );

                    ModelState.Remove(
                        $"StavkeUgovora[{i}].Ukupno"
                    );
                }

                ModelState.Remove(
                    nameof(ugovor.UkupnoZaPlacanje)
                );
            }

            if (!ModelState.IsValid)
            {
                await PopuniPadajuceListe(ugovor);
                return View(ugovor);
            }

            HttpResponseMessage odgovor =
                await _httpClient.PostAsJsonAsync(
                    "api/Ugovori",
                    ugovor
                );

            string odgovorTekst =
                await odgovor.Content.ReadAsStringAsync();

            if (odgovor.IsSuccessStatusCode)
            {
                TempData["Uspeh"] =
                    "Ugovor je uspešno dodat.";

                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(
                string.Empty,
                $"API greška: {(int)odgovor.StatusCode} " +
                $"{odgovor.StatusCode} - {odgovorTekst}"
            );

            await PopuniPadajuceListe(ugovor);

            return View(ugovor);
        }

        // GET: Ugovori/PromeniStatus/5
        [HttpGet]
        public async Task<IActionResult> PromeniStatus(int id)
        {
            HttpResponseMessage odgovor =
                await _httpClient.GetAsync(
                    $"api/Ugovori/{id}"
                );

            if (odgovor.StatusCode ==
                HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            if (!odgovor.IsSuccessStatusCode)
            {
                string tekstGreske =
                    await odgovor.Content.ReadAsStringAsync();

                return StatusCode(
                    (int)odgovor.StatusCode,
                    tekstGreske
                );
            }

            UgovorViewModel? ugovor =
                await odgovor.Content
                    .ReadFromJsonAsync<UgovorViewModel>();

            if (ugovor == null)
            {
                return NotFound();
            }

            PromenaStatusaUgovoraViewModel model =
                new PromenaStatusaUgovoraViewModel
                {
                    Id = ugovor.Id,
                    BrojUgovora = ugovor.BrojUgovora,
                    TrenutniStatus = ugovor.StatusUgovora,
                    NoviStatus = ugovor.StatusUgovora
                };

            PopuniStatuse(model);

            return View(model);
        }

        // POST: Ugovori/PromeniStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PromeniStatus(
            int id,
            PromenaStatusaUgovoraViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest(
                    "ID iz rute se ne podudara sa ID-em ugovora."
                );
            }

            string[] dozvoljeniStatusi =
            {
                "Aktivan",
                "Završen",
                "Otkazan"
            };

            if (!dozvoljeniStatusi.Contains(
                    model.NoviStatus,
                    StringComparer.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(
                    nameof(model.NoviStatus),
                    "Dozvoljeni statusi su: " +
                    "Aktivan, Završen i Otkazan."
                );
            }

            if (!ModelState.IsValid)
            {
                PopuniStatuse(model);
                return View(model);
            }

            HttpResponseMessage odgovor =
                await _httpClient.PutAsJsonAsync(
                    $"api/Ugovori/{id}/status",
                    new
                    {
                        NoviStatus = model.NoviStatus
                    }
                );

            string odgovorTekst =
                await odgovor.Content.ReadAsStringAsync();

            if (odgovor.IsSuccessStatusCode)
            {
                TempData["Uspeh"] =
                    "Status ugovora je uspešno promenjen.";

                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(
                string.Empty,
                $"API greška: {(int)odgovor.StatusCode} " +
                $"{odgovor.StatusCode} - {odgovorTekst}"
            );

            PopuniStatuse(model);

            return View(model);
        }

        // GET: Ugovori/Obrisi/5
        [HttpGet]
        public async Task<IActionResult> Obrisi(int id)
        {
            HttpResponseMessage odgovor =
                await _httpClient.GetAsync(
                    $"api/Ugovori/{id}"
                );

            if (odgovor.StatusCode ==
                HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            if (!odgovor.IsSuccessStatusCode)
            {
                string tekstGreske =
                    await odgovor.Content.ReadAsStringAsync();

                return StatusCode(
                    (int)odgovor.StatusCode,
                    tekstGreske
                );
            }

            UgovorViewModel? ugovor =
                await odgovor.Content
                    .ReadFromJsonAsync<UgovorViewModel>();

            if (ugovor == null)
            {
                return NotFound();
            }

            return View(ugovor);
        }

        // POST: Ugovori/ObrisiPotvrda/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ObrisiPotvrda(
            int id)
        {
            HttpResponseMessage odgovor =
                await _httpClient.DeleteAsync(
                    $"api/Ugovori/{id}"
                );

            string odgovorTekst =
                await odgovor.Content.ReadAsStringAsync();

            if (odgovor.IsSuccessStatusCode)
            {
                TempData["Uspeh"] =
                    "Ugovor je uspešno obrisan.";

                return RedirectToAction(nameof(Index));
            }

            TempData["Greska"] =
                $"API greška: {(int)odgovor.StatusCode} " +
                $"{odgovor.StatusCode} - {odgovorTekst}";

            return RedirectToAction(
                nameof(Obrisi),
                new { id }
            );
        }

        private async Task PopuniPadajuceListe(
            UgovorViewModel ugovor)
        {
            try
            {
                List<KlijentViewModel>? klijenti =
                    await _httpClient.GetFromJsonAsync<
                        List<KlijentViewModel>>(
                            "api/Klijenti"
                        );

                ugovor.KlijentiOpcije =
                    klijenti?
                        .Select(k => new SelectListItem
                        {
                            Value = k.Id.ToString(),
                            Text = $"{k.Ime} {k.Prezime}",
                            Selected = k.Id == ugovor.KlijentId
                        })
                        .ToList()
                    ?? new List<SelectListItem>();
            }
            catch
            {
                ugovor.KlijentiOpcije =
                    new List<SelectListItem>();
            }

            try
            {
                List<VoziloViewModel>? vozila =
                    await _httpClient.GetFromJsonAsync<
                        List<VoziloViewModel>>(
                            "api/Vozila"
                        );

                ugovor.VozilaOpcije =
                    vozila?
                        .Select(v => new SelectListItem
                        {
                            Value = v.Id.ToString(),
                            Text = NapraviNazivVozila(v),

                            Selected =
                                ugovor.StavkeUgovora.Any(
                                    s => s.VoziloId == v.Id
                                )
                        })
                        .ToList()
                    ?? new List<SelectListItem>();
            }
            catch
            {
                ugovor.VozilaOpcije =
                    new List<SelectListItem>();
            }

            try
            {
                // Pretpostavljena REST ruta za korisnike.
                List<KorisnikViewModel>? korisnici =
                    await _httpClient.GetFromJsonAsync<
                        List<KorisnikViewModel>>(
                            "api/Korisnici"
                        );

                ugovor.KorisniciOpcije =
                    korisnici?
                        .Select(k => new SelectListItem
                        {
                            Value = k.Id.ToString(),
                            Text = string.IsNullOrWhiteSpace(
                                k.PunoIme)
                                    ? k.Email
                                    : $"{k.PunoIme} ({k.Uloga})",

                            Selected =
                                k.Id == ugovor.KorisnikId
                        })
                        .ToList()
                    ?? new List<SelectListItem>();
            }
            catch
            {
                ugovor.KorisniciOpcije =
                    new List<SelectListItem>();
            }
        }

        private static void IzracunajStavkeIUgovor(
            UgovorViewModel ugovor)
        {


            int brojDana =
                (int)Math.Ceiling(
                    (ugovor.DatumVracanja -
                     ugovor.DatumPreuzimanja).TotalDays
                );

            if (brojDana < 1)
            {
                brojDana = 1;
            }

            foreach (StavkaUgovoraViewModel stavka
                     in ugovor.StavkeUgovora)
            {
                stavka.BrojDana = brojDana;

                decimal osnovnaCena =
                    stavka.BrojDana *
                    stavka.CenaPoDanu;

                decimal iznosPopusta =
                    osnovnaCena *
                    stavka.PopustProcenat /
                    100m;

                stavka.Ukupno =
                    osnovnaCena - iznosPopusta;
            }

            decimal zbirStavki =
                ugovor.StavkeUgovora.Sum(
                    s => s.Ukupno
                );

            decimal popustNaUgovor =
                zbirStavki *
                ugovor.PopustProcenat /
                100m;

            ugovor.UkupnoZaPlacanje =
                zbirStavki -
                popustNaUgovor;


        }

        private static void PopuniStatuse(
            PromenaStatusaUgovoraViewModel model)
        {
            model.StatusiOpcije =
                new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = "Aktivan",
                        Text = "Aktivan"
                    },
                    new SelectListItem
                    {
                        Value = "Završen",
                        Text = "Završen"
                    },
                    new SelectListItem
                    {
                        Value = "Otkazan",
                        Text = "Otkazan"
                    }
                };
        }

        private static string NapraviNazivVozila(
            VoziloViewModel vozilo)
        {
            Type tip = vozilo.GetType();

            string marka =
                tip.GetProperty("Marka")
                    ?.GetValue(vozilo)
                    ?.ToString()
                ?? string.Empty;

            string model =
                tip.GetProperty("Model")
                    ?.GetValue(vozilo)
                    ?.ToString()
                ?? string.Empty;

            string registracija =
                tip.GetProperty("Registracija")
                    ?.GetValue(vozilo)
                    ?.ToString()
                ?? tip.GetProperty("RegistarskaOznaka")
                    ?.GetValue(vozilo)
                    ?.ToString()
                ?? string.Empty;

            string naziv =
                $"{marka} {model}".Trim();

            if (!string.IsNullOrWhiteSpace(registracija))
            {
                naziv += $" - {registracija}";
            }

            if (string.IsNullOrWhiteSpace(naziv))
            {
                naziv = $"Vozilo ID: {vozilo.Id}";
            }

            return naziv;
        }
    }
}