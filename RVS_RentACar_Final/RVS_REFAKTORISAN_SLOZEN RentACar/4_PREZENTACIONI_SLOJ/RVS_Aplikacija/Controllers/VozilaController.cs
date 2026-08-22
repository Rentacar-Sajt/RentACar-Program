using Microsoft.AspNetCore.Mvc;
using RVS_Aplikacija.ViewModels;
using RVS_MVC.Filteri;
using System.Net.Http.Json;

namespace RVS_Aplikacija.Controllers
{
    [AdminAutorizacijaFilter]
    // Uloga klase: VozilaController grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class VozilaController : Controller
    {
        private static readonly string[] DozvoljeneEkstenzije = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaksimalnaVelicinaSlike = 5 * 1024 * 1024;

        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VozilaController(IHttpClientFactory httpClientFactory, IWebHostEnvironment webHostEnvironment)
        {
            _httpClient = httpClientFactory.CreateClient("RestCrudServis");
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        // Učitava podatke potrebne za listu vozila i prosleđuje ih Index prikazu.
        public async Task<IActionResult> Index(string? status)
        {
            try
            {
                List<VoziloViewModel> vozila =
                    await _httpClient.GetFromJsonAsync<List<VoziloViewModel>>("api/Vozila")
                    ?? new List<VoziloViewModel>();

                if (!string.IsNullOrWhiteSpace(status))
                {
                    vozila = vozila.Where(v => string.Equals(v.StatusVozila, status, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                ViewBag.IzabraniStatus = status;
                return View(vozila);
            }
            catch (HttpRequestException ex)
            {
                ViewBag.Greska = "Nije moguće povezati se sa REST CRUD servisom. " + ex.Message;
                return View(new List<VoziloViewModel>());
            }
        }

        [HttpGet]
        // Otvara formu za dodavanje vozila i priprema početni model/podatke potrebne za prikaz forme.
        public IActionResult Dodaj() => View(new VoziloViewModel { StatusVozila = "Slobodno" });

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Prima podatke iz forme za dodavanje vozila, proverava validaciju i šalje zahtev servisu da sačuva novi zapis.
        public async Task<IActionResult> Dodaj(VoziloViewModel vozilo)
        {
            ProveriSliku(vozilo.SlikaFajl);
            if (!ModelState.IsValid) return View(vozilo);

            string? novaSlika = null;
            try
            {
                novaSlika = await SacuvajSliku(vozilo.SlikaFajl);
                vozilo.SlikaPutanja = novaSlika;

                HttpResponseMessage odgovor = await _httpClient.PostAsJsonAsync("api/Vozila", vozilo);
                string tekst = await odgovor.Content.ReadAsStringAsync();

                if (odgovor.IsSuccessStatusCode)
                {
                    TempData["Uspeh"] = "Vozilo je uspešno dodato.";
                    return RedirectToAction(nameof(Index));
                }

                ObrisiSliku(novaSlika);
                ModelState.AddModelError(string.Empty, $"API greška: {(int)odgovor.StatusCode} {odgovor.StatusCode} - {tekst}");
            }
            catch (Exception ex)
            {
                ObrisiSliku(novaSlika);
                ModelState.AddModelError(string.Empty, "Greška prilikom čuvanja vozila: " + ex.Message);
            }

            return View(vozilo);
        }

        [HttpGet]
        // Učitava podatke izabranog vozila po ID-u i prosleđuje ih stranici Detalji; ako zapis ne postoji vraća NotFound.
        public async Task<IActionResult> Detalji(int id)
        {
            VoziloViewModel? vozilo = await UcitajVozilo(id);
            return vozilo == null ? NotFound() : View(vozilo);
        }

        [HttpGet]
        // Učitava postojeće podatke vozila po ID-u i otvara formu za njihovu izmenu.
        public async Task<IActionResult> Izmeni(int id)
        {
            VoziloViewModel? vozilo = await UcitajVozilo(id);
            return vozilo == null ? NotFound() : View(vozilo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Prima izmenjene podatke vozila, proverava validaciju i šalje zahtev servisu da ažurira zapis.
        public async Task<IActionResult> Izmeni(int id, VoziloViewModel vozilo)
        {
            if (id != vozilo.Id) return BadRequest();

            ProveriSliku(vozilo.SlikaFajl);
            if (!ModelState.IsValid) return View(vozilo);

            string? staraSlika = vozilo.SlikaPutanja;
            string? novaSlika = null;

            try
            {
                if (vozilo.SlikaFajl != null && vozilo.SlikaFajl.Length > 0)
                {
                    novaSlika = await SacuvajSliku(vozilo.SlikaFajl);
                    vozilo.SlikaPutanja = novaSlika;
                }

                HttpResponseMessage odgovor = await _httpClient.PutAsJsonAsync($"api/Vozila/{id}", vozilo);
                string tekst = await odgovor.Content.ReadAsStringAsync();

                if (odgovor.IsSuccessStatusCode)
                {
                    if (novaSlika != null) ObrisiSliku(staraSlika);
                    TempData["Uspeh"] = "Vozilo je uspešno izmenjeno.";
                    return RedirectToAction(nameof(Index));
                }

                ObrisiSliku(novaSlika);
                vozilo.SlikaPutanja = staraSlika;
                ModelState.AddModelError(string.Empty, $"API greška: {(int)odgovor.StatusCode} {odgovor.StatusCode} - {tekst}");
            }
            catch (Exception ex)
            {
                ObrisiSliku(novaSlika);
                vozilo.SlikaPutanja = staraSlika;
                ModelState.AddModelError(string.Empty, "Greška prilikom izmene vozila: " + ex.Message);
            }

            return View(vozilo);
        }

        [HttpGet]
        // Učitava podatke vozila koji treba obrisati i prikazuje stranicu za potvrdu brisanja.
        public async Task<IActionResult> Obrisi(int id)
        {
            VoziloViewModel? vozilo = await UcitajVozilo(id);
            return vozilo == null ? NotFound() : View(vozilo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Nakon potvrde korisnika šalje zahtev servisu da obriše izabranog vozila, pa vraća korisnika na listu.
        public async Task<IActionResult> ObrisiPotvrda(int id)
        {
            VoziloViewModel? vozilo = await UcitajVozilo(id);
            HttpResponseMessage odgovor = await _httpClient.DeleteAsync($"api/Vozila/{id}");
            string tekst = await odgovor.Content.ReadAsStringAsync();

            if (odgovor.IsSuccessStatusCode)
            {
                ObrisiSliku(vozilo?.SlikaPutanja);
                TempData["Uspeh"] = "Vozilo je uspešno obrisano.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Greska"] = $"API greška: {(int)odgovor.StatusCode} {odgovor.StatusCode} - {tekst}";
            return RedirectToAction(nameof(Obrisi), new { id });
        }

        // Preuzima jedno vozilo preko REST servisa na osnovu njegovog ID-a i vraća model za prikaz.
        private async Task<VoziloViewModel?> UcitajVozilo(int id)
        {
            HttpResponseMessage odgovor = await _httpClient.GetAsync($"api/Vozila/{id}");
            if (!odgovor.IsSuccessStatusCode) return null;
            return await odgovor.Content.ReadFromJsonAsync<VoziloViewModel>();
        }

        // Proverava da li poslata slika zadovoljava dozvoljeni format i veličinu pre čuvanja.
        private void ProveriSliku(IFormFile? slika)
        {
            if (slika == null || slika.Length == 0) return;

            string ekstenzija = Path.GetExtension(slika.FileName).ToLowerInvariant();
            if (!DozvoljeneEkstenzije.Contains(ekstenzija))
                ModelState.AddModelError("SlikaFajl", "Dozvoljeni formati su JPG, JPEG, PNG i WEBP.");

            if (slika.Length > MaksimalnaVelicinaSlike)
                ModelState.AddModelError("SlikaFajl", "Slika ne može biti veća od 5 MB.");

            if (string.IsNullOrWhiteSpace(slika.ContentType) || !slika.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                ModelState.AddModelError("SlikaFajl", "Izabrani fajl nije slika.");
        }

        // Čuva poslatu sliku vozila u wwwroot direktorijumu i vraća putanju koja se zapisuje uz vozilo.
        private async Task<string?> SacuvajSliku(IFormFile? slika)
        {
            if (slika == null || slika.Length == 0) return null;

            string folder = Path.Combine(_webHostEnvironment.WebRootPath, "Slike", "Vozila");
            Directory.CreateDirectory(folder);

            string ekstenzija = Path.GetExtension(slika.FileName).ToLowerInvariant();
            string naziv = $"{Guid.NewGuid():N}{ekstenzija}";
            string punaPutanja = Path.Combine(folder, naziv);

            await using FileStream tok = new FileStream(punaPutanja, FileMode.CreateNew);
            await slika.CopyToAsync(tok);
            return $"/Slike/Vozila/{naziv}";
        }

        // Briše postojeći fajl slike vozila sa diska ako putanja postoji.
        private void ObrisiSliku(string? putanja)
        {
            if (string.IsNullOrWhiteSpace(putanja)) return;

            string relativna = putanja.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            string puna = Path.Combine(_webHostEnvironment.WebRootPath, relativna);
            if (System.IO.File.Exists(puna)) System.IO.File.Delete(puna);
        }
    }
}
