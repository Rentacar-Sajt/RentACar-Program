using BibliotekaKlasa.KlasePodataka.Modeli;
using BibliotekaKlasa.KlasePodataka.Repozitorijumi;
using Microsoft.AspNetCore.Mvc;

namespace REST_SERVIS_CRUD_Operacija.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Uloga klase: KlijentiController grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class KlijentiController : ControllerBase
    {
        private readonly KlijentRepo _klijentRepo;

        public KlijentiController(IConfiguration configuration)
        {
            string konekcioniString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Nije pronađen connection string DefaultConnection."
                );

            _klijentRepo = new KlijentRepo(konekcioniString);
        }

        // GET: api/Klijenti
        [HttpGet]
        // Dohvata sve zapise klijenta preko repozitorijuma i vraća ih klijentu kao HTTP 200 odgovor.
        public ActionResult<List<KlijentModel>> DajSve()
        {
            return Ok(_klijentRepo.DajSveKlijente());
        }

        // GET: api/Klijenti/5
        [HttpGet("{id}")]
        // Dohvata klijenta po prosleđenom ID-u; ako zapis ne postoji vraća HTTP 404 NotFound.
        public ActionResult<KlijentModel> DajPoId(int id)
        {
            KlijentModel? klijent = _klijentRepo.DajKlijentaPoId(id);

            if (klijent == null)
                return NotFound();

            return Ok(klijent);
        }

        // GET: api/Klijenti/pretraga/Petar
        [HttpGet("pretraga/{tekst}")]
        // Prima tekst za pretragu, prosleđuje ga repozitorijumu i vraća listu pronađenih zapisa klijenta.
        public ActionResult<List<KlijentModel>> Pretraga(string tekst)
        {
            return Ok(_klijentRepo.FiltrirajKlijente(tekst));
        }

        // POST: api/Klijenti
        [HttpPost]
        // Prima podatke novog klijenta iz tela HTTP zahteva, proverava ih i preko repozitorijuma upisuje novi zapis u bazu.
        public ActionResult Dodaj([FromBody] KlijentModel klijent)
        {
            int noviId = _klijentRepo.DodajKlijenta(klijent);

            if (noviId == 0)
                return BadRequest();

            klijent.Id = noviId;

            return CreatedAtAction(nameof(DajPoId),
                new { id = noviId },
                klijent);
        }

        // PUT: api/Klijenti/5
        [HttpPut("{id}")]
        // Prima ID i nove podatke klijenta, proverava da li zapis postoji i preko repozitorijuma ažurira podatke u bazi.
        public ActionResult Izmeni(int id, [FromBody] KlijentModel klijent)
        {
            if (id != klijent.Id)
                return BadRequest();

            bool uspeh = _klijentRepo.IzmeniKlijenta(klijent);

            if (!uspeh)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/Klijenti/5
        [HttpDelete("{id}")]
        // Proverava da li klijenta sa prosleđenim ID-em postoji, zatim ga preko repozitorijuma briše iz baze.
        public ActionResult Obrisi(int id)
        {
            bool uspeh = _klijentRepo.ObrisiKlijenta(id);

            if (!uspeh)
                return NotFound();

            return NoContent();
        }
    }
}
