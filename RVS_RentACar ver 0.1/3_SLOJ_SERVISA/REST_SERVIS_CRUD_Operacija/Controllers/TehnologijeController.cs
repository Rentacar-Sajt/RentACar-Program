using BibliotekaKlasa.KlasePodataka.Modeli;
using BibliotekaKlasa.KlasePodataka.Repozitorijumi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace REST_SERVIS_CRUD_Operacija.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TehnologijeController : ControllerBase
    {
        private readonly TehnologijaSPRepo _tehnologijaSPRepoObjekat;

        public TehnologijeController(IConfiguration configuration)
        {
            string konekcioniString = configuration["KonekcioniString:Default"];
            _tehnologijaSPRepoObjekat = new TehnologijaSPRepo(konekcioniString);
        }

        // GET: api/Tehnologije
        [HttpGet]
        public ActionResult<List<TehnologijaModel>> DajSve()
        {
            var listaTehnologijaObjekata = _tehnologijaSPRepoObjekat.DajSveTehnologije();
            return Ok(listaTehnologijaObjekata);
        }

        // GET: api/Tehnologije/naziv
        [HttpGet("naziv/{naziv}")]
        public ActionResult<List<TehnologijaModel>> DajPoNazivu(string naziv)
        {
            var listaTehnologijaObjekata = _tehnologijaSPRepoObjekat.DajTehnologijuPoNazivu(naziv);
            if (listaTehnologijaObjekata.Count == 0)
                return NotFound();
            return Ok(listaTehnologijaObjekata);
        }

        [HttpGet("{id}")]
        public ActionResult<TehnologijaModel> DajPoId(int id)
        {
            var tehnologijaObjekat = _tehnologijaSPRepoObjekat.DajTehnologijuPoId(id);
            if (tehnologijaObjekat == null)
                return NotFound();
            return Ok(tehnologijaObjekat);
        }

        // POST: api/Tehnologije
        [HttpPost]
        public ActionResult Dodaj([FromBody] TehnologijaModel tehnologijaObjekat)
        {
            bool uspeh = _tehnologijaSPRepoObjekat.DodajNovuTehnologiju(tehnologijaObjekat);
            if (uspeh)
                return Ok(tehnologijaObjekat);
            return BadRequest();
        }

        // PUT: api/Tehnologije/5
        [HttpPut("{id}")]
        public ActionResult Izmeni(int id, [FromBody] TehnologijaModel tehnologijaObjekat)
        {
            if (id != tehnologijaObjekat.Id)
                return BadRequest();

            bool uspeh = _tehnologijaSPRepoObjekat.IzmeniTehnologiju(tehnologijaObjekat);
            if (uspeh)
                return NoContent();

            return NotFound();
        }

        // DELETE: api/Tehnologije/5
        [HttpDelete("{id}")]
        public ActionResult Obrisi(int id)
        {
            bool uspeh = _tehnologijaSPRepoObjekat.ObrisiTehnologiju(id);
            if (uspeh)
                return NoContent();
            return NotFound();
        }
    }
}