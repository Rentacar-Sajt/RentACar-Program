using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliotekaKlasa.KlasePodatakaEF.ModeliEF
{
    // Ova klasa predstavlja jedan red iz SQL tabele Klijenti u Entity Framework-u.
    // Svako C# svojstvo odgovara jednoj koloni tabele. Atributi [Key], [Required],
    // [StringLength] i [Column] govore Entity Framework-u kako da svojstva mapira na bazu.
// [Table("Klijenti")] eksplicitno povezuje ovu C# klasu sa SQL tabelom Klijenti.
    [Table("Klijenti")]
    public class KlijentEntityModel
    {
// [Key] označava Id kao primarni ključ entiteta.
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Ime { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Prezime { get; set; } = string.Empty;

        [StringLength(13)]
        public string? JMBG { get; set; }

        [StringLength(30)]
        public string? BrojPasosa { get; set; }

        [Required]
        [StringLength(30)]
        public string BrojVozackeDozvole { get; set; } = string.Empty;

// Kolona se čuva kao SQL tip date, bez vremenskog dela.
        [Required]
        [Column(TypeName = "date")]
        public DateTime DatumIzdavanjaDozvole { get; set; }

        [Required]
        [StringLength(30)]
        public string Telefon { get; set; } = string.Empty;

        [StringLength(128)]
        public string? Email { get; set; }

        [Required]
        [StringLength(150)]
        public string Adresa { get; set; } = string.Empty;

        [Column(TypeName = "date")]
        public DateTime? DatumIstekaVozackeDozvole { get; set; }
    }
}
