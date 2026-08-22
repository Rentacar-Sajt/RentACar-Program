namespace PoslovnaLogika
{
	// Uloga klase: PodaciZaObracunUgovora grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
	public class PodaciZaObracunUgovora
	{
		public int KlijentId { get; set; }

		public int VoziloId { get; set; }

		public DateTime DatumOd { get; set; }

		public DateTime DatumDo { get; set; }

		public DateTime? StvarniDatumVracanja { get; set; }

		public decimal CenaPoDanu { get; set; }

		public bool FullOsiguranje { get; set; }
	}
}
