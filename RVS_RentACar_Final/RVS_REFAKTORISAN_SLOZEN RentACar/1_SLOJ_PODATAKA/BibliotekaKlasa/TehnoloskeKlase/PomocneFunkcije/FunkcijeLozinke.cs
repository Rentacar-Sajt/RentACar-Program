using System;
using System.Security.Cryptography;
using System.Text;

namespace BibliotekaKlasa.TehnoloskeKlase.PomocneFunkcije
{
    // Uloga klase: FunkcijeLozinke grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public static class FunkcijeLozinke
    {
        // Generiše nasumičan salt koji se koristi pri bezbednom heširanju lozinke.
        public static string GenerisiSalt(int duzina = 16)
        {
            byte[] saltBajtovi = new byte[duzina];
            using (var randomBroj = RandomNumberGenerator.Create())
            {
                randomBroj.GetBytes(saltBajtovi);
            }
            return Convert.ToBase64String(saltBajtovi);
        }

        // Izračunava hash lozinke koristeći prosleđenu lozinku i salt.
        public static string IzracunajHash(string lozinka, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedLozinka = lozinka + salt;
                byte[] hashBajtovi = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedLozinka));
                return Convert.ToBase64String(hashBajtovi);
            }
        }

        // Proverava da li uneta lozinka odgovara sačuvanom hash-u korisnika.
        public static bool ProveriLozinku(string lozinka, string salt, string hash)
        {
            string izracunatiHash = IzracunajHash(lozinka, salt);
            return izracunatiHash == hash;
        }
    }
}
