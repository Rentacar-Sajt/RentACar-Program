using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliotekaKlasa.KlasePodatakaEF.ModeliEF
{
    [Table("Klijenti")]
    public class KlijentEntityModel
    {
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