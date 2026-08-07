using Microsoft.AspNetCore.Mvc;
using PoslovnaLogika;
using PoslovnaLogika.Servisi;

namespace REST_SERVIS_CRUD_Operacija.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
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