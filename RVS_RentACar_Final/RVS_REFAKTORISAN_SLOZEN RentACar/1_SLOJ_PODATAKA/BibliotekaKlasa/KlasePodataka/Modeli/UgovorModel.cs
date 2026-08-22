namespace BibliotekaKlasa.KlasePodataka.Modeli
{
    // Uloga klase: UgovorModel grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class UgovorModel
    {
        public int Id { get; set; }

        public string? BrojUgovora { get; set; }

        public DateTime DatumIzdavanja { get; set; }

        public DateTime DatumPreuzimanja { get; set; }

        public DateTime DatumVracanja { get; set; }

        public DateTime? StvarniDatumVracanja { get; set; }

        public int BrojDanaKasnjenja { get; set; }

        public decimal KaznaZaKasnjenje { get; set; }

        public string? MestoPreuzimanja { get; set; }

        public string? MestoVracanja { get; set; }

        public string? NacinPlacanja { get; set; }

        public decimal Depozit { get; set; }

        public string? StatusUgovora { get; set; }

        public string? Napomena { get; set; }

        public decimal PopustProcenat { get; set; }

        public decimal UkupnoZaPlacanje { get; set; }

        public int KlijentId { get; set; }

        public int KorisnikId { get; set; }

        public string? IzabraneDodatneUsluge { get; set; }

        public KlijentModel? KlijentObjekat { get; set; }

        public KorisnikModel? KorisnikObjekat { get; set; }

        public List<StavkaUgovoraModel>? StavkeUgovora { get; set; }
    }
}
