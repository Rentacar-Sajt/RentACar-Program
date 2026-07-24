namespace BibliotekaKlasa.KlasePodataka.Modeli
{
    public class VoziloModel
    {
        public int Id { get; set; }

        public string? Marka { get; set; }

        public string? Model { get; set; }

        public string? Registracija { get; set; }

        public decimal CenaPoDanu { get; set; }

        public string? StatusVozila { get; set; }
    }
}