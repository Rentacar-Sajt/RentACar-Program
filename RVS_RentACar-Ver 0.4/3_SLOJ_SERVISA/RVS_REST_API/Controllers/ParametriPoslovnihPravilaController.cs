using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using RVS_REST_API.Modeli;

namespace RVS_REST_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParametriPoslovnihPravilaController : ControllerBase
    {
        private readonly IWebHostEnvironment _okruzenje;

        public ParametriPoslovnihPravilaController(
            IWebHostEnvironment okruzenje)
        {
            _okruzenje = okruzenje;
        }

        [HttpGet]
        public ActionResult<ParametriPoslovnihPravila>
            DajParametrePoslovnihPravila()
        {
            string putanja = Path.Combine(
                _okruzenje.ContentRootPath,
                "Parametri",
                "poslovnaPravila.json"
            );

            if (!System.IO.File.Exists(putanja))
            {
                return NotFound(
                    "JSON fajl sa parametrima poslovnih pravila nije pronađen."
                );
            }

            string jsonSadrzaj =
                System.IO.File.ReadAllText(putanja);

            ParametriPoslovnihPravila? parametri =
                JsonSerializer.Deserialize<ParametriPoslovnihPravila>(
                    jsonSadrzaj,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }
                );

            if (parametri == null)
            {
                return BadRequest(
                    "Parametri poslovnih pravila nisu ispravno definisani."
                );
            }

            return Ok(parametri);
        }
    }
}