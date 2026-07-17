namespace BibliotekaKlasa.KlasePodataka.Modeli
{
    public abstract class OsobaModel
    {
        public int Id { get; set; }

        public string? Ime { get; set; }

        public string? Prezime { get; set; }

        public string? Email { get; set; }
    }
}