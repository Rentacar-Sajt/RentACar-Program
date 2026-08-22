using System;
using System.Collections.Generic;

namespace RVS_Aplikacija.ViewModels
{
    // Uloga klase: DashboardViewModel grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class DashboardViewModel
    {
        public int BrojVozila { get; set; }
        public int SlobodnaVozila { get; set; }
        public int ZauzetaVozila { get; set; }
        public int VozilaNaServisu { get; set; }
        public int BrojKlijenata { get; set; }
        public int AktivniUgovori { get; set; }
        public int PreuzimanjaDanas { get; set; }
        public int VracanjaDanas { get; set; }
        public int ZakasnelaVracanja { get; set; }
        public decimal PrihodOvogMeseca { get; set; }
        public DateTime GenerisanoU { get; set; } = DateTime.Now;

        public List<DashboardUgovorStavkaViewModel> DanasnjaPreuzimanja { get; set; }
            = new List<DashboardUgovorStavkaViewModel>();

        public List<DashboardUgovorStavkaViewModel> DanasnjaVracanja { get; set; }
            = new List<DashboardUgovorStavkaViewModel>();

        public List<DashboardUgovorStavkaViewModel> PoslednjiUgovori { get; set; }
            = new List<DashboardUgovorStavkaViewModel>();
    }

    // Uloga klase: DashboardUgovorStavkaViewModel grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class DashboardUgovorStavkaViewModel
    {
        public int Id { get; set; }
        public string BrojUgovora { get; set; } = string.Empty;
        public string Klijent { get; set; } = string.Empty;
        public string Vozilo { get; set; } = string.Empty;
        public DateTime DatumPreuzimanja { get; set; }
        public DateTime DatumVracanja { get; set; }
        public string StatusUgovora { get; set; } = string.Empty;
        public decimal UkupnoZaPlacanje { get; set; }
        public bool Kasni { get; set; }
    }
}
