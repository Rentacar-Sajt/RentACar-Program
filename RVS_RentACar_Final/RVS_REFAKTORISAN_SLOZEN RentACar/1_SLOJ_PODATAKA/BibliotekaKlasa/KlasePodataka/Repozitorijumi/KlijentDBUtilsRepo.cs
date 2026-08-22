using BibliotekaKlasa.KlasePodataka.Modeli;
using BibliotekaKlasa.TehnoloskeKlase;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BibliotekaKlasa.KlasePodataka.Repozitorijumi
{
    /// <summary>
    /// Repozitorijum koji pokazuje DBUtils pristup tabeli Klijenti.
    /// Ne otvara konekciju i ne izvršava komande potpuno samostalno, već nasleđuje
    /// TabelaKlasa i koristi njene zajedničke metode za izvršavanje SQL upita.
    /// U ovoj klasi se zato definiše šta želimo da uradimo nad tabelom Klijenti,
    /// dok bazna klasa rešava tehnički deo komunikacije sa bazom.
    /// </summary>
    public class KlijentDBUtilsRepo : TabelaKlasa
    {
        public KlijentDBUtilsRepo(string stringKonekcije)
            : base(stringKonekcije, "Klijenti")
        {
        }

        // Formira SELECT upit za sve klijente. Sam upit je ovde, ali njegovo izvršavanje
        // obavlja nasleđena metoda DajPodatke iz TabelaKlasa. Dobijeni DataSet se zatim
        // pretvara u List<KlijentModel> da bi ostatak aplikacije radio sa C# objektima.
        public List<KlijentModel> DajSveKlijente()
        {
            const string upit = @"
                SELECT
                    Id,
                    Ime,
                    Prezime,
                    JMBG,
                    BrojPasosa,
                    BrojVozackeDozvole,
                    DatumIzdavanjaDozvole,
                    DatumIstekaVozackeDozvole,
                    Telefon,
                    Email,
                    Adresa
                FROM Klijenti
                ORDER BY Prezime, Ime";

            return PretvoriUListu(DajPodatke(upit));
        }

        // Traži tačno jednog klijenta prema ID-u. Vrednost ID-a se prosleđuje kroz @Id parametar,
        // a ne lepi se direktno u SQL string. Time je upit bezbedniji i izbegava se SQL injection.
        public KlijentModel? DajKlijentaPoId(int id)
        {
            const string upit = @"
                SELECT
                    Id,
                    Ime,
                    Prezime,
                    JMBG,
                    BrojPasosa,
                    BrojVozackeDozvole,
                    DatumIzdavanjaDozvole,
                    DatumIstekaVozackeDozvole,
                    Telefon,
                    Email,
                    Adresa
                FROM Klijenti
                WHERE Id = @Id";

            DataSet podaci = DajPodatke(
                upit,
                new SqlParameter("@Id", SqlDbType.Int) { Value = id });

            return PretvoriUListu(podaci).FirstOrDefault();
        }

        // Pretražuje klijente po imenu, prezimenu, broju vozačke dozvole ili telefonu.
        // Korisnički tekst se prosleđuje kao @Tekst parametar i koristi sa LIKE operatorom.
        public List<KlijentModel> FiltrirajKlijente(string tekst)
        {
            const string upit = @"
                SELECT
                    Id,
                    Ime,
                    Prezime,
                    JMBG,
                    BrojPasosa,
                    BrojVozackeDozvole,
                    DatumIzdavanjaDozvole,
                    DatumIstekaVozackeDozvole,
                    Telefon,
                    Email,
                    Adresa
                FROM Klijenti
                WHERE
                    Ime LIKE @Tekst OR
                    Prezime LIKE @Tekst OR
                    BrojVozackeDozvole LIKE @Tekst OR
                    Telefon LIKE @Tekst
                ORDER BY Prezime, Ime";

            return PretvoriUListu(
                DajPodatke(
                    upit,
                    new SqlParameter("@Tekst", SqlDbType.NVarChar, 100)
                    {
                        Value = $"%{tekst ?? string.Empty}%"
                    }));
        }

        // Dodaje novog klijenta pomoću parametrizovanog INSERT upita.
        // Posle INSERT-a SCOPE_IDENTITY() vraća ID reda koji je SQL Server upravo kreirao.
        public int DodajKlijenta(KlijentModel klijent)
        {
            const string upit = @"
                INSERT INTO Klijenti
                (
                    Ime,
                    Prezime,
                    JMBG,
                    BrojPasosa,
                    BrojVozackeDozvole,
                    DatumIzdavanjaDozvole,
                    DatumIstekaVozackeDozvole,
                    Telefon,
                    Email,
                    Adresa
                )
                VALUES
                (
                    @Ime,
                    @Prezime,
                    @JMBG,
                    @BrojPasosa,
                    @BrojVozackeDozvole,
                    @DatumIzdavanjaDozvole,
                    @DatumIstekaVozackeDozvole,
                    @Telefon,
                    @Email,
                    @Adresa
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            object? rezultat = IzvrsiSkalar(
                upit,
                NapraviParametreKlijenta(klijent));

            return rezultat == null || rezultat == DBNull.Value
                ? 0
                : Convert.ToInt32(rezultat);
        }

        // Menja podatke postojećeg klijenta. UPDATE se izvršava samo nad redom čiji je Id jednak @Id,
        // a sve nove vrednosti se šalju kroz SqlParameter objekte.
        public bool IzmeniKlijenta(KlijentModel klijent)
        {
            const string upit = @"
                UPDATE Klijenti
                SET
                    Ime = @Ime,
                    Prezime = @Prezime,
                    JMBG = @JMBG,
                    BrojPasosa = @BrojPasosa,
                    BrojVozackeDozvole = @BrojVozackeDozvole,
                    DatumIzdavanjaDozvole = @DatumIzdavanjaDozvole,
                    DatumIstekaVozackeDozvole = @DatumIstekaVozackeDozvole,
                    Telefon = @Telefon,
                    Email = @Email,
                    Adresa = @Adresa
                WHERE Id = @Id";

            List<SqlParameter> parametri =
                NapraviParametreKlijenta(klijent).ToList();

            parametri.Add(
                new SqlParameter("@Id", SqlDbType.Int)
                {
                    Value = klijent.Id
                });

            return IzvrsiAzuriranjeParametrizovano(
                upit,
                parametri.ToArray()) > 0;
        }

        // Briše jednog klijenta prema njegovom ID-u. ExecuteNonQuery vraća broj obrisanih redova,
        // pa rezultat > 0 znači da je brisanje zaista izvršeno.
        public bool ObrisiKlijenta(int id)
        {
            const string upit =
                "DELETE FROM Klijenti WHERE Id = @Id";

            return IzvrsiAzuriranjeParametrizovano(
                upit,
                new SqlParameter("@Id", SqlDbType.Int)
                {
                    Value = id
                }) > 0;
        }

        // Na jednom mestu pravi sve SqlParameter objekte za podatke o klijentu.
        // Ovu pomoćnu metodu koriste i INSERT i UPDATE kako se isti kod ne bi ponavljao.
        private static SqlParameter[] NapraviParametreKlijenta(
            KlijentModel klijent)
        {
            return new[]
            {
                new SqlParameter("@Ime", SqlDbType.NVarChar, 50)
                {
                    Value = klijent.Ime ?? string.Empty
                },
                new SqlParameter("@Prezime", SqlDbType.NVarChar, 50)
                {
                    Value = klijent.Prezime ?? string.Empty
                },
                new SqlParameter("@JMBG", SqlDbType.NVarChar, 13)
                {
                    Value = string.IsNullOrWhiteSpace(klijent.JMBG)
                        ? DBNull.Value
                        : klijent.JMBG
                },
                new SqlParameter("@BrojPasosa", SqlDbType.NVarChar, 30)
                {
                    Value = string.IsNullOrWhiteSpace(klijent.BrojPasosa)
                        ? DBNull.Value
                        : klijent.BrojPasosa
                },
                new SqlParameter("@BrojVozackeDozvole", SqlDbType.NVarChar, 30)
                {
                    Value = klijent.BrojVozackeDozvole ?? string.Empty
                },
                new SqlParameter("@DatumIzdavanjaDozvole", SqlDbType.Date)
                {
                    Value = klijent.DatumIzdavanjaDozvole
                },
                new SqlParameter("@DatumIstekaVozackeDozvole", SqlDbType.Date)
                {
                    Value = klijent.DatumIstekaVozackeDozvole.HasValue
                        ? klijent.DatumIstekaVozackeDozvole.Value
                        : DBNull.Value
                },
                new SqlParameter("@Telefon", SqlDbType.NVarChar, 30)
                {
                    Value = klijent.Telefon ?? string.Empty
                },
                new SqlParameter("@Email", SqlDbType.NVarChar, 128)
                {
                    Value = string.IsNullOrWhiteSpace(klijent.Email)
                        ? DBNull.Value
                        : klijent.Email
                },
                new SqlParameter("@Adresa", SqlDbType.NVarChar, 150)
                {
                    Value = klijent.Adresa ?? string.Empty
                }
            };
        }

        // DataSet koji vraća DBUtils sloj sadrži redove iz baze. Ova metoda svaki DataRow
        // pretvara u KlijentModel objekat i na kraju vraća listu koju koriste viši slojevi aplikacije.
        private static List<KlijentModel> PretvoriUListu(DataSet podaci)
        {
            List<KlijentModel> klijenti = new List<KlijentModel>();

            if (podaci.Tables.Count == 0)
            {
                return klijenti;
            }

            foreach (DataRow red in podaci.Tables[0].Rows)
            {
                klijenti.Add(new KlijentModel
                {
                    Id = Convert.ToInt32(red["Id"]),
                    Ime = red["Ime"].ToString() ?? string.Empty,
                    Prezime = red["Prezime"].ToString() ?? string.Empty,
                    JMBG = red["JMBG"] == DBNull.Value
                        ? null
                        : red["JMBG"].ToString(),
                    BrojPasosa = red["BrojPasosa"] == DBNull.Value
                        ? null
                        : red["BrojPasosa"].ToString(),
                    BrojVozackeDozvole =
                        red["BrojVozackeDozvole"].ToString() ?? string.Empty,
                    DatumIzdavanjaDozvole =
                        Convert.ToDateTime(red["DatumIzdavanjaDozvole"]),
                    DatumIstekaVozackeDozvole =
                        red["DatumIstekaVozackeDozvole"] == DBNull.Value
                            ? null
                            : Convert.ToDateTime(
                                red["DatumIstekaVozackeDozvole"]),
                    Telefon = red["Telefon"].ToString() ?? string.Empty,
                    Email = red["Email"] == DBNull.Value
                        ? null
                        : red["Email"].ToString(),
                    Adresa = red["Adresa"].ToString() ?? string.Empty
                });
            }

            return klijenti;
        }
    }
}
