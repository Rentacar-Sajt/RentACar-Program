using BibliotekaKlasa.KlasePodataka.Modeli;
using BibliotekaKlasa.KlasePodataka.Repozitorijumi;

namespace PoslovnaLogika.Klase
{
    // Uloga klase: AutentikacijaLogika grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class AutentikacijaLogika
    {
        private readonly KorisnikRepo _korisnikRepo;

        public AutentikacijaLogika(
            string konekcioniString)
        {
            _korisnikRepo =
                new KorisnikRepo(konekcioniString);
        }

        // Pronalazi korisnika prema e-mail adresi i proverava unetu lozinku pomoću BCrypt-a; vraća korisnika samo ako je prijava ispravna.
        public KorisnikModel? PrijaviKorisnika(
            string email,
            string lozinka)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(lozinka))
            {
                return null;
            }

            KorisnikModel? korisnik =
                _korisnikRepo.DajKorisnikaPoEmailu(
                    email.Trim()
                );

            if (korisnik == null ||
                string.IsNullOrWhiteSpace(
                    korisnik.LozinkaHash))
            {
                return null;
            }

            bool lozinkaIspravna;

            try
            {
                lozinkaIspravna =
                    BCrypt.Net.BCrypt.Verify(
                        lozinka,
                        korisnik.LozinkaHash
                    );
            }
            catch
            {
                return null;
            }

            if (!lozinkaIspravna)
            {
                return null;
            }

            return korisnik;
        }
    }
}
