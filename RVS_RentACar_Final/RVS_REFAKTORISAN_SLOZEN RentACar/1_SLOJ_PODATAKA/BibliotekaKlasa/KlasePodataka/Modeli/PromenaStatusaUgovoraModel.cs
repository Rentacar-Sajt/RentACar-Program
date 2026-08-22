namespace REST_SERVIS_CRUD_Operacija.Modeli
{
    // Uloga klase: PromenaStatusaUgovoraModel grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class PromenaStatusaUgovoraModel
    {
        public string NoviStatus { get; set; } = string.Empty;

        public DateTime? StvarniDatumVracanja { get; set; }

        public int BrojDanaKasnjenja { get; set; }

        public decimal KaznaZaKasnjenje { get; set; }
    }
}
