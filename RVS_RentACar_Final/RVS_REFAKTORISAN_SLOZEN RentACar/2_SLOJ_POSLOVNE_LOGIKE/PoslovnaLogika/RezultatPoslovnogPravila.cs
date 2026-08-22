namespace PoslovnaLogika
{
    // Uloga klase: RezultatPoslovnogPravila grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class RezultatPoslovnogPravila
    {
        public bool Uspesno { get; set; }

        public string Poruka { get; set; } = string.Empty;

        public decimal PocetniIznos { get; set; }

        public decimal IznosPopusta { get; set; }

        public decimal TrosakKasnjenja { get; set; }

        public decimal TrosakFullOsiguranja { get; set; }

        public decimal KonacniIznos { get; set; }
    }
}
