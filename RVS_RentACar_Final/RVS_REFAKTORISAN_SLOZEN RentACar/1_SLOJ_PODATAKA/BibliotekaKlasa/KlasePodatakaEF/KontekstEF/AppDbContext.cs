using BibliotekaKlasa.KlasePodatakaEF.ModeliEF;
using Microsoft.EntityFrameworkCore;

namespace BibliotekaKlasa.KlasePodatakaEF.KontekstEF
{
    // AppDbContext je centralna Entity Framework klasa za pristup bazi.
    // Nasleđuje DbContext, pa Entity Framework preko nje prati entitete i pretvara LINQ izraze
    // u SQL naredbe. Zbog toga u EF repozitorijumu ne moramo ručno da pravimo SqlConnection
    // i SqlCommand za svaku operaciju.
    public class AppDbContext : DbContext
    {
        // DbSet predstavlja tabelu Klijenti u bazi. Preko ovog svojstva možemo da čitamo,
        // dodajemo, menjamo i brišemo KlijentEntityModel objekte.
        public DbSet<KlijentEntityModel> Klijenti { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> opcije)
            : base(opcije)
        {
        }

        // OnModelCreating se izvršava kada EF pravi model baze. Ovde dodatno definišemo pravila
        // koja nisu dovoljna samo kroz atribute na klasi: broj vozačke dozvole mora biti jedinstven,
        // a JMBG je jedinstven samo kada nije NULL.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<KlijentEntityModel>()
                .HasIndex(k => k.BrojVozackeDozvole)
                .IsUnique();

            modelBuilder.Entity<KlijentEntityModel>()
                .HasIndex(k => k.JMBG)
                .IsUnique()
                .HasFilter("[JMBG] IS NOT NULL");
        }
    }
}
