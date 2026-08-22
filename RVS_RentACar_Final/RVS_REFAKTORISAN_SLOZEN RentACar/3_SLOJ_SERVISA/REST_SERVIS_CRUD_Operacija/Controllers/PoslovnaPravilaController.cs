using Microsoft.AspNetCore.Mvc;
using PoslovnaLogika;
using PoslovnaLogika.Servisi;

namespace REST_SERVIS_CRUD_Operacija.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	// Uloga klase: PoslovnaPravilaController grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
	public class PoslovnaPravilaController : ControllerBase
	{
		private readonly ObradaPoslovnihPravila _obrada;
		private readonly ParametriPoslovnihPravilaServis _parametriServis;

		public PoslovnaPravilaController(
			ObradaPoslovnihPravila obrada,
			ParametriPoslovnihPravilaServis parametriServis)
		{
			_obrada = obrada;
			_parametriServis = parametriServis;
		}

		[HttpPost("obradi-ugovor")]
		// Obrađuje poslovna pravila ugovora: proverava dostupnost vozila i uslove najma, zatim obračunava cenu, popust, osiguranje i eventualno kašnjenje.
		public async Task<ActionResult<RezultatObradeUgovora>> ObradiUgovor(
			ZahtevZaObraduUgovora zahtev)
		{
			ParametriPoslovnihPravila parametri =
				await _parametriServis.DajParametreAsync();

			RezultatObradeUgovora rezultat =
				_obrada.ObradiUgovor(zahtev, parametri);

			return Ok(rezultat);
		}
	}
}
