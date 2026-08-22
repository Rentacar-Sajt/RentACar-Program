namespace BibliotekaKlasa.KlasePodataka.Modeli
{
    // Uloga klase: KorisnikModel grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class KorisnikModel : OsobaModel
    {
        public string? LozinkaHash { get; set; }

        public string? LozinkaSalt { get; set; }

        public string? Uloga { get; set; }
    }
}
