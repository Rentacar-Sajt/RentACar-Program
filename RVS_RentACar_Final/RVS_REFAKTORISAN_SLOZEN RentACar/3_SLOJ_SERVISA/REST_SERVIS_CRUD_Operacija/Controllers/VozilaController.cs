using BibliotekaKlasa.KlasePodataka.Modeli;
using BibliotekaKlasa.KlasePodataka.Repozitorijumi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace REST_SERVIS_CRUD_Operacija.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Uloga klase: VozilaController grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class VozilaController : ControllerBase
    {
        private readonly VoziloSPRepo _repo;

        public VozilaController(IConfiguration konfiguracija)
        {
            string stringKonekcije =
                konfiguracija.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Nije pronađen connection string DefaultConnection."
                );

            Console.WriteLine("KONEKCIJA API-ja:");
            Console.WriteLine(stringKonekcije);

            _repo = new VoziloSPRepo(stringKonekcije);
        }

        [HttpGet]
        // Dohvata sva vozila iz baze i vraća ih kao listu.
        public ActionResult<List<VoziloModel>> DajSvaVozila()
        {
            try
            {
                return Ok(_repo.DajSvaVozila());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    poruka = "Greška prilikom učitavanja vozila.",
                    greska = ex.Message
                });
            }
        }

        [HttpGet("{id:int}")]
        // Dohvata vozilo po prosleđenom ID-u; ako vozilo sa tim ID-em ne postoji vraća poruku da vozilo nije pronađeno.
        public ActionResult<VoziloModel> DajVoziloPoId(int id)
        {
            try
            {
                VoziloModel? vozilo = _repo.DajVoziloPoId(id);

                if (vozilo == null)
                {
                    return NotFound(new
                    {
                        poruka = $"Vozilo sa ID vrednošću {id} nije pronađeno."
                    });
                }

                return Ok(vozilo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    poruka = "Greška prilikom učitavanja vozila.",
                    greska = ex.Message
                });
            }
        }

        [HttpGet("registracija/{registracija}")]
        // Pretražuje vozila prema registraciji i vraća listu vozila koja odgovaraju unetom tekstu.
        public ActionResult<List<VoziloModel>> DajVozilaPoRegistraciji(
    string registracija)
        {
            if (string.IsNullOrWhiteSpace(registracija))
            {
                return BadRequest(new
                {
                    poruka = "Registracija je obavezna."
                });
            }

            try
            {
                List<VoziloModel> vozila =
                    _repo.DajVozilaPoRegistraciji(registracija.Trim());

                if (vozila.Count == 0)
                {
                    return NotFound(new
                    {
                        poruka =
                            $"Nije pronađeno vozilo sa registracijom koja sadrži '{registracija}'."
                    });
                }

                return Ok(vozila);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    poruka = "Greška prilikom pretrage vozila.",
                    greska = ex.Message
                });
            }
        }

        [HttpPost]
        // Dodaje novo vozilo u bazu na osnovu prosleđenih podataka i vraća rezultat operacije.
        public ActionResult DodajVozilo(
            [FromBody] VoziloModel vozilo)
        {
            if (vozilo == null)
            {
                return BadRequest(new
                {
                    poruka = "Podaci o vozilu nisu poslati."
                });
            }

            try
            {
                int noviId = _repo.DodajVozilo(vozilo);

                return CreatedAtAction(
                    nameof(DajVoziloPoId),
                    new { id = noviId },
                    new
                    {
                        id = noviId,
                        poruka = "Vozilo je uspešno dodato."
                    }
                );
            }
            catch (SqlException ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    poruka = "Greška prilikom dodavanja vozila.",
                    greska = ex.Message
                });
            }
        }

        [HttpPut("{id:int}")]
        // Menja podatke postojećeg vozila sa prosleđenim ID-em i vraća rezultat izmene.
        public ActionResult IzmeniVozilo(
            int id,
            [FromBody] VoziloModel vozilo)
        {
            if (vozilo == null)
            {
                return BadRequest(new
                {
                    poruka = "Podaci o vozilu nisu poslati."
                });
            }

            try
            {
                VoziloModel? postojeceVozilo =
                    _repo.DajVoziloPoId(id);

                if (postojeceVozilo == null)
                {
                    return NotFound(new
                    {
                        poruka = $"Vozilo sa ID vrednošću {id} nije pronađeno."
                    });
                }

                vozilo.Id = id;

                bool uspesno = _repo.IzmeniVozilo(vozilo);

                if (!uspesno)
                {
                    return BadRequest(new
                    {
                        poruka = "Vozilo nije izmenjeno."
                    });
                }

                return Ok(new
                {
                    poruka = "Vozilo je uspešno izmenjeno."
                });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    poruka = "Greška prilikom izmene vozila.",
                    greska = ex.Message
                });
            }
        }

        [HttpDelete("{id:int}")]
        // Briše vozilo sa prosleđenim ID-em iz baze i vraća rezultat operacije.
        public ActionResult ObrisiVozilo(int id)
        {
            try
            {
                VoziloModel? postojeceVozilo =
                    _repo.DajVoziloPoId(id);

                if (postojeceVozilo == null)
                {
                    return NotFound(new
                    {
                        poruka = $"Vozilo sa ID vrednošću {id} nije pronađeno."
                    });
                }

                bool uspesno = _repo.ObrisiVozilo(id);

                if (!uspesno)
                {
                    return BadRequest(new
                    {
                        poruka = "Vozilo nije obrisano."
                    });
                }

                return Ok(new
                {
                    poruka = "Vozilo je uspešno obrisano."
                });
            }
            catch (SqlException ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    poruka = "Greška prilikom brisanja vozila.",
                    greska = ex.Message
                });
            }
        }


    }
}
