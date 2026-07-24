namespace PoslovnaLogika.Klase
{
    public class ZahtevZaObraduUgovora
    {
        public int VoziloId { get; set; }

        public DateTime DatumPreuzimanja { get; set; }

        public DateTime DatumVracanja { get; set; }

        public DateTime DatumIstekaVozackeDozvole { get; set; }

        public decimal CenaPoDanu { get; set; }

        public bool IzabranoFullOsiguranje { get; set; }

        public DateTime? StvarniDatumVracanja { get; set; }
    }
}