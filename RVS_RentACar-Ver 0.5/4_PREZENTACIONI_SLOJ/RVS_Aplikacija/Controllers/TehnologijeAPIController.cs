using Microsoft.AspNetCore.Mvc;

namespace RVS_Aplikacija.Controllers
{
    public class TehnologijeAPIController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dodaj()
        {
            return View();
        }
        public IActionResult Izmeni(int id)
        {
            // prosledjuje se samo ID view-u, REST ide preko JS
            ViewData["Id"] = id;
            return View();
        }
    }
}
