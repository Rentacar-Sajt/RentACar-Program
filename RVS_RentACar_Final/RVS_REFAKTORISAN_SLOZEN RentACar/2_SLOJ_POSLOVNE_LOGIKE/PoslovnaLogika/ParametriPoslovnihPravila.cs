namespace PoslovnaLogika
{
    // Uloga klase: ParametriPoslovnihPravila grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class ParametriPoslovnihPravila
    {
        public int MaksimalanBrojPreklapanjaUgovora { get; set; }

        public int MinimalanBrojDanaVazenjaDozvoleNakonZavrsetka { get; set; }

        public int MinimalanBrojDanaZaPopust { get; set; }

        public decimal ProcenatPopusta { get; set; }

        public int DozvoljeniBrojSatiKasnjenja { get; set; }

        public decimal DodatniTrosakPoDanuKasnjenja { get; set; }

        public decimal CenaFullOsiguranjaPoDanu { get; set; }
    }
}
