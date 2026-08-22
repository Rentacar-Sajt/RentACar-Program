namespace BibliotekaKlasa.KlasePodataka.Modeli
{
    // Uloga klase: VoziloModel grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class VoziloModel
    {
        public int Id { get; set; }
        public string? Marka { get; set; }
        public string? Model { get; set; }
        public string? Registracija { get; set; }
        public decimal CenaPoDanu { get; set; }
        public string? StatusVozila { get; set; }
        public int? Godiste { get; set; }
        public string? Gorivo { get; set; }
        public string? Menjac { get; set; }
        public int? Kilometraza { get; set; }
        public string? Boja { get; set; }
        public int? BrojSedista { get; set; }
        public decimal? ZapreminaMotora { get; set; }
        public int? SnagaMotora { get; set; }
        public string? SlikaPutanja { get; set; }
    }
}
