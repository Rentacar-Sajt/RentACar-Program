using System.Diagnostics;
using System.Net.Http.Json;
using BibliotekaKlasa.Models;
using Microsoft.AspNetCore.Mvc;
using RVS_Aplikacija.ViewModels;
using RVS_MVC.Filteri;

namespace RVS_Aplikacija.Controllers
{
    [AdminAutorizacijaFilter]
    // Uloga klase: PocetnaController grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class PocetnaController : Controller
    {
        private readonly ILogger<PocetnaController> _logger;
        private readonly HttpClient _httpClient;

        public PocetnaController(
            ILogger<PocetnaController> logger,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("RestCrudServis");
        }

        [HttpGet]
        // Učitava podatke potrebne za listu pocetna i prosleđuje ih Index prikazu.
        public async Task<IActionResult> Index()
        {
            DashboardViewModel model = new DashboardViewModel
            {
                GenerisanoU = DateTime.Now
            };

            try
            {
                Task<List<VoziloViewModel>?> vozilaZadatak =
                    _httpClient.GetFromJsonAsync<List<VoziloViewModel>>("api/Vozila");

                Task<List<KlijentViewModel>?> klijentiZadatak =
                    _httpClient.GetFromJsonAsync<List<KlijentViewModel>>("api/Klijenti");

                Task<List<UgovorViewModel>?> ugovoriZadatak =
                    _httpClient.GetFromJsonAsync<List<UgovorViewModel>>("api/Ugovori");

                await Task.WhenAll(vozilaZadatak, klijentiZadatak, ugovoriZadatak);

                List<VoziloViewModel> vozila =
                    await vozilaZadatak ?? new List<VoziloViewModel>();

                List<KlijentViewModel> klijenti =
                    await klijentiZadatak ?? new List<KlijentViewModel>();

                List<UgovorViewModel> ugovori =
                    await ugovoriZadatak ?? new List<UgovorViewModel>();

                Dictionary<int, KlijentViewModel> klijentiPoId =
                    klijenti.ToDictionary(k => k.Id);

                Dictionary<int, VoziloViewModel> vozilaPoId =
                    vozila.ToDictionary(v => v.Id);

                foreach (UgovorViewModel ugovor in ugovori)
                {
                    if (klijentiPoId.TryGetValue(ugovor.KlijentId, out KlijentViewModel? klijent))
                    {
                        ugovor.KlijentObjekat = klijent;
                    }

                    foreach (StavkaUgovoraViewModel stavka in ugovor.StavkeUgovora ?? new List<StavkaUgovoraViewModel>())
                    {
                        if (vozilaPoId.TryGetValue(stavka.VoziloId, out VoziloViewModel? vozilo))
                        {
                            stavka.VoziloObjekat = vozilo;
                        }
                    }
                }

                DateTime sada = DateTime.Now;
                DateTime danas = sada.Date;
                DateTime sutra = danas.AddDays(1);
                DateTime pocetakMeseca = new DateTime(danas.Year, danas.Month, 1);
                DateTime pocetakSledecegMeseca = pocetakMeseca.AddMonths(1);

                model.BrojVozila = vozila.Count;
                model.SlobodnaVozila = BrojVozilaSaStatusom(vozila, "Slobodno");
                model.ZauzetaVozila = BrojVozilaSaStatusom(vozila, "Zauzeto");
                model.VozilaNaServisu = BrojVozilaSaStatusom(vozila, "Servis");
                model.BrojKlijenata = klijenti.Count;

                model.AktivniUgovori = ugovori.Count(u =>
                    StatusJe(u.StatusUgovora, "Aktivan"));

                model.PreuzimanjaDanas = ugovori.Count(u =>
                    u.DatumPreuzimanja >= danas &&
                    u.DatumPreuzimanja < sutra &&
                    !StatusJe(u.StatusUgovora, "Otkazan"));

                model.VracanjaDanas = ugovori.Count(u =>
                    u.DatumVracanja >= danas &&
                    u.DatumVracanja < sutra &&
                    !StatusJe(u.StatusUgovora, "Otkazan"));

                model.ZakasnelaVracanja = ugovori.Count(u =>
                    u.DatumVracanja < sada &&
                    StatusJe(u.StatusUgovora, "Aktivan"));

                model.PrihodOvogMeseca = ugovori
                    .Where(u =>
                        u.DatumIzdavanja >= pocetakMeseca &&
                        u.DatumIzdavanja < pocetakSledecegMeseca &&
                        !StatusJe(u.StatusUgovora, "Otkazan"))
                    .Sum(u => u.UkupnoZaPlacanje);

                model.DanasnjaPreuzimanja = ugovori
                    .Where(u =>
                        u.DatumPreuzimanja >= danas &&
                        u.DatumPreuzimanja < sutra &&
                        !StatusJe(u.StatusUgovora, "Otkazan"))
                    .OrderBy(u => u.DatumPreuzimanja)
                    .Select(u => NapraviDashboardStavku(u, sada))
                    .ToList();

                model.DanasnjaVracanja = ugovori
                    .Where(u =>
                        u.DatumVracanja >= danas &&
                        u.DatumVracanja < sutra &&
                        !StatusJe(u.StatusUgovora, "Otkazan"))
                    .OrderBy(u => u.DatumVracanja)
                    .Select(u => NapraviDashboardStavku(u, sada))
                    .ToList();

                model.PoslednjiUgovori = ugovori
                    .OrderByDescending(u => u.DatumIzdavanja)
                    .ThenByDescending(u => u.Id)
                    .Take(5)
                    .Select(u => NapraviDashboardStavku(u, sada))
                    .ToList();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Greška pri učitavanju podataka za kontrolnu tablu.");

                ViewBag.Greska =
                    "Nije moguće učitati podatke sa REST CRUD servisa. " +
                    ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Neočekivana greška na kontrolnoj tabli.");

                ViewBag.Greska =
                    "Došlo je do greške pri pripremi kontrolne table. " +
                    ex.Message;
            }

            return View(model);
        }

        // Broji koliko vozila iz prosleđene kolekcije ima zadati status.
        private static int BrojVozilaSaStatusom(
            IEnumerable<VoziloViewModel> vozila,
            string status)
        {
            return vozila.Count(v => StatusJe(v.StatusVozila, status));
        }

        // Poredi trenutni status sa traženim statusom bez obzira na velika i mala slova.
        private static bool StatusJe(string? trenutniStatus, string trazeniStatus)
        {
            return string.Equals(
                trenutniStatus?.Trim(),
                trazeniStatus,
                StringComparison.OrdinalIgnoreCase);
        }

        // Formira jednu stavku podataka koja se prikazuje na početnom dashboard-u.
        private static DashboardUgovorStavkaViewModel NapraviDashboardStavku(
            UgovorViewModel ugovor,
            DateTime sada)
        {
            string klijent = ugovor.KlijentObjekat == null
                ? $"Klijent ID: {ugovor.KlijentId}"
                : $"{ugovor.KlijentObjekat.Ime} {ugovor.KlijentObjekat.Prezime}";

            List<string> naziviVozila = (ugovor.StavkeUgovora ?? new List<StavkaUgovoraViewModel>())
                .Select(s => s.VoziloObjekat == null
                    ? $"Vozilo ID: {s.VoziloId}"
                    : $"{s.VoziloObjekat.Marka} {s.VoziloObjekat.Model} ({s.VoziloObjekat.Registracija})")
                .Distinct()
                .ToList();

            string vozilo = naziviVozila.Count == 0
                ? "Nije učitano"
                : string.Join(", ", naziviVozila);

            return new DashboardUgovorStavkaViewModel
            {
                Id = ugovor.Id,
                BrojUgovora = ugovor.BrojUgovora,
                Klijent = klijent,
                Vozilo = vozilo,
                DatumPreuzimanja = ugovor.DatumPreuzimanja,
                DatumVracanja = ugovor.DatumVracanja,
                StatusUgovora = ugovor.StatusUgovora,
                UkupnoZaPlacanje = ugovor.UkupnoZaPlacanje,
                Kasni = ugovor.DatumVracanja < sada && StatusJe(ugovor.StatusUgovora, "Aktivan")
            };
        }

        // Otvara stranicu sa informacijama o privatnosti.
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // Priprema model sa podacima o grešci i prikazuje stranicu za grešku.
        public IActionResult Error()
        {
            return View(new GreskaViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
