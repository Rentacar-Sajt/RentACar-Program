using BibliotekaKlasa.KlasePodataka.Modeli;
using BibliotekaKlasa.KlasePodataka.Repozitorijumi;
using Microsoft.AspNetCore.Mvc;

namespace REST_SERVIS_CRUD_Operacija.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KorisniciController : ControllerBase
    {
        private readonly KorisnikRepo _korisnikRepo;

        public KorisniciController(
            IConfiguration configuration)
        {
            string konekcioniString =
                configuration.GetConnectionString(
                    "DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Nije pronađen connection string " +
                    "DefaultConnection.");

            _korisnikRepo =
                new KorisnikRepo(konekcioniString);
        }

        // GET: api/Korisnici
        [HttpGet]
        public ActionResult<List<KorisnikModel>> DajSve()
        {
            List<KorisnikModel> korisnici =
                _korisnikRepo.DajSveKorisnike();

            // LozinkaHash i LozinkaSalt ne treba
            // da se šalju klijentskoj aplikaciji.
            foreach (KorisnikModel korisnik in korisnici)
            {
                korisnik.LozinkaHash = null;
                korisnik.LozinkaSalt = null;
            }

            return Ok(korisnici);
        }

        // GET: api/Korisnici/5
        [HttpGet("{id:int}")]
        public ActionResult<KorisnikModel> DajPoId(int id)
        {
            KorisnikModel? korisnik =
                _korisnikRepo.DajKorisnikaPoId(id);

            if (korisnik == null)
            {
                return NotFound(
                    new
                    {
                        poruka =
                            $"Korisnik sa ID vrednošću {id} " +
                            "nije pronađen."
                    }
                );
            }

            korisnik.LozinkaHash = null;
            korisnik.LozinkaSalt = null;

            return Ok(korisnik);
        }

        // GET: api/Korisnici/email/admin@rentacar.rs
        [HttpGet("email/{email}")]
        public ActionResult<KorisnikModel> DajPoEmailu(
            string email)
        {
            KorisnikModel? korisnik =
                _korisnikRepo.DajKorisnikaPoEmailu(email);

            if (korisnik == null)
            {
                return NotFound(
                    new
                    {
                        poruka =
                            $"Korisnik sa email adresom '{email}' nije pronađen."
                    });
            }

            // Namerno vraćamo LozinkaHash.
            // Ovu rutu koristi samo REST API za autentikaciju.
            return Ok(korisnik);
        }

        // GET: api/Korisnici/pretraga/Admin
        [HttpGet("pretraga/{tekst}")]
        public ActionResult<List<KorisnikModel>> Pretraga(
            string tekst)
        {
            List<KorisnikModel> korisnici =
                _korisnikRepo.FiltrirajKorisnike(tekst);

            foreach (KorisnikModel korisnik in korisnici)
            {
                korisnik.LozinkaHash = null;
                korisnik.LozinkaSalt = null;
            }

            return Ok(korisnici);
        }

        // POST: api/Korisnici
        [HttpPost]
        public ActionResult Dodaj(
            [FromBody] KorisnikModel korisnik)
        {
            if (korisnik == null)
            {
                return BadRequest(
                    new
                    {
                        poruka =
                            "Podaci o korisniku nisu poslati."
                    }
                );
            }

            if (string.IsNullOrWhiteSpace(korisnik.Ime) ||
                string.IsNullOrWhiteSpace(korisnik.Prezime))
            {
                return BadRequest(
                    new
                    {
                        poruka =
                            "Ime i prezime su obavezni."
                    }
                );
            }

            int noviId =
                _korisnikRepo.DodajKorisnika(korisnik);

            if (noviId == 0)
            {
                return BadRequest(
                    new
                    {
                        poruka =
                            "Korisnik nije dodat."
                    }
                );
            }

            korisnik.Id = noviId;
            korisnik.LozinkaHash = null;
            korisnik.LozinkaSalt = null;

            return CreatedAtAction(
                nameof(DajPoId),
                new { id = noviId },
                korisnik
            );
        }

        // PUT: api/Korisnici/5
        [HttpPut("{id:int}")]
        public ActionResult Izmeni(
            int id,
            [FromBody] KorisnikModel korisnik)
        {
            if (korisnik == null || id != korisnik.Id)
            {
                return BadRequest(
                    new
                    {
                        poruka =
                            "ID korisnika nije ispravan."
                    }
                );
            }

            bool uspeh =
                _korisnikRepo.IzmeniKorisnika(korisnik);

            if (!uspeh)
            {
                return NotFound(
                    new
                    {
                        poruka =
                            $"Korisnik sa ID vrednošću {id} " +
                            "nije pronađen."
                    }
                );
            }

            return NoContent();
        }

        // DELETE: api/Korisnici/5
        [HttpDelete("{id:int}")]
        public ActionResult Obrisi(int id)
        {
            bool uspeh =
                _korisnikRepo.ObrisiKorisnika(id);

            if (!uspeh)
            {
                return NotFound(
                    new
                    {
                        poruka =
                            $"Korisnik sa ID vrednošću {id} " +
                            "nije pronađen."
                    }
                );
            }

            return NoContent();
        }
    }
}