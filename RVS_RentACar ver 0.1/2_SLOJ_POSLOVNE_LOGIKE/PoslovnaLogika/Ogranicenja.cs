using Microsoft.Data.SqlClient;
using System.Text.Json;
using BibliotekaKlasa.TehnoloskeKlase;
using BibliotekaKlasa.KlasePodataka.Repozitorijumi;
using BibliotekaKlasa.KlasePodataka.Modeli;
namespace PoslovnaLogika.Klase;

public class Ogranicenja
{
    public int MaksKursevaPoKorisniku { get; set; }


//    public bool DaLiImaMestaZaUpis(int korisnikId)
//    {
//        KonekcijaKlasa konekcijaObjekat = new KonekcijaKlasa();
//        konekcijaObjekat.OtvoriKonekciju();
//        KursRepo kursRepoObjekat = new KursRepo(konekcijaObjekat);
//        kursRepoObjekat.UzmiBrojKursevaDodeljenihKorisnikuIzBazePodataka(korisnikId);

//        konekcijaObjekat.ZatvoriKonekciju();
//        int maxBrojKursevaIzJSONa = UzmiBrojKursevaPoKorisnikuIzJSON();
        
//;

//        return trenutnoKurseva < maxBrojKursevaIzJSONa;
//    }

    //private int UzmiBrojKursevaPoKorisnikuIzBaze(int korisnikId)
    //{
    //    var konekcija = StatickaBaznaKonekcija.Konekcija;

    //    string upit = @"SELECT COUNT(*) FROM Kursevi WHERE AutorId = @KorisnikId";

    //    using var komanda = new SqlCommand(upit, konekcija);
    //    komanda.Parameters.AddWithValue("@KorisnikId", korisnikId);

    //    return (int)komanda.ExecuteScalar();
    //}

    //Folder Ogranicenja, a zatim fajl ogranicenjeUpisa.json, nalazi se unutar web servisa koji proverava poslovnu logiku.
    public int UzmiBrojKursevaPoKorisnikuIzJSON()
    {
        int brojOgranicenja = 0;
        string putanja = Path.Combine("Ogranicenja", "ogranicenjeUpisa.json");


        if (!File.Exists(putanja))
        {
            throw new Exception("JSON fajl sa ograničenjem nije pronađen.");
        }

        string json = File.ReadAllText(putanja);
        var podaci = JsonSerializer.Deserialize<Dictionary<string, int>>(json);
        if (podaci == null || !podaci.ContainsKey("MaksKursevaPoKorisniku"))
        {
            throw new Exception("MaksKursevaPoKorisniku nije definisano u JSON-u.");
        }


        return podaci["MaksKursevaPoKorisniku"];
    }

}
