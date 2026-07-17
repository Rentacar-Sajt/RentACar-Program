namespace BibliotekaKlasa.KlasePodataka.Modeli
{
    public class KlijentModel : OsobaModel
    {
        public string? JMBG { get; set; }

        public string? BrojPasosa { get; set; }

        public string? BrojVozackeDozvole { get; set; }

        public DateTime DatumIzdavanjaDozvole { get; set; }

        public string? Telefon { get; set; }

        public string? Adresa { get; set; }
    }
}