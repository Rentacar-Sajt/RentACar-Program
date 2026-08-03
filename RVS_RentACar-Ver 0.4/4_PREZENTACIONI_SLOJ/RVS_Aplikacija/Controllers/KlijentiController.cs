using Microsoft.AspNetCore.Mvc;
using RVS_Aplikacija.ViewModels;
using System.Net;
using System.Net.Http.Json;

namespace RVS_Aplikacija.Controllers
{
    public class KlijentiController : Controller
    {
        private readonly HttpClient _httpClient;

        public KlijentiController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClient =
                httpClientFactory.CreateClient(
                    "RestCrudServis"
                );
        }

        // GET: Klijenti
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                List<KlijentViewModel>? klijenti =
                    await _httpClient
                        .GetFromJsonAsync<List<KlijentViewModel>>(
                            "api/Klijenti"
                        );

                return View(
                    klijenti ??
                    new List<KlijentViewModel>()
                );
            }
            catch (HttpRequestException ex)
            {
                ViewBag.Greska =
                    "Nije moguće povezati se sa REST CRUD servisom. " +
                    ex.Message;

                return View(
                    new List<KlijentViewModel>()
                );
            }
        }

        // GET: Klijenti/Detalji/5
        [HttpGet]
        public async Task<IActionResult> Detalji(int id)
        {
            HttpResponseMessage odgovor =
                await _httpClient.GetAsync(
                    $"api/Klijenti/{id}"
                );

            if (odgovor.StatusCode ==
                HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            if (!odgovor.IsSuccessStatusCode)
            {
                string tekstGreske =
                    await odgovor.Content
                        .ReadAsStringAsync();

                return StatusCode(
                    (int)odgovor.StatusCode,
                    tekstGreske
                );
            }

            KlijentViewModel? klijent =
                await odgovor.Content
                    .ReadFromJsonAsync<KlijentViewModel>();

            if (klijent == null)
            {
                return NotFound();
            }

            return View(klijent);
        }

        // GET: Klijenti/Dodaj
        [HttpGet]
        public IActionResult Dodaj()
        {
            KlijentViewModel klijent =
                new KlijentViewModel
                {
                    DatumIzdavanjaDozvole =
                        DateTime.Today,

                    DatumIstekaVozackeDozvole =
                        DateTime.Today.AddYears(5)
                };

            return View(klijent);
        }

        // POST: Klijenti/Dodaj
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dodaj(
            KlijentViewModel klijent)
        {
            if (!ModelState.IsValid)
            {
                return View(klijent);
            }

            HttpResponseMessage odgovor =
                await _httpClient.PostAsJsonAsync(
                    "api/Klijenti",
                    klijent
                );

            string odgovorTekst =
                await odgovor.Content
                    .ReadAsStringAsync();

            if (odgovor.IsSuccessStatusCode)
            {
                TempData["Uspeh"] =
                    "Klijent je uspešno dodat.";

                return RedirectToAction(
                    nameof(Index)
                );
            }

            ModelState.AddModelError(
                string.Empty,
                $"API greška: " +
                $"{(int)odgovor.StatusCode} " +
                $"{odgovor.StatusCode} - " +
                odgovorTekst
            );

            return View(klijent);
        }

        // GET: Klijenti/Izmeni/5
        [HttpGet]
        public async Task<IActionResult> Izmeni(int id)
        {
            HttpResponseMessage odgovor =
                await _httpClient.GetAsync(
                    $"api/Klijenti/{id}"
                );

            if (odgovor.StatusCode ==
                HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            if (!odgovor.IsSuccessStatusCode)
            {
                string tekstGreske =
                    await odgovor.Content
                        .ReadAsStringAsync();

                return StatusCode(
                    (int)odgovor.StatusCode,
                    tekstGreske
                );
            }

            KlijentViewModel? klijent =
                await odgovor.Content
                    .ReadFromJsonAsync<KlijentViewModel>();

            if (klijent == null)
            {
                return NotFound();
            }

            return View(klijent);
        }

        // POST: Klijenti/Izmeni/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Izmeni(
            int id,
            KlijentViewModel klijent)
        {
            if (id != klijent.Id)
            {
                return BadRequest(
                    "ID iz rute se ne podudara " +
                    "sa ID-em klijenta."
                );
            }

            if (!ModelState.IsValid)
            {
                return View(klijent);
            }

            HttpResponseMessage odgovor =
                await _httpClient.PutAsJsonAsync(
                    $"api/Klijenti/{id}",
                    klijent
                );

            string odgovorTekst =
                await odgovor.Content
                    .ReadAsStringAsync();

            if (odgovor.IsSuccessStatusCode)
            {
                TempData["Uspeh"] =
                    "Klijent je uspešno izmenjen.";

                return RedirectToAction(
                    nameof(Index)
                );
            }

            ModelState.AddModelError(
                string.Empty,
                $"API greška: " +
                $"{(int)odgovor.StatusCode} " +
                $"{odgovor.StatusCode} - " +
                odgovorTekst
            );

            return View(klijent);
        }

        // GET: Klijenti/Obrisi/5
        [HttpGet]
        public async Task<IActionResult> Obrisi(int id)
        {
            HttpResponseMessage odgovor =
                await _httpClient.GetAsync(
                    $"api/Klijenti/{id}"
                );

            if (odgovor.StatusCode ==
                HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            if (!odgovor.IsSuccessStatusCode)
            {
                string tekstGreske =
                    await odgovor.Content
                        .ReadAsStringAsync();

                return StatusCode(
                    (int)odgovor.StatusCode,
                    tekstGreske
                );
            }

            KlijentViewModel? klijent =
                await odgovor.Content
                    .ReadFromJsonAsync<KlijentViewModel>();

            if (klijent == null)
            {
                return NotFound();
            }

            return View(klijent);
        }

        // POST: Klijenti/ObrisiPotvrda/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ObrisiPotvrda(
            int id)
        {
            HttpResponseMessage odgovor =
                await _httpClient.DeleteAsync(
                    $"api/Klijenti/{id}"
                );

            string odgovorTekst =
                await odgovor.Content
                    .ReadAsStringAsync();

            if (odgovor.IsSuccessStatusCode)
            {
                TempData["Uspeh"] =
                    "Klijent je uspešno obrisan.";

                return RedirectToAction(
                    nameof(Index)
                );
            }

            TempData["Greska"] =
                $"API greška: " +
                $"{(int)odgovor.StatusCode} " +
                $"{odgovor.StatusCode} - " +
                odgovorTekst;

            return RedirectToAction(
                nameof(Obrisi),
                new { id }
            );
        }
    }
}