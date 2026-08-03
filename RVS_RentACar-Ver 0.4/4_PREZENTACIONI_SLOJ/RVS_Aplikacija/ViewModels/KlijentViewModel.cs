using System.ComponentModel.DataAnnotations;

namespace RVS_Aplikacija.ViewModels
{
    public class KlijentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ime je obavezno.")]
        [StringLength(50)]
        public string Ime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prezime je obavezno.")]
        [StringLength(50)]
        public string Prezime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "JMBG je obavezan.")]
        [StringLength(13)]
        public string JMBG { get; set; } = string.Empty;

        public string BrojPasosa { get; set; } = string.Empty;

        [Required(ErrorMessage = "Broj vozačke dozvole je obavezan.")]
        public string BrojVozackeDozvole { get; set; } = string.Empty;

        [Required(ErrorMessage = "Datum izdavanja dozvole je obavezan.")]
        [DataType(DataType.Date)]
        public DateTime DatumIzdavanjaDozvole { get; set; }

        [Required(ErrorMessage = "Datum isteka dozvole je obavezan.")]
        [DataType(DataType.Date)]
        public DateTime DatumIstekaVozackeDozvole { get; set; }

        [Required(ErrorMessage = "Telefon je obavezan.")]
        public string Telefon { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adresa je obavezna.")]
        public string Adresa { get; set; } = string.Empty;
    }
}