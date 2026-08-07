using BibliotekaKlasa.KlasePodataka.Repozitorijumi;
using PoslovnaLogika;

namespace PoslovnaLogika
{
    public class ObradaPoslovnihPravila
    {
        private readonly UgovorRepo _ugovorRepo;

        public ObradaPoslovnihPravila(UgovorRepo ugovorRepo)
        {
            _ugovorRepo = ugovorRepo;
        }

        public RezultatObradeUgovora ObradiUgovor(
            ZahtevZaObraduUgovora zahtev,
            ParametriPoslovnihPravila parametri)
        {
            ProveriUlaznePodatke(zahtev, parametri);

            RezultatObradeUgovora rezultat =
                new RezultatObradeUgovora();

            bool voziloJeDostupno =
                _ugovorRepo.DaLiJeVoziloDostupno(
                    zahtev.VoziloId,
                    zahtev.DatumPreuzimanja,
                    zahtev.DatumVracanja);

            if (!voziloJeDostupno)
            {
                rezultat.DozvoljenoKreiranjeUgovora = false;
                rezultat.Poruke.Add(
                    "Vozilo nije dostupno u traženom periodu.");

                return rezultat;
            }

            DateTime minimalniDatum =
    zahtev.DatumVracanja.AddDays(
        parametri.MinimalanBrojDanaVazenjaDozvoleNakonZavrsetka);

            if (zahtev.DatumIstekaVozackeDozvole.Date <
                zahtev.DatumVracanja.Date)
            {
                rezultat.DozvoljenoKreiranjeUgovora = false;
                rezultat.Poruke.Add(
                    "Vozačka dozvola ne važi tokom celog perioda iznajmljivanja.");

                return rezultat;
            }

            rezultat.DozvoljenoKreiranjeUgovora = true;

            rezultat.BrojDanaIznajmljivanja =
                IzracunajBrojDana(
                    zahtev.DatumPreuzimanja,
                    zahtev.DatumVracanja);

            rezultat.OsnovniIznos =
                rezultat.BrojDanaIznajmljivanja *
                zahtev.CenaPoDanu;

            ObracunajPopust(
                rezultat,
                parametri);

            ObracunajFullOsiguranje(
                rezultat,
                zahtev,
                parametri);

            ObracunajKasnjenje(
                rezultat,
                zahtev,
                parametri);

            rezultat.UkupanIznos =
                rezultat.OsnovniIznos
                - rezultat.IznosPopusta
                + rezultat.TrosakFullOsiguranja
                + rezultat.TrosakKasnjenja;

            rezultat.UkupanIznos =
                Math.Round(rezultat.UkupanIznos, 2);

            rezultat.Poruke.Add(
                "Sva poslovna pravila su uspešno obrađena.");

            return rezultat;
        }

        private static void ObracunajPopust(
            RezultatObradeUgovora rezultat,
            ParametriPoslovnihPravila parametri)
        {
            if (rezultat.BrojDanaIznajmljivanja >
                parametri.MinimalanBrojDanaZaPopust)
            {
                rezultat.PopustProcenat =
                    parametri.ProcenatPopusta;

                rezultat.IznosPopusta =
                    Math.Round(
                        rezultat.OsnovniIznos *
                        rezultat.PopustProcenat / 100,
                        2);

                rezultat.Poruke.Add(
                    $"Obračunat je popust od " +
                    $"{rezultat.PopustProcenat:N2}%.");
            }
        }

        private static void ObracunajFullOsiguranje(
            RezultatObradeUgovora rezultat,
            ZahtevZaObraduUgovora zahtev,
            ParametriPoslovnihPravila parametri)
        {
            if (!zahtev.IzabranoFullOsiguranje)
            {
                return;
            }

            rezultat.TrosakFullOsiguranja =
                Math.Round(
                    rezultat.BrojDanaIznajmljivanja *
                    parametri.CenaFullOsiguranjaPoDanu,
                    2);

            rezultat.Poruke.Add(
                $"Dodat je trošak Full osiguranja u iznosu od " +
                $"{rezultat.TrosakFullOsiguranja:N2} dinara.");
        }

        private static void ObracunajKasnjenje(
            RezultatObradeUgovora rezultat,
            ZahtevZaObraduUgovora zahtev,
            ParametriPoslovnihPravila parametri)
        {
            if (!zahtev.StvarniDatumVracanja.HasValue)
            {
                return;
            }

            TimeSpan kasnjenje =
                zahtev.StvarniDatumVracanja.Value - zahtev.DatumVracanja;

            if (kasnjenje.TotalHours <=
                parametri.DozvoljeniBrojSatiKasnjenja)
            {
                return;
            }

            rezultat.BrojDanaKasnjenja =
                IzracunajBrojDana(
                    zahtev.DatumVracanja,
                    zahtev.StvarniDatumVracanja.Value);

            rezultat.TrosakKasnjenja =
    Math.Round(
        rezultat.BrojDanaKasnjenja *
        parametri.DodatniTrosakPoDanuKasnjenja,
        2);

            rezultat.Poruke.Add(
                $"Vozilo kasni {rezultat.BrojDanaKasnjenja} dana. " +
                $"Dodatni trošak iznosi " +
                $"{rezultat.TrosakKasnjenja:N2} dinara.");
        }

        private static int IzracunajBrojDana(
            DateTime pocetniDatum,
            DateTime krajnjiDatum)
        {
            double brojDana =
                (krajnjiDatum - pocetniDatum).TotalDays;

            return Math.Max(
                1,
                (int)Math.Ceiling(brojDana));
        }

        private static void ProveriUlaznePodatke(
            ZahtevZaObraduUgovora zahtev,
            ParametriPoslovnihPravila parametri)
        {
            ArgumentNullException.ThrowIfNull(zahtev);
            ArgumentNullException.ThrowIfNull(parametri);

            if (zahtev.VoziloId <= 0)
            {
                throw new ArgumentException(
                    "Identifikator vozila mora biti veći od nule.");
            }

            if (zahtev.DatumVracanja <= zahtev.DatumPreuzimanja)
            {
                throw new ArgumentException(
                    "Datum vraćanja mora biti posle datuma preuzimanja.");
            }

            if (zahtev.CenaPoDanu <= 0)
            {
                throw new ArgumentException(
                    "Cena po danu mora biti veća od nule.");
            }

            if (parametri.MinimalanBrojDanaZaPopust < 0)
            {
                throw new ArgumentException(
                    "Minimalan broj dana za popust nije ispravan.");
            }

            if (parametri.ProcenatPopusta < 0 ||
                parametri.ProcenatPopusta > 100)
            {
                throw new ArgumentException(
                    "Procenat popusta mora biti između 0 i 100.");
            }

            if (parametri.DodatniTrosakPoDanuKasnjenja < 0)
            {
                throw new ArgumentException(
                    "Cena kašnjenja po danu ne može biti negativna.");
            }

            if (parametri.CenaFullOsiguranjaPoDanu < 0)
            {
                throw new ArgumentException(
                    "Cena Full osiguranja ne može biti negativna.");
            }
        }
    }
}