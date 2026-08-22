using BibliotekaKlasa.KlasePodataka.Modeli;
using BibliotekaKlasa.KlasePodataka.Repozitorijumi;
using BibliotekaKlasa.KlasePodatakaEF.KontekstEF;
using BibliotekaKlasa.KlasePodatakaEF.ModeliEF;
using BibliotekaKlasa.KlasePodatakaEF.RepozitorijumiEF;
using Microsoft.AspNetCore.Mvc;

namespace REST_SERVIS_CRUD_Operacija.Controllers
{
    // Ovaj kontroler služi samo za demonstraciju tri različita načina pristupa istoj tabeli Klijenti.
    // Svaki endpoint vraća iste podatke, ali ih dobija drugom tehnologijom: direktnim SqlClient
    // pristupom, preko DBUtils/TabelaKlasa nasleđivanja ili preko Entity Framework Core-a.
    [ApiController]
    [Route("api/[controller]")]
    public class TehnologijePodatakaController : ControllerBase
    {
        private readonly string _stringKonekcije;
        private readonly AppDbContext _dbContext;

        public TehnologijePodatakaController(
            IConfiguration konfiguracija,
            AppDbContext dbContext)
        {
            _stringKonekcije =
                konfiguracija.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Nije pronađen connection string DefaultConnection.");

            _dbContext = dbContext;
        }

        // 1) SQL Client pristup: KlijentRepo direktno koristi Microsoft.Data.SqlClient i stored procedure.
        // Kontroler samo poziva repozitorijum i vraća rezultat kao HTTP 200 odgovor.
        [HttpGet("klijenti/sql-client")]
        public ActionResult<List<KlijentModel>>
            DajKlijenteSqlClient()
        {
            KlijentRepo repo = new KlijentRepo(_stringKonekcije);
            return Ok(repo.DajSveKlijente());
        }

        // 2) DBUtils pristup: KlijentDBUtilsRepo nasleđuje TabelaKlasa. Konkretni SQL upit je u
        // KlijentDBUtilsRepo, dok TabelaKlasa izvršava upit i upravlja zajedničkim ADO.NET kodom.
        [HttpGet("klijenti/dbutils")]
        public ActionResult<List<KlijentModel>>
            DajKlijenteDbUtils()
        {
            KlijentDBUtilsRepo repo =
                new KlijentDBUtilsRepo(_stringKonekcije);

            return Ok(repo.DajSveKlijente());
        }

        // 3) Entity Framework pristup: KlijentEntityRepo koristi AppDbContext, DbSet i LINQ.
        // Entity Framework sam prevodi LINQ u SQL i izvršava ga nad istom tabelom Klijenti.
        [HttpGet("klijenti/entity-framework")]
        public ActionResult<List<KlijentEntityModel>>
            DajKlijenteEntityFramework()
        {
            KlijentEntityRepo repo =
                new KlijentEntityRepo(_dbContext);

            return Ok(repo.DajSve());
        }

        // Ovaj endpoint poziva sva tri pristupa i poredi broj vraćenih klijenata. Ako je istiRezultat true,
        // sva tri načina su pročitala isti broj zapisa iz iste tabele Klijenti.
        [HttpGet("klijenti/poredjenje")]
        public ActionResult DajPoredjenjeTehnologija()
        {
            KlijentRepo sqlRepo = new KlijentRepo(_stringKonekcije);
            KlijentDBUtilsRepo dbUtilsRepo =
                new KlijentDBUtilsRepo(_stringKonekcije);
            KlijentEntityRepo efRepo =
                new KlijentEntityRepo(_dbContext);

            int sqlClientBroj = sqlRepo.DajSveKlijente().Count;
            int dbUtilsBroj = dbUtilsRepo.DajSveKlijente().Count;
            int entityFrameworkBroj = efRepo.DajSve().Count;

            return Ok(new
            {
                tabela = "Klijenti",
                sqlClient = new
                {
                    pristup = "Microsoft.Data.SqlClient + stored procedure",
                    brojZapisa = sqlClientBroj
                },
                dbUtils = new
                {
                    pristup = "KlijentDBUtilsRepo : TabelaKlasa + SQL upit",
                    brojZapisa = dbUtilsBroj
                },
                entityFramework = new
                {
                    pristup = "AppDbContext : DbContext + LINQ",
                    brojZapisa = entityFrameworkBroj
                },
                istiRezultat =
                    sqlClientBroj == dbUtilsBroj &&
                    dbUtilsBroj == entityFrameworkBroj
            });
        }
    }
}
