using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using RVS_Aplikacija.ViewModels;

namespace RVS_Aplikacija.Controllers
{
    public class VozilaController : Controller
    {
        private readonly HttpClient _httpClient;

        public VozilaController(IHttpClientFactory httpClientFactory)
        {
            _httpClient =
                httpClientFactory.CreateClient("RestCrudServis");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<VoziloViewModel>? vozila =
                await _httpClient.GetFromJsonAsync<List<VoziloViewModel>>(
                    "api/Vozila"
                );

            return View(vozila ?? new List<VoziloViewModel>());
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