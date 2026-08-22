namespace BibliotekaKlasa.KlasePodataka.Modeli
{
    // Uloga klase: KlijentModel grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class KlijentModel : OsobaModel
    {
        public string? JMBG { get; set; }

        public string? BrojPasosa { get; set; }

        public string? BrojVozackeDozvole { get; set; }

        public DateTime DatumIzdavanjaDozvole { get; set; }

        public string? Telefon { get; set; }

        public string? Adresa { get; set; }

        public DateTime? DatumIstekaVozackeDozvole { get; set; }
    }
}
