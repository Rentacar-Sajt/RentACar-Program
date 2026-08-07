using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BibliotekaKlasa.Models;
using RVS_Aplikacija.ViewModels;
using RVS_MVC.Filteri;
using System.Net.Http.Json;

namespace RVS_Aplikacija.Controllers
{
    [AdminAutorizacijaFilter]
    public class PocetnaController : Controller
    {
        private readonly ILogger<PocetnaController> _logger;
        private readonly HttpClient _httpClient;

        public PocetnaController(
            ILogger<PocetnaController> logger,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;

            _httpClient =
                httpClientFactory.CreateClient("RestCrudServis");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            DashboardViewModel model =
                new DashboardViewModel();

            try
            {
                List<VoziloViewModel> vozila =
                    await _httpClient
                        .GetFromJsonAsync<List<VoziloViewModel>>(
                            "api/Vozila"
                        )
                    ?? new List<VoziloViewModel>();

                List<KlijentViewModel> klijenti =
                    await _httpClient
                        .GetFromJsonAsync<List<KlijentViewModel>>(
                            "api/Klijenti"
                        )
                    ?? new List<KlijentViewModel>();

                List<UgovorViewModel> ugovori =
                    await _httpClient
                        .GetFromJsonAsync<List<UgovorViewModel>>(
                            "api/Ugovori"
                        )
                    ?? new List<UgovorViewModel>();

                model.BrojVozila = vozila.Count;

                model.SlobodnaVozila =
                    vozila.Count(v =>
                        string.Equals(
                            v.StatusVozila,
                            "Slobodno",
                            StringComparison.OrdinalIgnoreCase
                        )
                    );

                model.ZauzetaVozila =
                    vozila.Count(v =>
                        string.Equals(
                            v.StatusVozila,
                            "Zauzeto",
                            StringComparison.OrdinalIgnoreCase
                        )
                    );

                model.BrojKlijenata = klijenti.Count;

                model.AktivniUgovori =
                    ugovori.Count(u =>
                        string.Equals(
                            u.StatusUgovora,
                            "Aktivan",
                            StringComparison.OrdinalIgnoreCase
                        )
                    );
            }
            catch (HttpRequestException ex)
            {
                ViewBag.Greska =
                    "Nije moguće učitati podatke sa REST CRUD servisa. " +
                    ex.Message;
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new GreskaViewModel
                {
                    RequestId =
                        Activity.Current?.Id ??
                        HttpContext.TraceIdentifier
                }
            );
        }
    }
}