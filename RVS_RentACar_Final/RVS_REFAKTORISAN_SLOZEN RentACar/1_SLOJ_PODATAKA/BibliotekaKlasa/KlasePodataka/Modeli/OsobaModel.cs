namespace BibliotekaKlasa.KlasePodataka.Modeli
{
    // Uloga klase: OsobaModel grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public abstract class OsobaModel
    {
        public int Id { get; set; }

        public string? Ime { get; set; }

        public string? Prezime { get; set; }

        public string? Email { get; set; }
    }
}
