namespace RVS_REST_API.Modeli
{
    public class ParametriPoslovnihPravila
    {
        // Pravilo 1 – dostupnost vozila
        public int MaksimalanBrojPreklapanjaUgovora { get; set; }

        // Pravilo 2 – važenje vozačke dozvole
        public int MinimalanBrojDanaVazenjaDozvoleNakonZavrsetka { get; set; }

        // Pravilo 3 – popust
        public int MinimalanBrojDanaZaPopust { get; set; }
        public decimal ProcenatPopusta { get; set; }

        // Pravilo 4 – kašnjenje
        public int DozvoljeniBrojSatiKasnjenja { get; set; }
        public decimal DodatniTrosakPoDanuKasnjenja { get; set; }

        // Pravilo 5 – Full osiguranje
        public decimal CenaFullOsiguranjaPoDanu { get; set; }
    }
}