using BibliotekaKlasa.KlasePodataka.Modeli;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace BibliotekaKlasa.KlasePodataka.Repozitorijumi
{
    public class UgovorRepo
    {
        private readonly string _stringKonekcije;

        public UgovorRepo(string stringKonekcije)
        {
            _stringKonekcije = stringKonekcije;
        }

        public List<UgovorModel> DajSveUgovore()
        {
            List<UgovorModel> ugovori = new List<UgovorModel>();

            using SqlConnection konekcija =
                new SqlConnection(_stringKonekcije);

            using SqlCommand komanda =
                new SqlCommand("spDajSveUgovore", konekcija);

            komanda.CommandType = CommandType.StoredProcedure;

            konekcija.Open();

            using SqlDataReader citac = komanda.ExecuteReader();

            while (citac.Read())
            {
                ugovori.Add(MapirajUgovor(citac));
            }

            return ugovori;
        }

        public UgovorModel? DajUgovorPoId(int id)
        {
            using SqlConnection konekcija =
                new SqlConnection(_stringKonekcije);

            using SqlCommand komanda =
                new SqlCommand("spDajUgovorPoId", konekcija);

            komanda.CommandType = CommandType.StoredProcedure;
            komanda.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            konekcija.Open();

            using SqlDataReader citac = komanda.ExecuteReader();

            UgovorModel? ugovor = null;

            if (citac.Read())
            {
                ugovor = MapirajUgovor(citac);
                ugovor.StavkeUgovora =
                    new List<StavkaUgovoraModel>();
            }

            if (ugovor != null && citac.NextResult())
            {
                while (citac.Read())
                {
                    ugovor.StavkeUgovora!.Add(
                        new StavkaUgovoraModel
                        {
                            Id = citac.GetInt32(
                                citac.GetOrdinal("Id")),

                            UgovorId = citac.GetInt32(
                                citac.GetOrdinal("UgovorId")),

                            VoziloId = citac.GetInt32(
                                citac.GetOrdinal("VoziloId")),

                            BrojDana = citac.GetInt32(
                                citac.GetOrdinal("BrojDana")),

                            CenaPoDanu = citac.GetDecimal(
                                citac.GetOrdinal("CenaPoDanu")),

                            PopustProcenat = citac.GetDecimal(
                                citac.GetOrdinal("PopustProcenat")),

                            Ukupno = citac.GetDecimal(
                                citac.GetOrdinal("Ukupno")),

                            VoziloObjekat = new VoziloModel
                            {
                                Id = citac.GetInt32(
                                    citac.GetOrdinal("VoziloId")),

                                Marka = citac.GetString(
                                    citac.GetOrdinal("Marka")),

                                Model = citac.GetString(
                                    citac.GetOrdinal("Model")),

                                Registracija = citac.GetString(
                                    citac.GetOrdinal("Registracija"))
                            }
                        });
                }
            }

            return ugovor;
        }

        public bool ObrisiUgovor(int id)
        {
            using SqlConnection konekcija =
                new SqlConnection(_stringKonekcije);

            using SqlCommand komanda =
                new SqlCommand("spObrisiUgovor", konekcija);

            komanda.CommandType = CommandType.StoredProcedure;
            komanda.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            konekcija.Open();

            return komanda.ExecuteNonQuery() >= 0;
        }

        public int DajBrojZavrsenihUgovoraKlijenta(
            int klijentId)
        {
            const string upit = @"
                SELECT COUNT(*)
                FROM Ugovori
                WHERE KlijentId = @KlijentId
                  AND StatusUgovora = N'Završen'";

            using SqlConnection konekcija =
                new SqlConnection(_stringKonekcije);

            using SqlCommand komanda =
                new SqlCommand(upit, konekcija);

            komanda.Parameters.Add(
                "@KlijentId",
                SqlDbType.Int
            ).Value = klijentId;

            konekcija.Open();

            return Convert.ToInt32(
                komanda.ExecuteScalar());
        }

        public bool DaLiJeVoziloDostupno(
            int voziloId,
            DateTime datumPreuzimanja,
            DateTime datumVracanja)
        {
            const string upit = @"
                SELECT COUNT(*)
                FROM StavkeUgovora su
                INNER JOIN Ugovori u
                    ON su.UgovorId = u.Id
                WHERE su.VoziloId = @VoziloId
                  AND u.StatusUgovora <> N'Otkazan'
                  AND @DatumPreuzimanja < u.DatumVracanja
                  AND @DatumVracanja > u.DatumPreuzimanja";

            using SqlConnection konekcija =
                new SqlConnection(_stringKonekcije);

            using SqlCommand komanda =
                new SqlCommand(upit, konekcija);

            komanda.Parameters.Add(
                "@VoziloId",
                SqlDbType.Int
            ).Value = voziloId;

            komanda.Parameters.Add(
                "@DatumPreuzimanja",
                SqlDbType.DateTime2
            ).Value = datumPreuzimanja;

            komanda.Parameters.Add(
                "@DatumVracanja",
                SqlDbType.DateTime2
            ).Value = datumVracanja;

            konekcija.Open();

            int brojPreklapanja =
                Convert.ToInt32(
                    komanda.ExecuteScalar());

            return brojPreklapanja == 0;
        }

        private static UgovorModel MapirajUgovor(
            SqlDataReader citac)
        {
            return new UgovorModel
            {
                Id = citac.GetInt32(
                    citac.GetOrdinal("Id")),

                BrojUgovora = citac.GetString(
                    citac.GetOrdinal("BrojUgovora")),

                DatumIzdavanja = citac.GetDateTime(
                    citac.GetOrdinal("DatumIzdavanja")),

                DatumPreuzimanja = citac.GetDateTime(
                    citac.GetOrdinal("DatumPreuzimanja")),

                DatumVracanja = citac.GetDateTime(
                    citac.GetOrdinal("DatumVracanja")),

                MestoPreuzimanja = citac.GetString(
                    citac.GetOrdinal("MestoPreuzimanja")),

                MestoVracanja = citac.GetString(
                    citac.GetOrdinal("MestoVracanja")),

                NacinPlacanja = citac.GetString(
                    citac.GetOrdinal("NacinPlacanja")),

                Depozit = citac.GetDecimal(
                    citac.GetOrdinal("Depozit")),

                StatusUgovora = citac.GetString(
                    citac.GetOrdinal("StatusUgovora")),

                Napomena =
                    citac.IsDBNull(
                        citac.GetOrdinal("Napomena"))
                        ? null
                        : citac.GetString(
                            citac.GetOrdinal("Napomena")),

                PopustProcenat = citac.GetDecimal(
                    citac.GetOrdinal("PopustProcenat")),

                UkupnoZaPlacanje = citac.GetDecimal(
                    citac.GetOrdinal("UkupnoZaPlacanje")),

                KlijentId = citac.GetInt32(
                    citac.GetOrdinal("KlijentId")),

                KorisnikId = citac.GetInt32(
                    citac.GetOrdinal("KorisnikId"))
            };
        }
    }
}