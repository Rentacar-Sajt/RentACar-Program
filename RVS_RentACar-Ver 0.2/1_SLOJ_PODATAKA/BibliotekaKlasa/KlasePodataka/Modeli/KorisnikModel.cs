namespace BibliotekaKlasa.KlasePodataka.Modeli
{
    public class KorisnikModel : OsobaModel
    {
        public string? LozinkaHash { get; set; }

        public string? LozinkaSalt { get; set; }

        public string? Uloga { get; set; }
    }
}