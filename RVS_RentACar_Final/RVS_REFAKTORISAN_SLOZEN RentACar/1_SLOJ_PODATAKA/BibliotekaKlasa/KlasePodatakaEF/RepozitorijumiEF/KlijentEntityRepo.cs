using BibliotekaKlasa.KlasePodatakaEF.KontekstEF;
using BibliotekaKlasa.KlasePodatakaEF.ModeliEF;
using Microsoft.EntityFrameworkCore;

namespace BibliotekaKlasa.KlasePodatakaEF.RepozitorijumiEF
{
    // Ovaj repozitorijum radi sa tabelom Klijenti preko Entity Framework Core-a.
    // Dobija AppDbContext kroz konstruktor i sve operacije radi preko DbSet<KlijentEntityModel>.
    // Za čitanje koristi LINQ, a za INSERT/UPDATE/DELETE koristi Add, Find, Remove i SaveChanges.
    // Entity Framework zatim sam generiše odgovarajuće SQL naredbe.
    public class KlijentEntityRepo
    {
// Čuva referencu na DbContext preko kog se izvršavaju sve EF operacije nad bazom.
        private readonly AppDbContext _kontekst;

        public KlijentEntityRepo(AppDbContext kontekst)
        {
            _kontekst = kontekst;
        }

        // Učitava sve klijente i sortira ih po prezimenu i imenu. AsNoTracking znači da EF
        // ne prati izmene ovih objekata, što je pogodnije kada podatke samo čitamo i prikazujemo.
        public List<KlijentEntityModel> DajSve()
        {
            return _kontekst.Klijenti
                .AsNoTracking()
                .OrderBy(k => k.Prezime)
                .ThenBy(k => k.Ime)
                .ToList();
        }

        // Traži prvog klijenta čiji Id odgovara prosleđenoj vrednosti. LINQ uslov k => k.Id == id
        // Entity Framework prevodi u odgovarajući SQL WHERE uslov.
        public KlijentEntityModel? DajPoId(int id)
        {
            return _kontekst.Klijenti
                .AsNoTracking()
                .FirstOrDefault(k => k.Id == id);
        }

        // Pretražuje klijente preko LINQ izraza. Contains se u SQL-u prevodi u odgovarajući LIKE uslov,
        // pa nije potrebno ručno pisati SELECT ... WHERE ... LIKE upit.
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

        // Add registruje novi objekat kao novi entitet u kontekstu, a SaveChanges() generiše i izvršava
        // SQL INSERT. Posle čuvanja EF popunjava Id koji je baza dodelila novom klijentu.
        public int Dodaj(KlijentEntityModel klijent)
        {
            ArgumentNullException.ThrowIfNull(klijent);

            _kontekst.Klijenti.Add(klijent);
            _kontekst.SaveChanges();

            return klijent.Id;
        }

        // Find učitava postojećeg klijenta po primarnom ključu. Nakon promene njegovih svojstava,
        // SaveChanges() prepoznaje šta je promenjeno i generiše SQL UPDATE samo za taj entitet.
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
            postojeciKlijent.DatumIstekaVozackeDozvole =
                klijent.DatumIstekaVozackeDozvole;
            postojeciKlijent.Telefon = klijent.Telefon;
            postojeciKlijent.Email = klijent.Email;
            postojeciKlijent.Adresa = klijent.Adresa;

            return _kontekst.SaveChanges() > 0;
        }

        // Find pronalazi klijenta, Remove ga označava za brisanje, a SaveChanges() zatim
        // generiše i izvršava SQL DELETE naredbu.
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

        // Any proverava da li postoji makar jedan klijent sa istim brojem vozačke dozvole.
        // Parametar izuzmiKlijentaId omogućava da kod izmene zanemarimo trenutnog klijenta.
        public bool PostojiBrojVozackeDozvole(
            string brojVozackeDozvole,
            int? izuzmiKlijentaId = null)
        {
            return _kontekst.Klijenti.Any(k =>
                k.BrojVozackeDozvole == brojVozackeDozvole &&
                (!izuzmiKlijentaId.HasValue ||
                 k.Id != izuzmiKlijentaId.Value));
        }

        // Proverava da li je isti JMBG već upisan. Prazan JMBG se ne proverava jer je ovo polje opciono.
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
