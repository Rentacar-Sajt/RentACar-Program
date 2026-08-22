using BibliotekaKlasa.KlasePodataka.Modeli;
using Microsoft.AspNetCore.Mvc;
using RVS_REST_API.Modeli;

namespace RVS_REST_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // Uloga klase: AutentikacijaController grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class AutentikacijaController : ControllerBase
    {
        private readonly IConfiguration _konfiguracija;

        public AutentikacijaController(IConfiguration konfiguracija)
        {
            _konfiguracija = konfiguracija;
        }

        // POST: api/Autentikacija/Login
        [HttpPost("Login")]
        // Obrađuje prijavu korisnika: proverava unete podatke, poziva autentikaciju i vraća rezultat prijave ili odgovarajuću grešku.
        public ActionResult<LoginOdgovor> Login(
            [FromBody] LoginZahtev zahtev)
        {
            if (zahtev == null)
            {
                return BadRequest(new
                {
                    poruka = "Podaci za prijavu nisu poslati."
                });
            }

            if (string.IsNullOrWhiteSpace(zahtev.Email) ||
                string.IsNullOrWhiteSpace(zahtev.Lozinka))
            {
                return BadRequest(new
                {
                    poruka = "Email i lozinka su obavezni."
                });
            }

            string konekcioniString =
    _konfiguracija.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Konekcioni string nije pronađen."
    );

            global::PoslovnaLogika.Klase.AutentikacijaLogika logika =
                new global::PoslovnaLogika.Klase.AutentikacijaLogika(
                    konekcioniString
                );

            KorisnikModel? korisnik =
                logika.PrijaviKorisnika(
                    zahtev.Email,
                    zahtev.Lozinka
                );

            if (korisnik == null)
            {
                return Unauthorized(new
                {
                    poruka = "Email ili lozinka nisu ispravni."
                });
            }

            if (!string.Equals(
                    korisnik.Uloga,
                    "Administrator",
                    StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new
                    {
                        poruka =
                            "Korisnik nema administratorska prava."
                    }
                );
            }

            LoginOdgovor odgovor = new LoginOdgovor
            {
                Id = korisnik.Id,
                Ime = korisnik.Ime ?? string.Empty,
                Prezime = korisnik.Prezime ?? string.Empty,
                Email = korisnik.Email ?? string.Empty,
                Uloga = korisnik.Uloga ?? string.Empty
            };

            return Ok(odgovor);
        }
    }
}
