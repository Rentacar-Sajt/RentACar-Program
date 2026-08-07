namespace RVS_MVC.ViewModels
{
    public class LoginOdgovor
    {
        public int Id { get; set; }

        public string Ime { get; set; } = string.Empty;

        public string Prezime { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Uloga { get; set; } = string.Empty;
    }
}