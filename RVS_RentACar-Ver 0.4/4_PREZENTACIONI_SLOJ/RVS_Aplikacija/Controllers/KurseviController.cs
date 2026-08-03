using BibliotekaKlasa.KlasePodataka.Modeli;
using BibliotekaKlasa.KlasePodataka.Repozitorijumi;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BibliotekaKlasa.Servisi;
using RVS_Aplikacija.ViewModels;
using BibliotekaKlasa.TehnoloskeKlase;

namespace RVS_Aplikacija.Controllers
{
    public class KurseviController : Controller
    {
        private readonly KursRepo _kursRepoObjekat;
        private readonly TehnologijaSPRepo _tehnologijaSPRepoObjekat;
        private CasRepo _casRepoObjekat;
        private KonekcijaKlasa _konekcijaObjekat;

        public KurseviController(IConfiguration konfiguracija, KursRepo kursRepo)
        {
            _kursRepoObjekat = kursRepo;

            string konekcioniString = konfiguracija.GetConnectionString("KonekcioniString");
            _konekcijaObjekat = new KonekcijaKlasa(konekcioniString);

            _casRepoObjekat = new CasRepo(_konekcijaObjekat);
            _tehnologijaSPRepoObjekat = new TehnologijaSPRepo(konekcioniString);
        }

        public IActionResult Index(string filter)
        {
            var listaKurseva = string.IsNullOrWhiteSpace(filter)
                ? _kursRepoObjekat.DajSve()
                : _kursRepoObjekat.DajSveSaFilterom(filter);

            if (!string.IsNullOrWhiteSpace(filter))
            {
                ViewData["Filter"] = filter;
            }

            return View(listaKurseva);
        }

        [HttpGet]
        public IActionResult Dodaj()
        {
            var kursViewModelObjekat = new KursViewModel();
            var listaTehnologijaObjekata = _tehnologijaSPRepoObjekat.DajSveTehnologije();

            ViewBag.listaTehnologijaObjekata = listaTehnologijaObjekata;

            return View(kursViewModelObjekat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dodaj(KursViewModel kursViewModelObjekat)
        {
            // Дебаг испис
            System.Diagnostics.Debug.WriteLine($"Примљено часова: {kursViewModelObjekat.Casovi?.Count ?? 0}");
            foreach (var key in Request.Form.Keys)
            {
                System.Diagnostics.Debug.WriteLine($"{key} = {Request.Form[key]}");
            }
            int autorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var servisProvereKursevaObjekat =
                new ServisProvereKurseva(new System.Net.Http.HttpClient());

            bool korisnikMozeDaUpiseKurs =
                await servisProvereKursevaObjekat.DaLiKorisnikMozeDaUpiseKurs(autorId);

            if (!korisnikMozeDaUpiseKurs)
            {
                TempData["Poruka"] = "Не можете додати више курсева!";
                TempData["PorukaTip"] = "greska";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                return View(kursViewModelObjekat);
            }

            var kursModelObjekat = new KursModel
            {
                NazivKursa = kursViewModelObjekat.NazivKursa,
                OpisKursa = kursViewModelObjekat.OpisKursa
            };

            _kursRepoObjekat.Dodaj(kursModelObjekat, autorId);

            int kursIdPoslednjegZapisa = _kursRepoObjekat.DajPoslednjiKursId();

            if (kursViewModelObjekat.Casovi != null && kursViewModelObjekat.Casovi.Any())
            {
                _konekcijaObjekat.OtvoriKonekciju();

                foreach (var casModelObjekat in kursViewModelObjekat.Casovi)
                {
                    var casZaUpisUBazuObjekat = new CasModel
                    {
                        RedniBrojCasa = casModelObjekat.RedniBrojCasa,
                        KursId = kursIdPoslednjegZapisa,
                        TehnologijaObjekat = casModelObjekat.TehnologijaObjekat
                    };

                    _casRepoObjekat.Dodaj(casZaUpisUBazuObjekat);
                }

                _konekcijaObjekat.ZatvoriKonekciju();
            }

            TempData["Poruka"] = "Курс је успешно додат!";
            TempData["PorukaTip"] = "uspeh";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Izmeni(int id)
        {
            var kursModelObjekat = _kursRepoObjekat.DajSve()
                .FirstOrDefault(kursModelObjekat => kursModelObjekat.Id == id);

            if (kursModelObjekat == null)
            {
                return NotFound();
            }

            _konekcijaObjekat.OtvoriKonekciju();

            kursModelObjekat.Casovi =
                _casRepoObjekat.DajSveCasovePoIdKursa(id);

            _konekcijaObjekat.ZatvoriKonekciju();

            var kursViewModelObjekat = new KursViewModel
            {
                Id = kursModelObjekat.Id,
                NazivKursa = kursModelObjekat.NazivKursa,
                OpisKursa = kursModelObjekat.OpisKursa,
                Casovi = kursModelObjekat.Casovi
            };

            ViewBag.listaTehnologijaObjekata =
                _tehnologijaSPRepoObjekat.DajSveTehnologije();

            return View(kursViewModelObjekat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Izmeni(KursViewModel kursViewModelObjekat)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.listaTehnologijaObjekata =
                    _tehnologijaSPRepoObjekat.DajSveTehnologije();

                return View(kursViewModelObjekat);
            }

            _konekcijaObjekat.OtvoriKonekciju();

            var kursModelObjekat = new KursModel
            {
                Id = kursViewModelObjekat.Id,
                NazivKursa = kursViewModelObjekat.NazivKursa,
                OpisKursa = kursViewModelObjekat.OpisKursa
            };

            _kursRepoObjekat.Izmeni(kursModelObjekat);

            var listaStarihCasova =
                _casRepoObjekat.DajSveCasovePoIdKursa(kursViewModelObjekat.Id);

            foreach (var casModelObjekat in listaStarihCasova)
            {
                _casRepoObjekat.Obrisi(casModelObjekat.Id);
            }

            if (kursViewModelObjekat.Casovi != null && kursViewModelObjekat.Casovi.Any())
            {
                foreach (var casModelObjekat in kursViewModelObjekat.Casovi)
                {
                    var casZaUpisUBazuObjekat = new CasModel
                    {
                        RedniBrojCasa = casModelObjekat.RedniBrojCasa,
                        KursId = kursViewModelObjekat.Id,
                        TehnologijaObjekat = casModelObjekat.TehnologijaObjekat
                    };

                    _casRepoObjekat.Dodaj(casZaUpisUBazuObjekat);
                }
            }

            _konekcijaObjekat.ZatvoriKonekciju();

            TempData["Poruka"] = "Курс је успешно измењен!";
            TempData["PorukaTip"] = "uspeh";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Obrisi(int id)
        {
            _kursRepoObjekat.Obrisi(id);

            TempData["Poruka"] = "Курс је успешно обрисан!";
            TempData["PorukaTip"] = "uspeh";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult MasterDetail(int id)
        {
            var kursModelObjekat = _kursRepoObjekat.DajSve()
                .FirstOrDefault(kursModelObjekat => kursModelObjekat.Id == id);

            if (kursModelObjekat == null)
            {
                return NotFound();
            }

            _konekcijaObjekat.OtvoriKonekciju();

            kursModelObjekat.Casovi =
                _casRepoObjekat.DajSveCasovePoIdKursa(id);

            _konekcijaObjekat.ZatvoriKonekciju();

            return View(kursModelObjekat);
        }
    }
}