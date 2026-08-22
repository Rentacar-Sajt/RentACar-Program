using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RVS_Aplikacija.Servisi.Pdf;
using RVS_Aplikacija.ViewModels;
using RVS_MVC.Filteri;
using System.Net;
using System.Net.Http.Json;

namespace RVS_Aplikacija.Controllers
{
    [AdminAutorizacijaFilter]
    // Uloga klase: UgovoriController grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class UgovoriController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IPdfUgovorServis _pdfUgovorServis;

        public UgovoriController(
            IHttpClientFactory httpClientFactory,
            IPdfUgovorServis pdfUgovorServis)
        {
            _httpClient = httpClientFactory.CreateClient(
                "RestCrudServis"
            );

            _pdfUgovorServis = pdfUgovorServis;
        }

        // GET: Ugovori/PreuzmiPdf/5
        [HttpGet]
        // Preuzima podatke konkretnog ugovora preko REST servisa, generiše PDF i vraća fajl korisniku.
        public async Task<IActionResult> PreuzmiPdf(int id)
        {
            HttpResponseMessage odgovor =
                await _httpClient.GetAsync(
                    $"api/Ugovori/{id}"
                );

            if (odgovor.StatusCode == HttpStatusCode.NotFound)
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
                ugovor.KlijentObjekat =
                    await _httpClient
                        .GetFromJsonAsync<KlijentViewModel>(
                            $"api/Klijenti/{ugovor.KlijentId}"
                        );
            }
            catch
            {
                ugovor.KlijentObjekat = null;
            }

            try
            {
                ugovor.KorisnikObjekat =
                    await _httpClient
                        .GetFromJsonAsync<KorisnikViewModel>(
                            $"api/Korisnici/{ugovor.KorisnikId}"
                        );
            }
            catch
            {
                ugovor.KorisnikObjekat = null;
            }

            byte[] pdf =
                _pdfUgovorServis.GenerisiPdf(ugovor);

            string bezbedanBrojUgovora =
                string.IsNullOrWhiteSpace(ugovor.BrojUgovora)
                    ? ugovor.Id.ToString()
                    : ugovor.BrojUgovora
                        .Replace("/", "-")
                        .Replace("\\", "-")
                        .Replace(" ", "_");

            string nazivFajla =
                $"Ugovor_{bezbedanBrojUgovora}.pdf";

