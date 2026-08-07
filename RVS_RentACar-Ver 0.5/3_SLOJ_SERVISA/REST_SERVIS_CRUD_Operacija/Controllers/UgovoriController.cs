using BibliotekaKlasa.KlasePodataka.Modeli;
using BibliotekaKlasa.KlasePodataka.Repozitorijumi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using REST_SERVIS_CRUD_Operacija.Modeli;

namespace REST_SERVIS_CRUD_Operacija.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UgovoriController : ControllerBase
    {
        private readonly UgovorRepo _repo;

        public UgovoriController(IConfiguration konfiguracija)
        {
            string stringKonekcije =    
                konfiguracija.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Nije pronađen connection string DefaultConnection."
                );

            _repo = new UgovorRepo(stringKonekcije);
        }

        [HttpGet]
        public ActionResult<List<UgovorModel>> DajSveUgovore()
        {
            try
            {
                List<UgovorModel> ugovori =
                    _repo.DajSveUgovore();

                return Ok(ugovori);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        poruka = "Greška prilikom učitavanja ugovora.",
                        greska = ex.Message
                    }
                );
            }
        }

        [HttpGet("pretraga")]
        public ActionResult<List<UgovorModel>> FiltrirajUgovore(
    [FromQuery] string? brojUgovora,
    [FromQuery] string? klijent,
    [FromQuery] string? statusUgovora,
    [FromQuery] DateTime? datumOd,
    [FromQuery] DateTime? datumDo)
        {
            if (datumOd.HasValue &&
                datumDo.HasValue &&
                datumOd.Value.Date > datumDo.Value.Date)
            {
                return BadRequest(
                    new
                    {
                        poruka = "Datum od ne može biti posle datuma do."
                    }
                );
            }

            try
            {
                List<UgovorModel> ugovori =
                    _repo.FiltrirajUgovore(
                        brojUgovora,
                        klijent,
                        statusUgovora,
                        datumOd,
                        datumDo
                    );

                return Ok(ugovori);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        poruka = "Greška prilikom filtriranja ugovora.",
                        greska = ex.Message
                    }
                );
            }
        }

        [HttpGet("{id:int}")]
        public ActionResult<UgovorModel> DajUgovorPoId(int id)
        {
            try
            {
                UgovorModel? ugovor =
                    _repo.DajUgovorPoId(id);

                if (ugovor == null)
                {
                    return NotFound(
                        new
                        {
                            poruka = $"Ugovor sa ID vrednošću {id} nije pronađen."
                        }
                    );
                }

                return Ok(ugovor);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        poruka = "Greška prilikom učitavanja ugovora.",
                        greska = ex.Message
                    }
                );
            }
        }

        [HttpGet("dostupnost")]
        public ActionResult ProveriDostupnostVozila(
    [FromQuery] int voziloId,
    [FromQuery] DateTime datumPreuzimanja,
    [FromQuery] DateTime datumVracanja)
        {
            if (voziloId <= 0)
            {
                return BadRequest(
                    new { poruka = "VoziloId mora biti veći od nule." }
                );
            }

            if (datumVracanja <= datumPreuzimanja)
            {
                return BadRequest(
                    new
                    {
                        poruka =
                            "Datum vraćanja mora biti posle datuma preuzimanja."
                    }
                );
            }

            try
            {
                bool dostupno = _repo.DaLiJeVoziloDostupno(
                    voziloId,
                    datumPreuzimanja,
                    datumVracanja
                );

                return Ok(
                    new
                    {
                        voziloId,
                        datumPreuzimanja,
                        datumVracanja,
                        dostupno,
                        poruka = dostupno
                            ? "Vozilo je dostupno u izabranom periodu."
                            : "Vozilo nije dostupno u izabranom periodu."
                    }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        poruka = "Greška prilikom provere dostupnosti vozila.",
                        greska = ex.Message
                    }
                );
            }
        }

        [HttpGet("klijent/{klijentId:int}/broj-zavrsenih")]
        public ActionResult DajBrojZavrsenihUgovoraKlijenta(
    int klijentId)
        {
            if (klijentId <= 0)
            {
                return BadRequest(
                    new { poruka = "KlijentId mora biti veći od nule." }
                );
            }

            try
            {
                int brojUgovora =
                    _repo.DajBrojZavrsenihUgovoraKlijenta(klijentId);

                return Ok(
                    new
                    {
                        klijentId,
                        brojZavrsenihUgovora = brojUgovora
                    }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        poruka =
                            "Greška prilikom učitavanja broja završenih ugovora.",
                        greska = ex.Message
                    }
                );
            }
        }

        [HttpPost]
        public ActionResult DodajUgovor(
            [FromBody] UgovorModel ugovor)
        {
            if (ugovor == null)
            {
                return BadRequest(
                    new { poruka = "Podaci o ugovoru nisu poslati." }
                );
            }

            if (ugovor.StavkeUgovora == null ||
                ugovor.StavkeUgovora.Count == 0)
            {
                return BadRequest(
                    new
                    {
                        poruka = "Ugovor mora imati najmanje jednu stavku."
                    }
                );
            }

            if (ugovor.DatumVracanja <= ugovor.DatumPreuzimanja)
            {
                return BadRequest(
                    new
                    {
                        poruka = "Datum vraćanja mora biti posle datuma preuzimanja."
                    }
                );
            }

            try
            {
                int noviId = _repo.DodajUgovor(ugovor);

                return CreatedAtAction(
                    nameof(DajUgovorPoId),
                    new { id = noviId },
                    new
                    {
                        id = noviId,
                        poruka = "Ugovor je uspešno dodat."
                    }
                );
            }
            catch (SqlException ex)
            {
                return BadRequest(
                    new
                    {
                        poruka = ex.Message
                    }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        poruka = "Greška prilikom dodavanja ugovora.",
                        greska = ex.Message
                    }
                );
            }
        }

        [HttpPut("{id:int}/status")]
        public ActionResult PromeniStatus(
            int id,
            [FromBody] PromenaStatusaUgovoraModel zahtev)
        {
            if (zahtev == null ||
                string.IsNullOrWhiteSpace(zahtev.NoviStatus))
            {
                return BadRequest(
                    new { poruka = "Novi status je obavezan." }
                );
            }

            string[] dozvoljeniStatusi =
            {
                "Aktivan",
                "Završen",
                "Otkazan"
            };

            if (!dozvoljeniStatusi.Contains(
                    zahtev.NoviStatus,
                    StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(
                    new
                    {
                        poruka =
                            "Dozvoljeni statusi su: Aktivan, Završen i Otkazan."
                    }
                );
            }

            try
            {
                UgovorModel? postojeciUgovor =
                    _repo.DajUgovorPoId(id);

                if (postojeciUgovor == null)
                {
                    return NotFound(
                        new
                        {
                            poruka =
                                $"Ugovor sa ID vrednošću {id} nije pronađen."
                        }
                    );
                }

                _repo.PromeniStatusUgovora(
                    id,
                    zahtev.NoviStatus
                );

                return Ok(
                    new
                    {
                        poruka = "Status ugovora je uspešno promenjen.",
                        ugovorId = id,
                        noviStatus = zahtev.NoviStatus
                    }
                );
            }
            catch (SqlException ex)
            {
                return BadRequest(
                    new { poruka = ex.Message }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        poruka = "Greška prilikom promene statusa.",
                        greska = ex.Message
                    }
                );
            }
        }

        [HttpDelete("{id:int}")]
        public ActionResult ObrisiUgovor(int id)
        {
            try
            {
                UgovorModel? postojeciUgovor =
                    _repo.DajUgovorPoId(id);

                if (postojeciUgovor == null)
                {
                    return NotFound(
                        new
                        {
                            poruka =
                                $"Ugovor sa ID vrednošću {id} nije pronađen."
                        }
                    );
                }

                _repo.ObrisiUgovor(id);

                return Ok(
                    new
                    {
                        poruka = "Ugovor je uspešno obrisan."
                    }
                );
            }
            catch (SqlException ex)
            {
                return BadRequest(
                    new { poruka = ex.Message }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        poruka = "Greška prilikom brisanja ugovora.",
                        greska = ex.Message
                    }
                );
            }
        }
    }
}