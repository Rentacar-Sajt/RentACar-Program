using BibliotekaKlasa.KlasePodataka.Modeli;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BibliotekaKlasa.KlasePodataka.Repozitorijumi
{
    public class KorisnikRepo
    {
        private readonly string _stringKonekcije;

        public KorisnikRepo(string stringKonekcije)
        {
            _stringKonekcije = stringKonekcije;
        }

        public List<KorisnikModel> DajSveKorisnike()
        {
            const string upit = @"
                SELECT
                    Id,
                    Ime,
                    Prezime,
                    Email,
                    LozinkaHash,
                    LozinkaSalt,
                    Uloga
                FROM Korisnici
                ORDER BY Prezime, Ime;";

            List<KorisnikModel> korisnici =
                new List<KorisnikModel>();

            using SqlConnection konekcija =
                new SqlConnection(_stringKonekcije);

            using SqlCommand komanda =
                new SqlCommand(upit, konekcija);

            konekcija.Open();

            using SqlDataReader citac =
                komanda.ExecuteReader();

            while (citac.Read())
            {
                korisnici.Add(MapirajKorisnika(citac));
            }

            return korisnici;
        }

        public KorisnikModel? DajKorisnikaPoId(int id)
        {
            const string upit = @"
                SELECT
                    Id,
                    Ime,
                    Prezime,
                    Email,
                    LozinkaHash,
                    LozinkaSalt,
                    Uloga
                FROM Korisnici
                WHERE Id = @Id;";

            using SqlConnection konekcija =
                new SqlConnection(_stringKonekcije);

            using SqlCommand komanda =
                new SqlCommand(upit, konekcija);

            komanda.Parameters.Add(
                "@Id",
                SqlDbType.Int
            ).Value = id;

            konekcija.Open();

            using SqlDataReader citac =
                komanda.ExecuteReader();

            if (citac.Read())
            {
                return MapirajKorisnika(citac);
            }

            return null;
        }

        public KorisnikModel? DajKorisnikaPoEmailu(
    string email)
        {
            const string upit = @"
        SELECT
            Id,
            Ime,
            Prezime,
            Email,
            LozinkaHash,
            LozinkaSalt,
            Uloga
        FROM Korisnici
        WHERE Email = @Email;";

            using SqlConnection konekcija =
                new SqlConnection(_stringKonekcije);

            using SqlCommand komanda =
                new SqlCommand(upit, konekcija);

            komanda.Parameters.Add(
                "@Email",
                SqlDbType.NVarChar,
                128
            ).Value = email;

            konekcija.Open();

            using SqlDataReader citac =
                komanda.ExecuteReader();

            if (citac.Read())
            {
                return MapirajKorisnika(citac);
            }

            return null;
        }

        public List<KorisnikModel> FiltrirajKorisnike(
            string tekst)
        {
            const string upit = @"
                SELECT
                    Id,
                    Ime,
                    Prezime,
                    Email,
                    LozinkaHash,
                    LozinkaSalt,
                    Uloga
                FROM Korisnici
                WHERE Ime LIKE @Tekst
                   OR Prezime LIKE @Tekst
                   OR Email LIKE @Tekst
                   OR Uloga LIKE @Tekst
                ORDER BY Prezime, Ime;";

            List<KorisnikModel> korisnici =
                new List<KorisnikModel>();

            using SqlConnection konekcija =
                new SqlConnection(_stringKonekcije);

            using SqlCommand komanda =
                new SqlCommand(upit, konekcija);

            komanda.Parameters.Add(
                "@Tekst",
                SqlDbType.NVarChar,
                150
            ).Value = $"%{tekst ?? string.Empty}%";

            konekcija.Open();

            using SqlDataReader citac =
                komanda.ExecuteReader();

            while (citac.Read())
            {
                korisnici.Add(MapirajKorisnika(citac));
            }

            return korisnici;
        }

        public int DodajKorisnika(
            KorisnikModel korisnik)
        {
            const string upit = @"
                INSERT INTO Korisnici
                (
                    Ime,
                    Prezime,
                    Email,
                    LozinkaHash,
                    LozinkaSalt,
                    Uloga
                )
                VALUES
                (
                    @Ime,
                    @Prezime,
                    @Email,
                    @LozinkaHash,
                    @LozinkaSalt,
                    @Uloga
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using SqlConnection konekcija =
                new SqlConnection(_stringKonekcije);

            using SqlCommand komanda =
                new SqlCommand(upit, konekcija);

            DodajParametreKorisnika(
                komanda,
                korisnik
            );

            konekcija.Open();

            object? rezultat =
                komanda.ExecuteScalar();

            if (rezultat == null ||
                rezultat == DBNull.Value)
            {
                return 0;
            }

            return Convert.ToInt32(rezultat);
        }

        public bool IzmeniKorisnika(
            KorisnikModel korisnik)
        {
            const string upit = @"
                UPDATE Korisnici
                SET
                    Ime = @Ime,
                    Prezime = @Prezime,
                    Email = @Email,
                    LozinkaHash = @LozinkaHash,
                    LozinkaSalt = @LozinkaSalt,
                    Uloga = @Uloga
                WHERE Id = @Id;";

            using SqlConnection konekcija =
                new SqlConnection(_stringKonekcije);

            using SqlCommand komanda =
                new SqlCommand(upit, konekcija);

            komanda.Parameters.Add(
                "@Id",
                SqlDbType.Int
            ).Value = korisnik.Id;

            DodajParametreKorisnika(
                komanda,
                korisnik
            );

            konekcija.Open();

            return komanda.ExecuteNonQuery() > 0;
        }

        public bool ObrisiKorisnika(int id)
        {
            const string upit = @"
                DELETE FROM Korisnici
                WHERE Id = @Id;";

            using SqlConnection konekcija =
                new SqlConnection(_stringKonekcije);

            using SqlCommand komanda =
                new SqlCommand(upit, konekcija);

            komanda.Parameters.Add(
                "@Id",
                SqlDbType.Int
            ).Value = id;

            konekcija.Open();

            return komanda.ExecuteNonQuery() > 0;
        }

        private static KorisnikModel MapirajKorisnika(
            SqlDataReader citac)
        {
            return new KorisnikModel
            {
                Id = citac.GetInt32(
                    citac.GetOrdinal("Id")),

                Ime = citac.IsDBNull(
                    citac.GetOrdinal("Ime"))
                        ? null
                        : citac.GetString(
                            citac.GetOrdinal("Ime")),

                Prezime = citac.IsDBNull(
                    citac.GetOrdinal("Prezime"))
                        ? null
                        : citac.GetString(
                            citac.GetOrdinal("Prezime")),

                Email = citac.IsDBNull(
                    citac.GetOrdinal("Email"))
                        ? null
                        : citac.GetString(
                            citac.GetOrdinal("Email")),

                LozinkaHash = citac.IsDBNull(
                    citac.GetOrdinal("LozinkaHash"))
                        ? null
                        : citac.GetString(
                            citac.GetOrdinal("LozinkaHash")),

                LozinkaSalt = citac.IsDBNull(
                    citac.GetOrdinal("LozinkaSalt"))
                        ? null
                        : citac.GetString(
                            citac.GetOrdinal("LozinkaSalt")),

                Uloga = citac.IsDBNull(
                    citac.GetOrdinal("Uloga"))
                        ? null
                        : citac.GetString(
                            citac.GetOrdinal("Uloga"))
            };
        }

        private static void DodajParametreKorisnika(
            SqlCommand komanda,
            KorisnikModel korisnik)
        {
            komanda.Parameters.Add(
                "@Ime",
                SqlDbType.NVarChar,
                50
            ).Value = korisnik.Ime ?? string.Empty;

            komanda.Parameters.Add(
                "@Prezime",
                SqlDbType.NVarChar,
                50
            ).Value = korisnik.Prezime ?? string.Empty;

            komanda.Parameters.Add(
                "@Email",
                SqlDbType.NVarChar,
                128
            ).Value =
                string.IsNullOrWhiteSpace(korisnik.Email)
                    ? DBNull.Value
                    : korisnik.Email;

            komanda.Parameters.Add(
                "@LozinkaHash",
                SqlDbType.NVarChar,
                500
            ).Value =
                string.IsNullOrWhiteSpace(
                    korisnik.LozinkaHash)
                        ? DBNull.Value
                        : korisnik.LozinkaHash;

            komanda.Parameters.Add(
                "@LozinkaSalt",
                SqlDbType.NVarChar,
                500
            ).Value =
                string.IsNullOrWhiteSpace(
                    korisnik.LozinkaSalt)
                        ? DBNull.Value
                        : korisnik.LozinkaSalt;

            komanda.Parameters.Add(
                "@Uloga",
                SqlDbType.NVarChar,
                50
            ).Value =
                string.IsNullOrWhiteSpace(korisnik.Uloga)
                    ? DBNull.Value
                    : korisnik.Uloga;
        }
    }
}