            return File(
                pdf,
                "application/pdf",
                nazivFajla
            );
        }

        // GET: Ugovori
        [HttpGet]
        // Učitava podatke potrebne za listu ugovora i prosleđuje ih Index prikazu.
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
        // Učitava podatke izabranog ugovora po ID-u i prosleđuje ih stranici Detalji; ako zapis ne postoji vraća NotFound.
        public async Task<IActionResult> Detalji(int id)
        {
            HttpResponseMessage odgovor =
                await _httpClient.GetAsync(
                    $"api/Ugovori/{id}"
                );

            if (odgovor.StatusCode == HttpStatusCode.NotFound)
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
                ugovor.KlijentObjekat =
                    await _httpClient.GetFromJsonAsync<
                        KlijentViewModel>(
                            $"api/Klijenti/{ugovor.KlijentId}"
                        );
            }
            catch
            {
                ugovor.KlijentObjekat = null;
            }

            try
            {
                ugovor.KorisnikObjekat =
                    await _httpClient.GetFromJsonAsync<
                        KorisnikViewModel>(
                            $"api/Korisnici/{ugovor.KorisnikId}"
                        );
            }
            catch
            {
                ugovor.KorisnikObjekat = null;
            }

            return View(ugovor);
        }

        // GET: Ugovori/Dodaj
        [HttpGet]
        // Otvara formu za dodavanje ugovora i priprema početni model/podatke potrebne za prikaz forme.
        public async Task<IActionResult> Dodaj()
        {
            const int pocetniBrojDana = 3;

            string noviBrojUgovora =
                await GenerisiBrojUgovora();

            UgovorViewModel ugovor =
                new UgovorViewModel
                {
                    BrojUgovora = noviBrojUgovora,

                    DatumIzdavanja = DateTime.Today,

                    DatumPreuzimanja =
                        DateTime.Today
                            .AddDays(1)
                            .AddHours(10),

                    DatumVracanja =
                        DateTime.Today
                            .AddDays(4)
                            .AddHours(10),

                    StatusUgovora = "Aktivan",

                    StavkeUgovora =
                        new List<StavkaUgovoraViewModel>
                        {
                    new StavkaUgovoraViewModel
                    {
                        BrojDana = pocetniBrojDana,
                        PopustProcenat = 0
                    }
                        },

                    DodatneUsluge =
                        NapraviPocetneDodatneUsluge(
                            pocetniBrojDana
                        )
                };

            await PopuniPadajuceListe(ugovor);

            return View(ugovor);
        }

        // POST: Ugovori/Dodaj
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Prima podatke iz forme za dodavanje ugovora, proverava validaciju i šalje zahtev servisu da sačuva novi zapis.
        public async Task<IActionResult> Dodaj(
            UgovorViewModel ugovor)
        {
            // Broj ugovora se obavezno generiše i na serveru.
            // Tako korisnik ne može promeniti readonly vrednost
            // preko razvojnih alata u pregledaču.
            ugovor.BrojUgovora =
                await GenerisiBrojUgovora();

            ModelState.Remove(
                nameof(ugovor.BrojUgovora)
            );

            ugovor.StavkeUgovora ??=
                new List<StavkaUgovoraViewModel>();

            ugovor.DodatneUsluge ??=
                new List<DodatnaUslugaUgovoraViewModel>();

            if (ugovor.Depozit < 0)
            {
                ModelState.AddModelError(
                    nameof(ugovor.Depozit),
                    "Depozit ne može biti negativan."
                );
            }

            if (ugovor.DatumVracanja <=
                ugovor.DatumPreuzimanja)
            {
                ModelState.AddModelError(
                    nameof(ugovor.DatumVracanja),
                    "Datum vraćanja mora biti posle datuma preuzimanja."
                );
            }

            if (ugovor.StavkeUgovora.Count == 0)
            {
                ModelState.AddModelError(
                    nameof(ugovor.StavkeUgovora),
                    "Ugovor mora imati najmanje jednu stavku."
                );
            }

            for (int i = 0; i < ugovor.StavkeUgovora.Count; i++)
            {
                if (ugovor.StavkeUgovora[i].VoziloId <= 0)
                {
                    ModelState.AddModelError(
                        $"StavkeUgovora[{i}].VoziloId",
                        "Izaberite vozilo."
                    );
                }
            }

            List<int> dupliranaVozila =
                ugovor.StavkeUgovora
                    .Where(stavka => stavka.VoziloId > 0)
                    .GroupBy(stavka => stavka.VoziloId)
                    .Where(grupa => grupa.Count() > 1)
                    .Select(grupa => grupa.Key)
                    .ToList();

            if (dupliranaVozila.Count > 0)
            {
                ModelState.AddModelError(
                    nameof(ugovor.StavkeUgovora),
                    "Isto vozilo ne može biti dodato više puta u jedan ugovor."
                );
            }

            if (ugovor.StavkeUgovora.Count > 0 &&
     ugovor.DatumVracanja >
     ugovor.DatumPreuzimanja)
            {
                await PostaviCeneIzabranihVozila(ugovor);

                IzracunajStavkeIUgovor(ugovor);

                ModelState.Remove(
    nameof(ugovor.PopustProcenat)
);

                decimal ukupnoPreDepozita =
                    ugovor.UkupnoZaPlacanje +
                    Math.Max(0, ugovor.Depozit);

                if (ugovor.Depozit > ukupnoPreDepozita)
                {
                    ModelState.AddModelError(
                        nameof(ugovor.Depozit),
                        "Depozit ne može biti veći od ukupne vrednosti ugovora."
                    );
                }

                ugovor.IzabraneDodatneUsluge =
    string.Join(
        ",",
        ugovor.DodatneUsluge
            .Where(usluga => usluga.Izabrana)
            .Select(usluga => usluga.DodatnaUslugaId)
    );

                for (int i = 0;
                     i < ugovor.StavkeUgovora.Count;
                     i++)
                {
                    ModelState.Remove(
                        $"StavkeUgovora[{i}].BrojDana"
                    );

                    ModelState.Remove(
                        $"StavkeUgovora[{i}].Ukupno"
                    );
                }

                for (int i = 0;
                     i < ugovor.DodatneUsluge.Count;
                     i++)
                {
                    ModelState.Remove(
                        $"DodatneUsluge[{i}].BrojDana"
                    );

                    ModelState.Remove(
                        $"DodatneUsluge[{i}].Ukupno"
                    );
                }

                ModelState.Remove(
                    nameof(ugovor.UkupnoDodatneUsluge)
                );

                ModelState.Remove(
                    nameof(ugovor.UkupnoZaPlacanje)
                );
            }

            if (!ModelState.IsValid)
            {
                if (ugovor.DodatneUsluge.Count == 0)
                {
                    int brojDana =
                        IzracunajBrojDana(
                            ugovor.DatumPreuzimanja,
                            ugovor.DatumVracanja
                        );

                    ugovor.DodatneUsluge =
                        NapraviPocetneDodatneUsluge(
                            brojDana
                        );
                }

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
        // Učitava izabrani ugovor i priprema formu sa dozvoljenim statusima za promenu statusa.
        public async Task<IActionResult> PromeniStatus(int id)
        {
            HttpResponseMessage odgovor =
                await _httpClient.GetAsync(
                    $"api/Ugovori/{id}"
                );

            if (odgovor.StatusCode == HttpStatusCode.NotFound)
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
                    NoviStatus = ugovor.StatusUgovora,
                    DatumVracanja = ugovor.DatumVracanja,
                    StvarniDatumVracanja =
                        ugovor.StvarniDatumVracanja ?? DateTime.Now,
                    BrojDanaKasnjenja = ugovor.BrojDanaKasnjenja,
                    KaznaZaKasnjenje = ugovor.KaznaZaKasnjenje
                };

            PopuniStatuse(model);

            return View(model);
        }

        // POST: Ugovori/PromeniStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Prima novi status ugovora, proverava podatke i šalje REST servisu zahtev za promenu statusa.
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

            HttpResponseMessage ugovorOdgovor =
                await _httpClient.GetAsync(
                    $"api/Ugovori/{id}"
                );

            if (!ugovorOdgovor.IsSuccessStatusCode)
            {
                return StatusCode(
                    (int)ugovorOdgovor.StatusCode,
                    await ugovorOdgovor.Content.ReadAsStringAsync()
                );
            }

            UgovorViewModel? postojeciUgovor =
                await ugovorOdgovor.Content
                    .ReadFromJsonAsync<UgovorViewModel>();

            if (postojeciUgovor == null)
            {
                return NotFound();
            }

            // Datum iz baze je autoritativan. Ne verujemo hidden polju iz forme.
            model.DatumVracanja = postojeciUgovor.DatumVracanja;
            model.BrojUgovora = postojeciUgovor.BrojUgovora;
            model.TrenutniStatus = postojeciUgovor.StatusUgovora;

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
                    "Dozvoljeni statusi su: Aktivan, Završen i Otkazan."
                );
            }

            const double tolerancijaUSatima = 6;
            const decimal kaznaPoDanu = 2000m;

            model.BrojDanaKasnjenja = 0;
            model.KaznaZaKasnjenje = 0m;

            bool zavrsavaUgovor =
                model.NoviStatus.Equals(
                    "Završen",
                    StringComparison.OrdinalIgnoreCase
                );

            if (zavrsavaUgovor)
            {
                if (!model.StvarniDatumVracanja.HasValue)
                {
                    ModelState.AddModelError(
                        nameof(model.StvarniDatumVracanja),
                        "Stvarni datum vraćanja je obavezan."
                    );
                }
                else
                {
                    DateTime stvarniDatum =
                        model.StvarniDatumVracanja.Value;

                    if (stvarniDatum < postojeciUgovor.DatumPreuzimanja)
                    {
                        ModelState.AddModelError(
                            nameof(model.StvarniDatumVracanja),
                            "Stvarni datum vraćanja ne može biti pre datuma preuzimanja."
                        );
                    }
                    else
                    {
                        TimeSpan kasnjenje =
                            stvarniDatum - postojeciUgovor.DatumVracanja;

                        if (kasnjenje.TotalHours > tolerancijaUSatima)
                        {
                            model.BrojDanaKasnjenja =
                                Math.Max(
                                    1,
                                    (int)Math.Ceiling(
                                        kasnjenje.TotalDays
                                    )
                                );

                            model.KaznaZaKasnjenje =
                                model.BrojDanaKasnjenja *
                                kaznaPoDanu;
                        }
                    }
                }
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
                        NoviStatus = model.NoviStatus,
                        StvarniDatumVracanja =
                            zavrsavaUgovor
                                ? model.StvarniDatumVracanja
                                : null,
                        BrojDanaKasnjenja =
                            zavrsavaUgovor
                                ? model.BrojDanaKasnjenja
                                : 0,
                        KaznaZaKasnjenje =
                            zavrsavaUgovor
                                ? model.KaznaZaKasnjenje
                                : 0m
                    }
                );

            string odgovorTekst =
                await odgovor.Content.ReadAsStringAsync();

            if (odgovor.IsSuccessStatusCode)
            {
                TempData["Uspeh"] =
                    zavrsavaUgovor && model.KaznaZaKasnjenje > 0
                        ? $"Ugovor je završen. Kazna za kašnjenje iznosi {model.KaznaZaKasnjenje:N2} RSD."
                        : "Status ugovora je uspešno promenjen.";

                return RedirectToAction(nameof(Detalji), new { id });
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
        // Učitava podatke ugovora koji treba obrisati i prikazuje stranicu za potvrdu brisanja.
        public async Task<IActionResult> Obrisi(int id)
        {
            HttpResponseMessage odgovor =
                await _httpClient.GetAsync(
                    $"api/Ugovori/{id}"
                );

            if (odgovor.StatusCode == HttpStatusCode.NotFound)
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
                ugovor.KlijentObjekat =
                    await _httpClient.GetFromJsonAsync<
                        KlijentViewModel>(
                            $"api/Klijenti/{ugovor.KlijentId}"
                        );
            }
            catch
            {
                ugovor.KlijentObjekat = null;
            }

            return View(ugovor);
        }

        // POST: Ugovori/ObrisiPotvrda/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Nakon potvrde korisnika šalje zahtev servisu da obriše izabranog ugovora, pa vraća korisnika na listu.
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

        // GET: Ugovori/PreuzmiCenuVozila?id=5
        [HttpGet]
        // Preuzima dnevnu cenu jednog vozila preko REST servisa na osnovu njegovog ID-a.
        public async Task<IActionResult> PreuzmiCenuVozila(int id)
        {
            if (id <= 0)
            {
                return BadRequest(
                    new
                    {
                        poruka = "ID vozila nije ispravan."
                    }
                );
            }

            try
            {
                VoziloViewModel? vozilo =
                    await _httpClient.GetFromJsonAsync<
                        VoziloViewModel>(
                            $"api/Vozila/{id}"
                        );

                if (vozilo == null)
                {
                    return NotFound(
                        new
                        {
                            poruka = "Vozilo nije pronađeno."
                        }
                    );
                }

                return Json(
                    new
                    {
                        cenaPoDanu = vozilo.CenaPoDanu
                    }
                );
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    new
                    {
                        poruka =
                            "Nije moguće učitati cenu vozila.",
                        detalji = ex.Message
                    }
                );
            }
        }

        // Učitava podatke potrebne za padajuće liste u formi ugovora, kao što su klijenti, vozila i korisnici.
        private async Task PopuniPadajuceListe(
            UgovorViewModel ugovor)
        {
            ugovor.StavkeUgovora ??=
                new List<StavkaUgovoraViewModel>();

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

                ugovor.CeneVozilaPoDanu =
    vozila?
        .ToDictionary(
            vozilo => vozilo.Id,
            vozilo => vozilo.CenaPoDanu
        )
    ?? new Dictionary<int, decimal>();

                ugovor.VozilaOpcije =
    vozila?
        .Where(v =>
            string.Equals(
                v.StatusVozila,
                "Slobodno",
                StringComparison.OrdinalIgnoreCase
            )
            ||
            ugovor.StavkeUgovora.Any(
                s => s.VoziloId == v.Id
            )
        )
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

        // Za izabrana vozila učitava njihove dnevne cene i upisuje ih u stavke ugovora pre obračuna.
        private async Task PostaviCeneIzabranihVozila(
            UgovorViewModel ugovor)
        {
            if (ugovor.StavkeUgovora == null)
            {
                return;
            }

            for (int i = 0;
                 i < ugovor.StavkeUgovora.Count;
                 i++)
            {
                StavkaUgovoraViewModel stavka =
                    ugovor.StavkeUgovora[i];

                if (stavka.VoziloId <= 0)
                {
                    continue;
                }

                try
                {
                    VoziloViewModel? vozilo =
                        await _httpClient.GetFromJsonAsync<
                            VoziloViewModel>(
                                $"api/Vozila/{stavka.VoziloId}"
                            );

                    if (vozilo == null)
                    {
                        if (!string.Equals(
        vozilo.StatusVozila,
        "Slobodno",
        StringComparison.OrdinalIgnoreCase))
                        {
                            ModelState.AddModelError(
                                $"StavkeUgovora[{i}].VoziloId",
                                $"Vozilo {NapraviNazivVozila(vozilo)} više nije slobodno."
                            );

                            continue;
                        }
                        ModelState.AddModelError(
                            $"StavkeUgovora[{i}].VoziloId",
                            "Izabrano vozilo nije pronađeno."
                        );

                        continue;
                    }

                    stavka.CenaPoDanu =
                        vozilo.CenaPoDanu;

                    stavka.VoziloObjekat =
                        vozilo;
                }
                catch
                {
                    ModelState.AddModelError(
                        $"StavkeUgovora[{i}].VoziloId",
                        "Nije moguće učitati cenu izabranog vozila."
                    );
                }
            }
        }

        // Ponovo obračunava broj dana, iznose stavki, popust, dodatne usluge i konačan iznos celog ugovora.
        private static void IzracunajStavkeIUgovor(
            UgovorViewModel ugovor)
        {
            ugovor.StavkeUgovora ??=
                new List<StavkaUgovoraViewModel>();

            ugovor.DodatneUsluge ??=
                new List<DodatnaUslugaUgovoraViewModel>();

            int brojDana =
                IzracunajBrojDana(
                    ugovor.DatumPreuzimanja,
                    ugovor.DatumVracanja
                );

            ugovor.PopustProcenat =
    brojDana >= 7
        ? 10m
        : 0m;

            foreach (StavkaUgovoraViewModel stavka
                     in ugovor.StavkeUgovora)
            {
                stavka.BrojDana = brojDana;

                decimal osnovnaCena =
                    stavka.BrojDana *
                    stavka.CenaPoDanu;

                decimal popustStavke =
                    osnovnaCena *
                    stavka.PopustProcenat /
                    100m;

                stavka.Ukupno =
                    osnovnaCena -
                    popustStavke;
            }

            decimal ukupnoVozila =
                ugovor.StavkeUgovora.Sum(
                    stavka => stavka.Ukupno
                );

            decimal ukupnoDodatneUsluge = 0;

            foreach (DodatnaUslugaUgovoraViewModel usluga
                     in ugovor.DodatneUsluge)
            {
                usluga.BrojDana = brojDana;

                if (!usluga.Izabrana)
                {
                    usluga.Ukupno = 0;
                    continue;
                }

                if (usluga.CenaPoDanu < 0)
                {
                    usluga.CenaPoDanu = 0;
                }

                usluga.Ukupno =
                    usluga.CenaPoDanu *
                    usluga.BrojDana;

                ukupnoDodatneUsluge +=
                    usluga.Ukupno;
            }

            ugovor.UkupnoDodatneUsluge =
                ukupnoDodatneUsluge;

            decimal popustNaUgovor =
                ukupnoVozila *
                ugovor.PopustProcenat /
                100m;

            decimal ukupnoPreDepozita =
                ukupnoVozila +
                ukupnoDodatneUsluge -
                popustNaUgovor;

            decimal depozit =
                Math.Max(
                    0,
                    ugovor.Depozit
                );

            ugovor.UkupnoZaPlacanje =
                Math.Max(
                    0,
                    ukupnoPreDepozita -
                    depozit
                );
        }

        // Računa broj naplatnih dana između datuma preuzimanja i vraćanja, pri čemu započeti dan računa kao ceo dan.
        private static int IzracunajBrojDana(
            DateTime datumPreuzimanja,
            DateTime datumVracanja)
        {
            return Math.Max(
                1,
                (int)Math.Ceiling(
                    (datumVracanja -
                     datumPreuzimanja).TotalDays
                )
            );
        }

        private static List<DodatnaUslugaUgovoraViewModel>
            NapraviPocetneDodatneUsluge(
                int brojDana)
        {
            return new List<DodatnaUslugaUgovoraViewModel>
            {
                new DodatnaUslugaUgovoraViewModel
                {
                    DodatnaUslugaId = 1,
                    NazivUsluge = "GPS navigacija",
                    CenaPoDanu = 300,
                    BrojDana = brojDana
                },

                new DodatnaUslugaUgovoraViewModel
                {
                    DodatnaUslugaId = 2,
                    NazivUsluge = "Dečije sedište",
                    CenaPoDanu = 400,
                    BrojDana = brojDana
                },

                new DodatnaUslugaUgovoraViewModel
                {
                    DodatnaUslugaId = 3,
                    NazivUsluge = "Full osiguranje",
                    CenaPoDanu = 1500,
                    BrojDana = brojDana
                },

                new DodatnaUslugaUgovoraViewModel
                {
                    DodatnaUslugaId = 4,
                    NazivUsluge = "Dodatni vozač",
                    CenaPoDanu = 700,
                    BrojDana = brojDana
                },

                new DodatnaUslugaUgovoraViewModel
                {
                    DodatnaUslugaId = 5,
                    NazivUsluge = "Ostalo",
                    CenaPoDanu = 0,
                    BrojDana = brojDana
                }
            };
        }

        // Popunjava listu dozvoljenih statusa ugovora koja se prikazuje u formi za promenu statusa.
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

        // Formira čitljiv naziv vozila od dostupnih podataka, npr. marke, modela i registracije.
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

        // Generiše sledeći broj ugovora u formatu koji koristi aplikacija.
        private async Task<string> GenerisiBrojUgovora()
        {
            int godina = DateTime.Today.Year;
            string prefiks = $"UG-{godina}-";

            List<UgovorViewModel> ugovori;

            try
            {
                ugovori =
                    await _httpClient.GetFromJsonAsync<
                        List<UgovorViewModel>>(
                            "api/Ugovori"
                        )
                    ?? new List<UgovorViewModel>();
            }
            catch
            {
                ugovori = new List<UgovorViewModel>();
            }

            int najveciRedniBroj = ugovori
                .Where(ugovor =>
                    ugovor.DatumIzdavanja.Year == godina)
                .Select(ugovor =>
                {
                    string broj =
                        ugovor.BrojUgovora ?? string.Empty;

                    if (!broj.StartsWith(
                            prefiks,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        return 0;
                    }

                    string redniDeo =
                        broj.Substring(prefiks.Length);

                    return int.TryParse(
                        redniDeo,
                        out int redniBroj)
                            ? redniBroj
                            : 0;
                })
                .DefaultIfEmpty(0)
                .Max();

            // Ako stari ugovori nemaju novi format,
            // brojanje ugovora iz tekuće godine služi kao početna vrednost.
            int brojUgovoraUTekucojGodini =
                ugovori.Count(ugovor =>
                    ugovor.DatumIzdavanja.Year == godina);

            int sledeciRedniBroj =
                Math.Max(
                    najveciRedniBroj,
                    brojUgovoraUTekucojGodini
                ) + 1;

            return $"{prefiks}{sledeciRedniBroj:D4}";
        }
    }
}
