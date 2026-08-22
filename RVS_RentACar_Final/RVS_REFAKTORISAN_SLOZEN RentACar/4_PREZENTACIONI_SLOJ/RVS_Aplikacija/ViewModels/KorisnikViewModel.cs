namespace RVS_Aplikacija.ViewModels
{
    // Uloga klase: KorisnikViewModel grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class KorisnikViewModel
    {
        public int Id { get; set; }

        public string Ime { get; set; } = string.Empty;

        public string Prezime { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Uloga { get; set; } = string.Empty;

        public string PunoIme
        {
            get
            {
                return $"{Ime} {Prezime}".Trim();
            }
        }
    }
}
