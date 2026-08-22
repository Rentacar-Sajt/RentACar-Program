namespace RVS_REST_API.Modeli
{
    // Uloga klase: LoginZahtev grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class LoginZahtev
    {
        public string Email { get; set; } = string.Empty;

        public string Lozinka { get; set; } = string.Empty;
    }
}
