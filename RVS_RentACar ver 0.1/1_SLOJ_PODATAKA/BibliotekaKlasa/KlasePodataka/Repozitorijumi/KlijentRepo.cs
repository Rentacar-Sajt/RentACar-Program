using BibliotekaKlasa.KlasePodataka.Modeli;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace BibliotekaKlasa.KlasePodataka.Repozitorijumi
{
    public class KlijentRepo
    {
        private readonly string _stringKonekcije;

        public KlijentRepo(string stringKonekcije)
        {
            _stringKonekcije = stringKonekcije;
        }

        public List<KlijentModel> DajSveKlijente()
        {
            List<KlijentModel> listaKlijenata = new List<KlijentModel>();

            using SqlConnection konekcija = new SqlConnection(_stringKonekcije);
            using SqlCommand komanda =
                new SqlCommand("spDajSveKlijente", konekcija);

            komanda.CommandType = CommandType.StoredProcedure;

            konekcija.Open();

            using SqlDataReader citac = komanda.ExecuteReader();

            while (citac.Read())
            {
                listaKlijenata.Add(MapirajKlijenta(citac));
            }

            return listaKlijenata;
        }

        public KlijentModel? DajKlijentaPoId(int id)
        {
            using SqlConnection konekcija = new SqlConnection(_stringKonekcije);
            using SqlCommand komanda =
                new SqlCommand("spDajKlijentaPoId", konekcija);

            komanda.CommandType = CommandType.StoredProcedure;
            komanda.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            konekcija.Open();

            using SqlDataReader citac = komanda.ExecuteReader();

            if (citac.Read())
            {
                return MapirajKlijenta(citac);
            }

            return null;
        }

        public List<KlijentModel> FiltrirajKlijente(string tekst)
        {
            List<KlijentModel> listaKlijenata = new List<KlijentModel>();

            using SqlConnection konekcija = new SqlConnection(_stringKonekcije);
            using SqlCommand komanda =
                new SqlCommand("spFiltrirajKlijente", konekcija);

            komanda.CommandType = CommandType.StoredProcedure;

            komanda.Parameters.Add(
                "@Tekst",
                SqlDbType.NVarChar,
                100
            ).Value = tekst ?? string.Empty;

            konekcija.Open();

            using SqlDataReader citac = komanda.ExecuteReader();

            while (citac.Read())
            {
                listaKlijenata.Add(MapirajKlijenta(citac));
            }

            return listaKlijenata;
        }

        public int DodajKlijenta(KlijentModel klijent)
        {
            using SqlConnection konekcija = new SqlConnection(_stringKonekcije);
            using SqlCommand komanda =
                new SqlCommand("spDodajKlijenta", konekcija);

            komanda.CommandType = CommandType.StoredProcedure;

            DodajParametreKlijenta(komanda, klijent);

            konekcija.Open();

            object? rezultat = komanda.ExecuteScalar();

            if (rezultat == null || rezultat == DBNull.Value)
            {
                return 0;
            }

            return Convert.ToInt32(rezultat);
        }

        public bool IzmeniKlijenta(KlijentModel klijent)
        {
            using SqlConnection konekcija = new SqlConnection(_stringKonekcije);
            using SqlCommand komanda =
                new SqlCommand("spIzmeniKlijenta", konekcija);

            komanda.CommandType = CommandType.StoredProcedure;
            komanda.Parameters.Add("@Id", SqlDbType.Int).Value = klijent.Id;

            DodajParametreKlijenta(komanda, klijent);

            konekcija.Open();

            return komanda.ExecuteNonQuery() > 0;
        }

        public bool ObrisiKlijenta(int id)
        {
            using SqlConnection konekcija = new SqlConnection(_stringKonekcije);
            using SqlCommand komanda =
                new SqlCommand("spObrisiKlijenta", konekcija);

            komanda.CommandType = CommandType.StoredProcedure;
            komanda.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            konekcija.Open();

            return komanda.ExecuteNonQuery() > 0;
        }

        private static KlijentModel MapirajKlijenta(SqlDataReader citac)
        {
            return new KlijentModel
            {
                Id = citac.GetInt32(citac.GetOrdinal("Id")),

                Ime = citac.GetString(citac.GetOrdinal("Ime")),

                Prezime = citac.GetString(citac.GetOrdinal("Prezime")),

                JMBG = citac.IsDBNull(citac.GetOrdinal("JMBG"))
                    ? null
                    : citac.GetString(citac.GetOrdinal("JMBG")),

                BrojPasosa =
                    citac.IsDBNull(citac.GetOrdinal("BrojPasosa"))
                        ? null
                        : citac.GetString(citac.GetOrdinal("BrojPasosa")),

                BrojVozackeDozvole =
                    citac.GetString(citac.GetOrdinal("BrojVozackeDozvole")),

                DatumIzdavanjaDozvole =
                    citac.GetDateTime(
                        citac.GetOrdinal("DatumIzdavanjaDozvole")),

                Telefon = citac.GetString(citac.GetOrdinal("Telefon")),

                Email = citac.IsDBNull(citac.GetOrdinal("Email"))
                    ? null
                    : citac.GetString(citac.GetOrdinal("Email")),

                Adresa = citac.GetString(citac.GetOrdinal("Adresa"))
            };
        }

        private static void DodajParametreKlijenta(
            SqlCommand komanda,
            KlijentModel klijent)
        {
            komanda.Parameters.Add(
                "@Ime",
                SqlDbType.NVarChar,
                50
            ).Value = klijent.Ime ?? string.Empty;

            komanda.Parameters.Add(
                "@Prezime",
                SqlDbType.NVarChar,
                50
            ).Value = klijent.Prezime ?? string.Empty;

            komanda.Parameters.Add(
                "@JMBG",
                SqlDbType.NVarChar,
                13
            ).Value = string.IsNullOrWhiteSpace(klijent.JMBG)
                ? DBNull.Value
                : klijent.JMBG;

            komanda.Parameters.Add(
                "@BrojPasosa",
                SqlDbType.NVarChar,
                30
            ).Value = string.IsNullOrWhiteSpace(klijent.BrojPasosa)
                ? DBNull.Value
                : klijent.BrojPasosa;

            komanda.Parameters.Add(
                "@BrojVozackeDozvole",
                SqlDbType.NVarChar,
                30
            ).Value = klijent.BrojVozackeDozvole ?? string.Empty;

            komanda.Parameters.Add(
                "@DatumIzdavanjaDozvole",
                SqlDbType.Date
            ).Value = klijent.DatumIzdavanjaDozvole;

            komanda.Parameters.Add(
                "@Telefon",
                SqlDbType.NVarChar,
                30
            ).Value = klijent.Telefon ?? string.Empty;

            komanda.Parameters.Add(
                "@Email",
                SqlDbType.NVarChar,
                128
            ).Value = string.IsNullOrWhiteSpace(klijent.Email)
                ? DBNull.Value
                : klijent.Email;

            komanda.Parameters.Add(
                "@Adresa",
                SqlDbType.NVarChar,
                150
            ).Value = klijent.Adresa ?? string.Empty;
        }
    }
}