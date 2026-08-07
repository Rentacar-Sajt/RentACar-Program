using BibliotekaKlasa.KlasePodatakaEF.ModeliEF;
using Microsoft.EntityFrameworkCore;

namespace BibliotekaKlasa.KlasePodatakaEF.KontekstEF
{
    public class AppDbContext : DbContext
    {
        public DbSet<KlijentEntityModel> Klijenti { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> opcije)
            : base(opcije)
        {
        }

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