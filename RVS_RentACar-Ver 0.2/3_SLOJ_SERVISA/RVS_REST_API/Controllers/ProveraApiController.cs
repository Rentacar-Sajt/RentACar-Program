
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;
using BibliotekaKlasa.KlasePodataka.Modeli;
using BibliotekaKlasa.KlasePodataka.Repozitorijumi;
using BibliotekaKlasa.TehnoloskeKlase;
using PoslovnaLogika.Klase;

namespace RVS_REST_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProveraApiController : ControllerBase
    {
        private readonly IWebHostEnvironment _okruzenje;
        private readonly IConfiguration _konfiguracija;

        public ProveraApiController(IWebHostEnvironment okruzenje, IConfiguration konfiguracija)
        {
            _okruzenje = okruzenje;
            _konfiguracija = konfiguracija;
        }

        [HttpGet("{korisnikId}")]
        public IActionResult Proveri(int korisnikId)
        {
            try
            {
                int brojOgranicenjaKursevaIzJSONa;
                int brojOgranicenjaKursevaIzBazePodataka;

                Ogranicenja ogranicenjaObjekat = new Ogranicenja();
                brojOgranicenjaKursevaIzJSONa = ogranicenjaObjekat.UzmiBrojKursevaPoKorisnikuIzJSON();

                string konekcioniString = _konfiguracija.GetConnectionString("KonekcioniString");
                KonekcijaKlasa konekcijaObjekat = new KonekcijaKlasa(konekcioniString);
                konekcijaObjekat.OtvoriKonekciju();
                KursRepo kursRepoObjekat = new KursRepo(konekcijaObjekat);
                brojOgranicenjaKursevaIzBazePodataka = kursRepoObjekat.UzmiBrojKursevaDodeljenihKorisnikuIzBazePodataka(korisnikId);
                konekcijaObjekat.ZatvoriKonekciju();

                bool mozeDaDodaNoviKurs = brojOgranicenjaKursevaIzBazePodataka < brojOgranicenjaKursevaIzJSONa;
                return Ok(new
                {
                    moze = mozeDaDodaNoviKurs
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { greska = ex.Message });
            }
            
        }
    }
}
