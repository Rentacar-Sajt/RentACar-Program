namespace BibliotekaKlasa.Models
{
    // Uloga klase: GreskaViewModel grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class GreskaViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
