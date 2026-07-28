namespace BibliotekaKlasa.Models
{
    public class GreskaViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
