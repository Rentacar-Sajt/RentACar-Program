using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RVS_MVC.Filteri
{
    // Uloga klase: AdminAutorizacijaFilter grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class AdminAutorizacijaFilter : ActionFilterAttribute
    {
        // Pre izvršavanja MVC akcije proverava podatke iz sesije i dozvoljava pristup samo prijavljenom administratoru.
        public override void OnActionExecuting(
            ActionExecutingContext context)
        {
            int? korisnikId =
                context.HttpContext.Session
                    .GetInt32("KorisnikId");

            string? uloga =
                context.HttpContext.Session
                    .GetString("Uloga");

            bool administratorPrijavljen =
                korisnikId.HasValue &&
                string.Equals(
                    uloga,
                    "Administrator",
                    StringComparison.OrdinalIgnoreCase
                );

            if (!administratorPrijavljen)
            {
                context.Result =
                    new RedirectToActionResult(
                        "Login",
                        "Autentikacija",
                        null
                    );

                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
