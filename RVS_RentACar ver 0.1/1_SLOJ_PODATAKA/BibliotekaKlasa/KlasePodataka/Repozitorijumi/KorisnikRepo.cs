using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using BibliotekaKlasa.KlasePodataka.Modeli;
using BibliotekaKlasa.TehnoloskeKlase;


namespace BibliotekaKlasa.KlasePodataka.Repozitorijumi
{
    public class KorisnikRepo
    {
        public KonekcijaKlasa _konekcijaObjekat { get; set; }

        public KorisnikRepo(KonekcijaKlasa konekcijaObjekat)
        {
            _konekcijaObjekat = konekcijaObjekat;
        }
        public void Dodaj(KorisnikModel korisnikModelObjekat)
        {
            _konekcijaObjekat.OtvoriKonekciju();
            //var konekcija = StatickaBaznaKonekcija.Konekcija;
            string upit = @"INSERT INTO Korisnici 
                            (Ime, Prezime, Email, LozinkaHash, LozinkaSalt, Uloga) 
                            VALUES (@Ime, @Prezime, @Email, @Hash, @Salt, @Uloga)";
            using var komanda = new SqlCommand(upit, _konekcijaObjekat.DajKonekciju());
            komanda.Parameters.AddWithValue("@Ime", korisnikModelObjekat.Ime);
            komanda.Parameters.AddWithValue("@Prezime", korisnikModelObjekat.Prezime);
            komanda.Parameters.AddWithValue("@Email", korisnikModelObjekat.Email);
            komanda.Parameters.AddWithValue("@Hash", korisnikModelObjekat.LozinkaHash);
            komanda.Parameters.AddWithValue("@Salt", korisnikModelObjekat.LozinkaSalt);
            komanda.Parameters.AddWithValue("@Uloga", korisnikModelObjekat.Uloga);
            komanda.ExecuteNonQuery();
            _konekcijaObjekat.ZatvoriKonekciju();
        }

        public void Izmeni(KorisnikModel korisnikModelObjekat)
        {
            _konekcijaObjekat.OtvoriKonekciju();
            string upit = @"UPDATE Korisnici SET 
                            Ime=@Ime, Prezime=@Prezime, Email=@Email, 
                            LozinkaHash=@Hash, LozinkaSalt=@Salt, Uloga=@Uloga 
                            WHERE Id=@Id";
            using var komanda = new SqlCommand(upit, _konekcijaObjekat.DajKonekciju());
            komanda.Parameters.AddWithValue("@Ime", korisnikModelObjekat.Ime);
            komanda.Parameters.AddWithValue("@Prezime", korisnikModelObjekat.Prezime);
            komanda.Parameters.AddWithValue("@Email", korisnikModelObjekat.Email);
            komanda.Parameters.AddWithValue("@Hash", korisnikModelObjekat.LozinkaHash);
            komanda.Parameters.AddWithValue("@Salt", korisnikModelObjekat.LozinkaSalt);
            komanda.Parameters.AddWithValue("@Uloga", korisnikModelObjekat.Uloga);
            komanda.Parameters.AddWithValue("@Id", korisnikModelObjekat.Id);
            komanda.ExecuteNonQuery();
            _konekcijaObjekat.ZatvoriKonekciju();
        }

        public void Obrisi(int id)
        {
            _konekcijaObjekat.OtvoriKonekciju();
            string upit = "DELETE FROM Korisnici WHERE Id=@Id";
            using var komanda = new SqlCommand(upit, _konekcijaObjekat.DajKonekciju());
            komanda.Parameters.AddWithValue("@Id", id);
            komanda.ExecuteNonQuery();
            _konekcijaObjekat.ZatvoriKonekciju();
        }

        public List<KorisnikModel> DajSve()
        {
            var lista = new List<KorisnikModel>();
            _konekcijaObjekat.OtvoriKonekciju();
            string upit = @"SELECT Id, Ime, Prezime, Email, LozinkaHash, LozinkaSalt, Uloga 
                            FROM Korisnici";
            using var komanda = new SqlCommand(upit, _konekcijaObjekat.DajKonekciju());
            using var citacKolekcije = komanda.ExecuteReader();
            while (citacKolekcije.Read())
            {
                lista.Add(new KorisnikModel
                {
                    Id = citacKolekcije.GetInt32(0),
                    Ime = citacKolekcije.GetString(1),
                    Prezime = citacKolekcije.GetString(2),
                    Email = citacKolekcije.GetString(3),
                    LozinkaHash = citacKolekcije.GetString(4),
                    LozinkaSalt = citacKolekcije.GetString(5),
                    Uloga = citacKolekcije.GetString(6)
                });
            }
            _konekcijaObjekat.ZatvoriKonekciju();
            return lista;
        }

        public List<KorisnikModel> DajSveSaFilterom(string filter)
        {
            var lista = new List<KorisnikModel>();
            _konekcijaObjekat.OtvoriKonekciju();
            string upit = @"SELECT Id, Ime, Prezime, Email, LozinkaHash, LozinkaSalt, Uloga 
                            FROM Korisnici 
                            WHERE Ime LIKE @Filter OR Prezime LIKE @Filter OR Email LIKE @Filter";
            using var komanda = new SqlCommand(upit, _konekcijaObjekat.DajKonekciju());
            komanda.Parameters.AddWithValue("@Filter", $"%{filter}%");
            using var citacKolekcije = komanda.ExecuteReader();
            while (citacKolekcije.Read())
            {
                lista.Add(new KorisnikModel
                {
                    Id = citacKolekcije.GetInt32(0),
                    Ime = citacKolekcije.GetString(1),
                    Prezime = citacKolekcije.GetString(2),
                    Email = citacKolekcije.GetString(3),
                    LozinkaHash = citacKolekcije.GetString(4),
                    LozinkaSalt = citacKolekcije.GetString(5),
                    Uloga = citacKolekcije.GetString(6)
                });
            }
            _konekcijaObjekat.ZatvoriKonekciju();
            return lista;
        }
    }
}
