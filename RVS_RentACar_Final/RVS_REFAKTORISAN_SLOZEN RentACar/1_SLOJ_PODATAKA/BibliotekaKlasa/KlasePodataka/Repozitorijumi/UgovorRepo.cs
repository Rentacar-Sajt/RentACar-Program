using BibliotekaKlasa.KlasePodataka.Modeli;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace BibliotekaKlasa.KlasePodataka.Repozitorijumi
{
    // Uloga klase: UgovorRepo grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class UgovorRepo
    {
        private readonly string _stringKonekcije;

        public UgovorRepo(string stringKonekcije)
        {
            _stringKonekcije = stringKonekcije;
        }

        // Dohvata sve ugovore iz baze i vraća ih kao listu ugovora.
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

        // Filtrira ugovore prema prosleđenim kriterijumima i vraća samo ugovore koji ispunjavaju uslove pretrage.
        public List<UgovorModel> FiltrirajUgovore(
    string? brojUgovora,
    string? klijent,
    string? statusUgovora,
    DateTime? datumOd,
    DateTime? datumDo)
        {
            List<UgovorModel> ugovori = new List<UgovorModel>();

            using SqlConnection konekcija =
                new SqlConnection(_stringKonekcije);

            using SqlCommand komanda =
                new SqlCommand("spFiltrirajUgovore", konekcija);

            komanda.CommandType = CommandType.StoredProcedure;

            komanda.Parameters.Add(
                "@BrojUgovora",
                SqlDbType.NVarChar,
                20
            ).Value = string.IsNullOrWhiteSpace(brojUgovora)
                ? DBNull.Value
                : brojUgovora;

            komanda.Parameters.Add(
                "@Klijent",
                SqlDbType.NVarChar,
                100
            ).Value = string.IsNullOrWhiteSpace(klijent)
                ? DBNull.Value
                : klijent;

            komanda.Parameters.Add(
                "@StatusUgovora",
                SqlDbType.NVarChar,
                30
            ).Value = string.IsNullOrWhiteSpace(statusUgovora)
                ? DBNull.Value
                : statusUgovora;

            komanda.Parameters.Add(
                "@DatumOd",
                SqlDbType.Date
            ).Value = datumOd.HasValue
                ? datumOd.Value.Date
                : DBNull.Value;

            komanda.Parameters.Add(
                "@DatumDo",
                SqlDbType.Date
            ).Value = datumDo.HasValue
                ? datumDo.Value.Date
                : DBNull.Value;

            konekcija.Open();

            using SqlDataReader citac = komanda.ExecuteReader();

            while (citac.Read())
            {
                ugovori.Add(MapirajUgovor(citac));
            }

            return ugovori;
        }

        // Dohvata ugovor po prosleđenom ID-u; ako ugovor ne postoji vraća rezultat da nije pronađen.
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

        // Dodaje novi ugovor nakon provere podataka i vraća ID novokreiranog ugovora odnosno rezultat operacije.
        public int DodajUgovor(UgovorModel ugovor)
        {
            using SqlConnection konekcija =
                new SqlConnection(_stringKonekcije);

            using SqlCommand komanda =
                new SqlCommand("spDodajUgovorSaStavkama", konekcija);

            komanda.CommandType = CommandType.StoredProcedure;

            DodajParametreUgovora(komanda, ugovor);

            DataTable tabelaStavki = NapraviTabeluStavki(
                ugovor.StavkeUgovora
            );

            SqlParameter parametarStavke =
                komanda.Parameters.AddWithValue("@Stavke", tabelaStavki);

            parametarStavke.SqlDbType = SqlDbType.Structured;
            parametarStavke.TypeName = "dbo.StavkaUgovoraTip";

            konekcija.Open();

            object? rezultat = komanda.ExecuteScalar();

            return Convert.ToInt32(rezultat);
        }

        // Briše ugovor sa prosleđenim ID-em; ako ugovor ne postoji vraća odgovarajući rezultat.
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

        // Poziva operaciju nad bazom koja menja status ugovora i po potrebi čuva podatke o stvarnom vraćanju i kazni.
        public bool PromeniStatusUgovora(
            int ugovorId,
            string noviStatus,
            DateTime? stvarniDatumVracanja,
            int brojDanaKasnjenja,
            decimal kaznaZaKasnjenje)
        {
            using SqlConnection konekcija =
                new SqlConnection(_stringKonekcije);

            using SqlCommand komanda =
                new SqlCommand("spPromeniStatusUgovora", konekcija);

            komanda.CommandType = CommandType.StoredProcedure;

            komanda.Parameters.Add(
                "@UgovorId",
                SqlDbType.Int
            ).Value = ugovorId;

            komanda.Parameters.Add(
                "@NoviStatus",
                SqlDbType.NVarChar,
                30
            ).Value = noviStatus;

            komanda.Parameters.Add(
                "@StvarniDatumVracanja",
                SqlDbType.DateTime2
            ).Value = stvarniDatumVracanja.HasValue
                ? stvarniDatumVracanja.Value
                : DBNull.Value;

            komanda.Parameters.Add(
                "@BrojDanaKasnjenja",
                SqlDbType.Int
            ).Value = Math.Max(0, brojDanaKasnjenja);

            SqlParameter kaznaParametar =
                komanda.Parameters.Add(
                    "@KaznaZaKasnjenje",
                    SqlDbType.Decimal
                );

            kaznaParametar.Precision = 12;
            kaznaParametar.Scale = 2;
            kaznaParametar.Value =
                Math.Max(0m, kaznaZaKasnjenje);

            konekcija.Open();
            komanda.ExecuteNonQuery();

            return true;
        }

        // Računa i vraća broj završenih ugovora za prosleđenog klijenta.
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

        // Proverava u bazi da li vozilo ima preklapanje sa postojećim ugovorima u zadatom periodu i vraća true samo kada je dostupno.
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

        // Dodaje SqlParameter vrednosti komandi za sva polja ugovora pre poziva stored procedure.
        private static void DodajParametreUgovora(
    SqlCommand komanda,
    UgovorModel ugovor)
        {
            komanda.Parameters.Add(
                "@BrojUgovora",
                SqlDbType.NVarChar,
                20
            ).Value = ugovor.BrojUgovora ?? string.Empty;

            komanda.Parameters.Add(
                "@DatumIzdavanja",
                SqlDbType.Date
            ).Value = ugovor.DatumIzdavanja;

            komanda.Parameters.Add(
                "@DatumPreuzimanja",
                SqlDbType.DateTime2
            ).Value = ugovor.DatumPreuzimanja;

            komanda.Parameters.Add(
                "@DatumVracanja",
                SqlDbType.DateTime2
            ).Value = ugovor.DatumVracanja;

            komanda.Parameters.Add(
                "@MestoPreuzimanja",
                SqlDbType.NVarChar,
                100
            ).Value = ugovor.MestoPreuzimanja ?? string.Empty;

            komanda.Parameters.Add(
                "@MestoVracanja",
                SqlDbType.NVarChar,
                100
            ).Value = ugovor.MestoVracanja ?? string.Empty;

            komanda.Parameters.Add(
                "@NacinPlacanja",
                SqlDbType.NVarChar,
                30
            ).Value = ugovor.NacinPlacanja ?? string.Empty;

            komanda.Parameters.Add(
                "@Depozit",
                SqlDbType.Decimal
            ).Value = ugovor.Depozit;

            komanda.Parameters["@Depozit"].Precision = 10;
            komanda.Parameters["@Depozit"].Scale = 2;

            komanda.Parameters.Add(
                "@StatusUgovora",
                SqlDbType.NVarChar,
                30
            ).Value = ugovor.StatusUgovora ?? "Aktivan";

            komanda.Parameters.Add(
                "@Napomena",
                SqlDbType.NVarChar,
                500
            ).Value = string.IsNullOrWhiteSpace(ugovor.Napomena)
                ? DBNull.Value
                : ugovor.Napomena;

            komanda.Parameters.Add(
                "@PopustProcenat",
                SqlDbType.Decimal
            ).Value = ugovor.PopustProcenat;

            komanda.Parameters["@PopustProcenat"].Precision = 5;
            komanda.Parameters["@PopustProcenat"].Scale = 2;

            komanda.Parameters.Add(
                "@UkupnoZaPlacanje",
                SqlDbType.Decimal
            ).Value = ugovor.UkupnoZaPlacanje;

            komanda.Parameters["@UkupnoZaPlacanje"].Precision = 12;
            komanda.Parameters["@UkupnoZaPlacanje"].Scale = 2;

            komanda.Parameters.Add(
                "@KlijentId",
                SqlDbType.Int
            ).Value = ugovor.KlijentId;

            komanda.Parameters.Add(
                "@KorisnikId",
                SqlDbType.Int
            ).Value = ugovor.KorisnikId;

            komanda.Parameters.Add(
    "@IzabraneDodatneUsluge",
    System.Data.SqlDbType.NVarChar,
    100
).Value =
    string.IsNullOrWhiteSpace(
        ugovor.IzabraneDodatneUsluge
    )
        ? DBNull.Value
        : ugovor.IzabraneDodatneUsluge;
        }

        // Pretvara listu stavki ugovora u DataTable koji se prosleđuje stored proceduri kao tabelarni parametar.
        private static DataTable NapraviTabeluStavki(
    List<StavkaUgovoraModel>? stavke)
        {
            DataTable tabela = new DataTable();

            tabela.Columns.Add("VoziloId", typeof(int));
            tabela.Columns.Add("BrojDana", typeof(int));
            tabela.Columns.Add("CenaPoDanu", typeof(decimal));
            tabela.Columns.Add("PopustProcenat", typeof(decimal));
            tabela.Columns.Add("Ukupno", typeof(decimal));

            if (stavke == null)
                return tabela;

            foreach (StavkaUgovoraModel stavka in stavke)
            {
                tabela.Rows.Add(
                    stavka.VoziloId,
                    stavka.BrojDana,
                    stavka.CenaPoDanu,
                    stavka.PopustProcenat,
                    stavka.Ukupno
                );
            }

            return tabela;
        }

        // Čita vrednosti iz SqlDataReader-a i pretvara jedan red rezultata u UgovorModel objekat.
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

                StvarniDatumVracanja =
                    citac["StvarniDatumVracanja"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(
                            citac["StvarniDatumVracanja"]),

                BrojDanaKasnjenja =
                    citac["BrojDanaKasnjenja"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(
                            citac["BrojDanaKasnjenja"]),

                KaznaZaKasnjenje =
                    citac["KaznaZaKasnjenje"] == DBNull.Value
                        ? 0m
                        : Convert.ToDecimal(
                            citac["KaznaZaKasnjenje"]),

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
                    citac.GetOrdinal("KorisnikId")),


                IzabraneDodatneUsluge =
    citac["IzabraneDodatneUsluge"] == DBNull.Value
        ? null
        : citac["IzabraneDodatneUsluge"].ToString(),
            }; 
        }
    }
}
