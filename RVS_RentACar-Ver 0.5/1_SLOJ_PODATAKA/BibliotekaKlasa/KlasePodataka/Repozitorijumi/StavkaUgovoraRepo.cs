using BibliotekaKlasa.KlasePodataka.Modeli;
using BibliotekaKlasa.TehnoloskeKlase;
using System;
using System.Collections.Generic;
using System.Data;

namespace BibliotekaKlasa.KlasePodataka.Repozitorijumi
{
    public class StavkaUgovoraRepo : TabelaKlasa
    {
        public StavkaUgovoraRepo(KonekcijaKlasa konekcijaObjekat)
            : base(konekcijaObjekat, "StavkeUgovora")
        {
        }

        public void Dodaj(StavkaUgovoraModel stavka)
        {
            string upit =
                "INSERT INTO StavkeUgovora " +
                "(UgovorId, VoziloId, BrojDana, CenaPoDanu, " +
                "PopustProcenat, Ukupno) VALUES (" +
                stavka.UgovorId + ", " +
                stavka.VoziloId + ", " +
                stavka.BrojDana + ", " +
                stavka.CenaPoDanu.ToString(
                    System.Globalization.CultureInfo.InvariantCulture) + ", " +
                stavka.PopustProcenat.ToString(
                    System.Globalization.CultureInfo.InvariantCulture) + ", " +
                stavka.Ukupno.ToString(
                    System.Globalization.CultureInfo.InvariantCulture) +
                ")";

            IzvrsiAzuriranje(upit);
        }

        public void Izmeni(StavkaUgovoraModel stavka)
        {
            string upit =
                "UPDATE StavkeUgovora SET " +
                "UgovorId=" + stavka.UgovorId + ", " +
                "VoziloId=" + stavka.VoziloId + ", " +
                "BrojDana=" + stavka.BrojDana + ", " +
                "CenaPoDanu=" +
                stavka.CenaPoDanu.ToString(
                    System.Globalization.CultureInfo.InvariantCulture) + ", " +
                "PopustProcenat=" +
                stavka.PopustProcenat.ToString(
                    System.Globalization.CultureInfo.InvariantCulture) + ", " +
                "Ukupno=" +
                stavka.Ukupno.ToString(
                    System.Globalization.CultureInfo.InvariantCulture) +
                " WHERE Id=" + stavka.Id;

            IzvrsiAzuriranje(upit);
        }

        public void Obrisi(int id)
        {
            string upit =
                "DELETE FROM StavkeUgovora WHERE Id=" + id;

            IzvrsiAzuriranje(upit);
        }

        public List<StavkaUgovoraModel> DajSve()
        {
            string upit = @"
                SELECT
                    su.Id,
                    su.UgovorId,
                    su.VoziloId,
                    su.BrojDana,
                    su.CenaPoDanu,
                    su.PopustProcenat,
                    su.Ukupno,
                    v.Marka,
                    v.Model,
                    v.Registracija,
                    v.StatusVozila
                FROM StavkeUgovora su
                INNER JOIN Vozila v
                    ON su.VoziloId = v.Id";

            return PretvoriUListu(DajPodatke(upit));
        }

        public List<StavkaUgovoraModel> DajSvePoUgovoruId(
            int ugovorId)
        {
            string upit = @"
                SELECT
                    su.Id,
                    su.UgovorId,
                    su.VoziloId,
                    su.BrojDana,
                    su.CenaPoDanu,
                    su.PopustProcenat,
                    su.Ukupno,
                    v.Marka,
                    v.Model,
                    v.Registracija,
                    v.StatusVozila
                FROM StavkeUgovora su
                INNER JOIN Vozila v
                    ON su.VoziloId = v.Id
                WHERE su.UgovorId = " + ugovorId;

            return PretvoriUListu(DajPodatke(upit));
        }

        private static List<StavkaUgovoraModel> PretvoriUListu(
            DataSet podaci)
        {
            List<StavkaUgovoraModel> stavke =
                new List<StavkaUgovoraModel>();

            if (podaci.Tables.Count == 0)
            {
                return stavke;
            }

            foreach (DataRow red in podaci.Tables[0].Rows)
            {
                stavke.Add(new StavkaUgovoraModel
                {
                    Id = Convert.ToInt32(red[0]),
                    UgovorId = Convert.ToInt32(red[1]),
                    VoziloId = Convert.ToInt32(red[2]),
                    BrojDana = Convert.ToInt32(red[3]),
                    CenaPoDanu = Convert.ToDecimal(red[4]),
                    PopustProcenat = Convert.ToDecimal(red[5]),
                    Ukupno = Convert.ToDecimal(red[6]),

                    VoziloObjekat = new VoziloModel
                    {
                        Id = Convert.ToInt32(red[2]),
                        Marka = red[7].ToString(),
                        Model = red[8].ToString(),
                        Registracija = red[9].ToString(),
                        StatusVozila = red[10].ToString()
                    }
                });
            }

            return stavke;
        }
    }
}