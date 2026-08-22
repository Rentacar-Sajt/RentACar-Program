using BibliotekaKlasa.KlasePodataka.Modeli;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace BibliotekaKlasa.KlasePodataka.Repozitorijumi
{
    // Uloga klase: VoziloSPRepo grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class VoziloSPRepo
    {
        private readonly string _stringKonekcije;

        public VoziloSPRepo(string noviStringKonekcije)
        {
            _stringKonekcije = noviStringKonekcije;
        }

        // Dohvata sva vozila iz baze i vraća ih kao listu.
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

        // Dohvata vozilo po prosleđenom ID-u; ako vozilo sa tim ID-em ne postoji vraća poruku da vozilo nije pronađeno.
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

        // Pretražuje vozila prema registraciji i vraća listu vozila koja odgovaraju unetom tekstu.
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

        // Dodaje novo vozilo u bazu na osnovu prosleđenih podataka i vraća rezultat operacije.
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

        // Menja podatke postojećeg vozila sa prosleđenim ID-em i vraća rezultat izmene.
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

        // Briše vozilo sa prosleđenim ID-em iz baze i vraća rezultat operacije.
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

        // Mapira jedan red dobijen iz baze u VoziloModel objekat.
        private static VoziloModel MapirajVozilo(SqlDataReader citac)
        {
            return new VoziloModel
            {
                Id = citac.GetInt32(citac.GetOrdinal("Id")),
                Marka = ProcitajString(citac, "Marka"),
                Model = ProcitajString(citac, "Model"),
                Registracija = ProcitajString(citac, "Registracija"),
                CenaPoDanu = citac.GetDecimal(citac.GetOrdinal("CenaPoDanu")),
                StatusVozila = ProcitajString(citac, "StatusVozila"),
                Godiste = ProcitajNullableInt(citac, "Godiste"),
                Gorivo = ProcitajString(citac, "Gorivo"),
                Menjac = ProcitajString(citac, "Menjac"),
                Kilometraza = ProcitajNullableInt(citac, "Kilometraza"),
                Boja = ProcitajString(citac, "Boja"),
                BrojSedista = ProcitajNullableInt(citac, "BrojSedista"),
                ZapreminaMotora = ProcitajNullableDecimal(citac, "ZapreminaMotora"),
                SnagaMotora = ProcitajNullableInt(citac, "SnagaMotora"),
                SlikaPutanja = ProcitajString(citac, "SlikaPutanja")
            };
        }

        // Bezbedno čita tekstualnu kolonu iz SqlDataReader-a i vraća null ako je vrednost u bazi NULL.
        private static string? ProcitajString(SqlDataReader citac, string kolona)
        {
            int ordinal = citac.GetOrdinal(kolona);
            return citac.IsDBNull(ordinal) ? null : citac.GetString(ordinal);
        }

        // Bezbedno čita celobrojnu kolonu iz SqlDataReader-a i vraća null ako je vrednost u bazi NULL.
        private static int? ProcitajNullableInt(SqlDataReader citac, string kolona)
        {
            int ordinal = citac.GetOrdinal(kolona);
            return citac.IsDBNull(ordinal) ? null : citac.GetInt32(ordinal);
        }

        // Bezbedno čita decimalnu kolonu iz SqlDataReader-a i vraća null ako je vrednost u bazi NULL.
        private static decimal? ProcitajNullableDecimal(SqlDataReader citac, string kolona)
        {
            int ordinal = citac.GetOrdinal(kolona);
            return citac.IsDBNull(ordinal) ? null : citac.GetDecimal(ordinal);
        }

        // Dodaje vrednosti vozila kao SQL parametre pre poziva stored procedure.
        private static void DodajParametreVozila(SqlCommand komanda, VoziloModel vozilo)
        {
            komanda.Parameters.Add("@Marka", SqlDbType.NVarChar, 50).Value = vozilo.Marka ?? string.Empty;
            komanda.Parameters.Add("@Model", SqlDbType.NVarChar, 50).Value = vozilo.Model ?? string.Empty;
            komanda.Parameters.Add("@Registracija", SqlDbType.NVarChar, 20).Value = vozilo.Registracija ?? string.Empty;

            SqlParameter cena = komanda.Parameters.Add("@CenaPoDanu", SqlDbType.Decimal);
            cena.Precision = 10;
            cena.Scale = 2;
            cena.Value = vozilo.CenaPoDanu;

            komanda.Parameters.Add("@StatusVozila", SqlDbType.NVarChar, 30).Value = vozilo.StatusVozila ?? string.Empty;
            komanda.Parameters.Add("@Godiste", SqlDbType.Int).Value = (object?)vozilo.Godiste ?? DBNull.Value;
            komanda.Parameters.Add("@Gorivo", SqlDbType.NVarChar, 30).Value = (object?)vozilo.Gorivo ?? DBNull.Value;
            komanda.Parameters.Add("@Menjac", SqlDbType.NVarChar, 30).Value = (object?)vozilo.Menjac ?? DBNull.Value;
            komanda.Parameters.Add("@Kilometraza", SqlDbType.Int).Value = (object?)vozilo.Kilometraza ?? DBNull.Value;
            komanda.Parameters.Add("@Boja", SqlDbType.NVarChar, 30).Value = (object?)vozilo.Boja ?? DBNull.Value;
            komanda.Parameters.Add("@BrojSedista", SqlDbType.Int).Value = (object?)vozilo.BrojSedista ?? DBNull.Value;

            SqlParameter zapremina = komanda.Parameters.Add("@ZapreminaMotora", SqlDbType.Decimal);
            zapremina.Precision = 6;
            zapremina.Scale = 2;
            zapremina.Value = (object?)vozilo.ZapreminaMotora ?? DBNull.Value;

            komanda.Parameters.Add("@SnagaMotora", SqlDbType.Int).Value = (object?)vozilo.SnagaMotora ?? DBNull.Value;
            komanda.Parameters.Add("@SlikaPutanja", SqlDbType.NVarChar, 300).Value = (object?)vozilo.SlikaPutanja ?? DBNull.Value;
        }
    }
}
