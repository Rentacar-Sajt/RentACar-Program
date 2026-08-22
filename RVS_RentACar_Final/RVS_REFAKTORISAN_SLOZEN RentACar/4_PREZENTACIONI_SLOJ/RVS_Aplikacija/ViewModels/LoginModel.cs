using System.ComponentModel.DataAnnotations;

namespace RVS_MVC.ViewModels
{
	// Uloga klase: LoginModel grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
	public class LoginModel
	{
		[Required(ErrorMessage = "Email je obavezan.")]
		[EmailAddress(ErrorMessage = "Unesite ispravnu email adresu.")]
		[Display(Name = "Email")]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "Lozinka je obavezna.")]
		[DataType(DataType.Password)]
		[Display(Name = "Lozinka")]
		public string Lozinka { get; set; } = string.Empty;
	}
}
