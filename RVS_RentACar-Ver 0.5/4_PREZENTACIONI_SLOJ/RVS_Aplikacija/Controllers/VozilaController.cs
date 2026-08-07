using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using RVS_Aplikacija.ViewModels;
using RVS_MVC.Filteri;

namespace RVS_Aplikacija.Controllers
{
    [AdminAutorizacijaFilter]
    public class VozilaController : Controller
    {
        private readonly HttpClient _httpClient;

        public VozilaController(IHttpClientFactory httpClientFactory)
        {
            _httpClient =
                httpClientFactory.CreateClient("RestCrudServis");
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? status)
        {
            try
            {
                List<VoziloViewModel> vozila =
                    await _httpClient
                        .GetFromJsonAsync<List<VoziloViewModel>>(
                            "api/Vozila"
                        )
                    ?? new List<VoziloViewModel>();

                if (!string.IsNullOrWhiteSpace(status))
                {
                    vozila = vozila
                        .Where(v =>
                            string.Equals(
                                v.StatusVozila,
                                status,
                                StringComparison.OrdinalIgnoreCase
                            )
                        )
                        .ToList();
                }

                ViewBag.IzabraniStatus = status;

                return View(vozila);
            }
            catch (HttpRequestException ex)
            {
                ViewBag.Greska =
                    "Nije moguće povezati se sa REST CRUD servisom. " +
                    ex.Message;

                return View(new List<VoziloViewModel>());
            }
        }

        [HttpGet]
        public IActionResult Dodaj()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dodaj(VoziloViewModel vozilo)
        {
            if (!ModelState.IsValid)
            {
                return View(vozilo);
            }

            HttpResponseMessage odgovor =
                await _httpClient.PostAsJsonAsync(
                    "api/Vozila",
                    vozilo
                );

            string odgovorTekst =
                await odgovor.Content.ReadAsStringAsync();

            if (odgovor.IsSuccessStatusCode)
            {
                TempData["Uspeh"] =
                    "Vozilo je uspešno dodato.";

                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(
                string.Empty,
                $"API greška: {(int)odgovor.StatusCode} " +
                $"{odgovor.StatusCode} - {odgovorTekst}"
            );

            return View(vozilo);
        }

        [HttpGet]
        public async Task<IActionResult> Detalji(int id)
        {
            HttpResponseMessage odgovor =
                await _httpClient.GetAsync($"api/Vozila/{id}");

            if (odgovor.StatusCode ==
                System.Net.HttpStatusCode.NotFound)
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

            VoziloViewModel? vozilo =
                await odgovor.Content
                    .ReadFromJsonAsync<VoziloViewModel>();

            if (vozilo == null)
            {
                return NotFound();
            }

            return View(vozilo);
        }

        [HttpGet]
        public async Task<IActionResult> Izmeni(int id)
        {
            HttpResponseMessage odgovor =
                await _httpClient.GetAsync($"api/Vozila/{id}");

            if (odgovor.StatusCode ==
                System.Net.HttpStatusCode.NotFound)
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

            VoziloViewModel? vozilo =
                await odgovor.Content
                    .ReadFromJsonAsync<VoziloViewModel>();

            if (vozilo == null)
            {
                return NotFound();
            }

            return View(vozilo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Izmeni(
    int id,
    VoziloViewModel vozilo)
        {
            if (id != vozilo.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(vozilo);
            }

            HttpResponseMessage odgovor =
                await _httpClient.PutAsJsonAsync(
                    $"api/Vozila/{id}",
                    vozilo
                );

            string odgovorTekst =
                await odgovor.Content.ReadAsStringAsync();

            if (odgovor.IsSuccessStatusCode)
            {
                TempData["Uspeh"] =
                    "Vozilo je uspešno izmenjeno.";

                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(
                string.Empty,
                $"API greška: {(int)odgovor.StatusCode} " +
                $"{odgovor.StatusCode} - {odgovorTekst}"
            );

            return View(vozilo);
        }

        [HttpGet]
        public async Task<IActionResult> Obrisi(int id)
        {
            HttpResponseMessage odgovor =
                await _httpClient.GetAsync($"api/Vozila/{id}");

            if (odgovor.StatusCode ==
                System.Net.HttpStatusCode.NotFound)
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

            VoziloViewModel? vozilo =
                await odgovor.Content
                    .ReadFromJsonAsync<VoziloViewModel>();

            if (vozilo == null)
            {
                return NotFound();
            }

            return View(vozilo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ObrisiPotvrda(int id)
        {
            HttpResponseMessage odgovor =
                await _httpClient.DeleteAsync(
                    $"api/Vozila/{id}"
                );

            string odgovorTekst =
                await odgovor.Content.ReadAsStringAsync();

            if (odgovor.IsSuccessStatusCode)
            {
                TempData["Uspeh"] =
                    "Vozilo je uspešno obrisano.";

                return RedirectToAction(nameof(Index));
            }

            TempData["Greska"] =
                $"API greška: {(int)odgovor.StatusCode} " +
                $"{odgovor.StatusCode} - {odgovorTekst}";

            return RedirectToAction(nameof(Obrisi), new { id });
        }   

    }
}