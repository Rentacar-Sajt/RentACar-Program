namespace BibliotekaKlasa.KlasePodataka.Modeli
{
    public class StavkaUgovoraModel
    {
        public int Id { get; set; }

        public int UgovorId { get; set; }

        public int VoziloId { get; set; }

        public int BrojDana { get; set; }

        public decimal CenaPoDanu { get; set; }

        public decimal PopustProcenat { get; set; }

        public decimal Ukupno { get; set; }

        public VoziloModel? VoziloObjekat { get; set; }
    }
}