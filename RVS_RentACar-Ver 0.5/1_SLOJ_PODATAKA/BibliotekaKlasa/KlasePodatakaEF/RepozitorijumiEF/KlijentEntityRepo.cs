using BibliotekaKlasa.KlasePodatakaEF.KontekstEF;
using BibliotekaKlasa.KlasePodatakaEF.ModeliEF;
using Microsoft.EntityFrameworkCore;

namespace BibliotekaKlasa.KlasePodatakaEF.RepozitorijumiEF
{
    public class KlijentEntityRepo
    {
        private readonly AppDbContext _kontekst;

        public KlijentEntityRepo(AppDbContext kontekst)
        {
            _kontekst = kontekst;
        }

        public List<KlijentEntityModel> DajSve()
        {
            return _kontekst.Klijenti
                .AsNoTracking()
                .OrderBy(k => k.Prezime)
                .ThenBy(k => k.Ime)
                .ToList();
        }

        public KlijentEntityModel? DajPoId(int id)
        {
            return _kontekst.Klijenti
                .AsNoTracking()
                .FirstOrDefault(k => k.Id == id);
        }

        public List<KlijentEntityModel> Filtriraj(string tekst)
        {
            tekst ??= string.Empty;

            return _kontekst.Klijenti
                .AsNoTracking()
                .Where(k =>
                    k.Ime.Contains(tekst) ||
                    k.Prezime.Contains(tekst) ||
                    k.BrojVozackeDozvole.Contains(tekst) ||
                    k.Telefon.Contains(tekst))
                .OrderBy(k => k.Prezime)
                .ThenBy(k => k.Ime)
                .ToList();
        }

        public int Dodaj(KlijentEntityModel klijent)
        {
            ArgumentNullException.ThrowIfNull(klijent);

            _kontekst.Klijenti.Add(klijent);
            _kontekst.SaveChanges();

            return klijent.Id;
        }

        public bool Izmeni(KlijentEntityModel klijent)
        {
            ArgumentNullException.ThrowIfNull(klijent);

            KlijentEntityModel? postojeciKlijent =
                _kontekst.Klijenti.Find(klijent.Id);

            if (postojeciKlijent == null)
            {
                return false;
            }

            postojeciKlijent.Ime = klijent.Ime;
            postojeciKlijent.Prezime = klijent.Prezime;
            postojeciKlijent.JMBG = klijent.JMBG;
            postojeciKlijent.BrojPasosa = klijent.BrojPasosa;
            postojeciKlijent.BrojVozackeDozvole =
                klijent.BrojVozackeDozvole;
            postojeciKlijent.DatumIzdavanjaDozvole =
                klijent.DatumIzdavanjaDozvole;
            postojeciKlijent.Telefon = klijent.Telefon;
            postojeciKlijent.Email = klijent.Email;
            postojeciKlijent.Adresa = klijent.Adresa;

            return _kontekst.SaveChanges() > 0;
        }

        public bool Obrisi(int id)
        {
            KlijentEntityModel? klijent =
                _kontekst.Klijenti.Find(id);

            if (klijent == null)
            {
                return false;
            }

            _kontekst.Klijenti.Remove(klijent);

            return _kontekst.SaveChanges() > 0;
        }

        public bool PostojiBrojVozackeDozvole(
            string brojVozackeDozvole,
            int? izuzmiKlijentaId = null)
        {
            return _kontekst.Klijenti.Any(k =>
                k.BrojVozackeDozvole == brojVozackeDozvole &&
                (!izuzmiKlijentaId.HasValue ||
                 k.Id != izuzmiKlijentaId.Value));
        }

        public bool PostojiJmbg(
            string jmbg,
            int? izuzmiKlijentaId = null)
        {
            if (string.IsNullOrWhiteSpace(jmbg))
            {
                return false;
            }

            return _kontekst.Klijenti.Any(k =>
                k.JMBG == jmbg &&
                (!izuzmiKlijentaId.HasValue ||
                 k.Id != izuzmiKlijentaId.Value));
        }
    }
}