namespace PoslovnaLogika
{
    // Uloga klase: RezultatObradeUgovora grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class RezultatObradeUgovora
    {
        public bool DozvoljenoKreiranjeUgovora { get; set; }

        public int BrojDanaIznajmljivanja { get; set; }

        public int BrojDanaKasnjenja { get; set; }

        public decimal OsnovniIznos { get; set; }

        public decimal PopustProcenat { get; set; }

        public decimal IznosPopusta { get; set; }

        public decimal TrosakFullOsiguranja { get; set; }

        public decimal TrosakKasnjenja { get; set; }

        public decimal UkupanIznos { get; set; }

        public List<string> Poruke { get; set; } = new();
    }
}
