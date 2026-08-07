using BibliotekaKlasa.KlasePodataka.Modeli;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace BibliotekaKlasa.KlasePodataka.Repozitorijumi
{
    public class VoziloSPRepo
    {
        private readonly string _stringKonekcije;

        public VoziloSPRepo(string noviStringKonekcije)
        {
            _stringKonekcije = noviStringKonekcije;
        }

        public List<VoziloModel> DajSvaVozila()
        {
            List<VoziloModel> listaVozila = new List<VoziloModel>();

            using SqlConnection konekcija = new SqlConnection(_stringKonekcije);
            using SqlCommand komanda = new SqlCommand("spDajSvaVozila", konekcija);

            komanda.CommandType = CommandType.StoredProcedure;

            konekcija.Open();

            using SqlDataReader citac = komanda.ExecuteReader();

            while (citac.Read())
            {
                listaVozila.Add(MapirajVozilo(citac));
            }

            return listaVozila;
        }

        public VoziloModel? DajVoziloPoId(int id)
        {
            using SqlConnection konekcija = new SqlConnection(_stringKonekcije);
            using SqlCommand komanda = new SqlCommand("spDajVoziloPoId", konekcija);

            komanda.CommandType = CommandType.StoredProcedure;
            komanda.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            konekcija.Open();

            using SqlDataReader citac = komanda.ExecuteReader();

            if (citac.Read())
            {
                return MapirajVozilo(citac);
            }

            return null;
        }

        public List<VoziloModel> DajVozilaPoRegistraciji(string registracija)
        {
            List<VoziloModel> listaVozila = new List<VoziloModel>();

            using SqlConnection konekcija = new SqlConnection(_stringKonekcije);
            using SqlCommand komanda =
                new SqlCommand("spDajVozilaPoRegistraciji", konekcija);

            komanda.CommandType = CommandType.StoredProcedure;

            komanda.Parameters.Add(
                "@Registracija",
                SqlDbType.NVarChar,
                20
            ).Value = registracija ?? string.Empty;

            konekcija.Open();

            using SqlDataReader citac = komanda.ExecuteReader();

            while (citac.Read())
            {
                listaVozila.Add(MapirajVozilo(citac));
            }

            return listaVozila;
        }

        public int DodajVozilo(VoziloModel vozilo)
        {
            using SqlConnection konekcija = new SqlConnection(_stringKonekcije);
            using SqlCommand komanda = new SqlCommand("spDodajVozilo", konekcija);

            komanda.CommandType = CommandType.StoredProcedure;

            DodajParametreVozila(komanda, vozilo);

            konekcija.Open();

            object? rezultat = komanda.ExecuteScalar();

            if (rezultat == null || rezultat == DBNull.Value)
            {
                return 0;
            }

            return Convert.ToInt32(rezultat);
        }

        public bool IzmeniVozilo(VoziloModel vozilo)
        {
            using SqlConnection konekcija =
                new SqlConnection(_stringKonekcije);

            using SqlCommand komanda =
                new SqlCommand("spIzmeniVozilo", konekcija);

            komanda.CommandType = CommandType.StoredProcedure;

            komanda.Parameters.Add(
                "@Id",
                SqlDbType.Int
            ).Value = vozilo.Id;

            DodajParametreVozila(komanda, vozilo);

            konekcija.Open();

            komanda.ExecuteNonQuery();

            return true;
        }

        public bool ObrisiVozilo(int id)
        {
            using SqlConnection konekcija =
                new SqlConnection(_stringKonekcije);

            using SqlCommand komanda =
                new SqlCommand("spObrisiVozilo", konekcija);

            komanda.CommandType = CommandType.StoredProcedure;

            komanda.Parameters.Add(
                "@Id",
                SqlDbType.Int
            ).Value = id;

            konekcija.Open();

            komanda.ExecuteNonQuery();

            return true;
        }

        private static VoziloModel MapirajVozilo(SqlDataReader citac)
        {
            return new VoziloModel
            {
                Id = citac.GetInt32(citac.GetOrdinal("Id")),

                Marka = citac.IsDBNull(citac.GetOrdinal("Marka"))
                    ? null
                    : citac.GetString(citac.GetOrdinal("Marka")),

                Model = citac.IsDBNull(citac.GetOrdinal("Model"))
                    ? null
                    : citac.GetString(citac.GetOrdinal("Model")),

                Registracija = citac.IsDBNull(citac.GetOrdinal("Registracija"))
                    ? null
                    : citac.GetString(citac.GetOrdinal("Registracija")),

                CenaPoDanu =
                    citac.GetDecimal(citac.GetOrdinal("CenaPoDanu")),

                StatusVozila =
                    citac.IsDBNull(citac.GetOrdinal("StatusVozila"))
                        ? null
                        : citac.GetString(citac.GetOrdinal("StatusVozila"))
            };
        }

        private static void DodajParametreVozila(
            SqlCommand komanda,
            VoziloModel vozilo)
        {
            komanda.Parameters.Add(
                "@Marka",
                SqlDbType.NVarChar,
                50
            ).Value = vozilo.Marka ?? string.Empty;

            komanda.Parameters.Add(
                "@Model",
                SqlDbType.NVarChar,
                50
            ).Value = vozilo.Model ?? string.Empty;

            komanda.Parameters.Add(
                "@Registracija",
                SqlDbType.NVarChar,
                20
            ).Value = vozilo.Registracija ?? string.Empty;

            SqlParameter cenaParametar =
                komanda.Parameters.Add(
                    "@CenaPoDanu",
                    SqlDbType.Decimal
                );

            cenaParametar.Precision = 10;
            cenaParametar.Scale = 2;
            cenaParametar.Value = vozilo.CenaPoDanu;

            komanda.Parameters.Add(
                "@StatusVozila",
                SqlDbType.NVarChar,
                30
            ).Value = vozilo.StatusVozila ?? string.Empty;
        }
    }
}