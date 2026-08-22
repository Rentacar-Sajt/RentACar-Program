using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//
using Microsoft.Data.SqlClient;
using System.Data;


namespace BibliotekaKlasa.TehnoloskeKlase
{
    // TabelaKlasa predstavlja opštu DBUtils klasu za rad sa bazom podataka.
    // U njoj se nalazi zajednički kod za povezivanje sa SQL Server bazom,
    // izvršavanje SELECT/INSERT/UPDATE/DELETE upita i vraćanje rezultata.
    // Konkretne klase, na primer KlijentDBUtilsRepo, nasleđuju ovu klasu
    // i koriste njene metode umesto da ponavljaju isti ADO.NET kod.
    public class TabelaKlasa
    {
        /* CRC: 
          * Responsibility - ODGOVORNOST: Konekcija na celinu baze podataka, SQL server tipa  
          Collaboration - zavisi od standardne klase SQlDataAdapter iz biblioteke System.Data.SqlClient
                          kao i klase Dataset iz standardne biblioteke System.Data*/

        #region Atributi

        private string _nazivTabele;
        private KonekcijaKlasa _konekcijaObjekat;
        private SqlDataAdapter _adapterObjekat;
        private DataSet _dataSetObjekat;

        #endregion

        #region Konstruktor

        public TabelaKlasa(KonekcijaKlasa novaKonekcija, string noviNazivTabele)
        {
            _konekcijaObjekat = novaKonekcija;
            _nazivTabele = noviNazivTabele;
        }

        // Ovaj konstruktor prima connection string i naziv tabele.
        // Na osnovu connection string-a pravi KonekcijaKlasa objekat i otvara konekciju.
        // Izvedena klasa zato samo prosledi naziv svoje tabele, npr. "Klijenti",
        // a TabelaKlasa preuzima zajednički deo rada sa bazom.
        public TabelaKlasa(string stringKonekcije, string noviNazivTabele)
        {
            _konekcijaObjekat = new KonekcijaKlasa(stringKonekcije);
            _nazivTabele = noviNazivTabele;

            if (!_konekcijaObjekat.OtvoriKonekciju())
            {
                throw new InvalidOperationException(
                    "Nije moguće otvoriti konekciju ka bazi podataka.");
            }
        }

        #endregion

        #region Privatne metode

        // Kreira SqlCommand objekte za SELECT, INSERT, UPDATE i DELETE i povezuje ih
        // sa istom otvorenom SQL Server konekcijom. Zatim ih postavlja u SqlDataAdapter.
        private void KreirajAdapter(string selectUpit, string insertUpit, string deleteUpit, string updateUpit)
        {
            SqlCommand pomSelectKomanda, pomInsertKomanda, pomDeleteKomanda, pomUpdateKomanda;

            pomSelectKomanda = new SqlCommand();
            pomSelectKomanda.CommandText = selectUpit;
            pomSelectKomanda.Connection = _konekcijaObjekat.DajKonekciju();

            pomInsertKomanda = new SqlCommand();
            pomInsertKomanda.CommandText = insertUpit;
            pomInsertKomanda.Connection = _konekcijaObjekat.DajKonekciju();

            pomDeleteKomanda = new SqlCommand();
            pomDeleteKomanda.CommandText = deleteUpit;
            pomDeleteKomanda.Connection = _konekcijaObjekat.DajKonekciju();

            pomUpdateKomanda = new SqlCommand();
            pomUpdateKomanda.CommandText = updateUpit;
            pomUpdateKomanda.Connection = _konekcijaObjekat.DajKonekciju();

            _adapterObjekat = new SqlDataAdapter();
            _adapterObjekat.SelectCommand = pomSelectKomanda;
            _adapterObjekat.InsertCommand = pomInsertKomanda;
            _adapterObjekat.UpdateCommand = pomUpdateKomanda;
            _adapterObjekat.DeleteCommand = pomDeleteKomanda;
        }

        // Izvršava SELECT komandu iz adaptera i rezultat smešta u DataSet pod nazivom konkretne tabele.
        private void KreirajDataset()
        {
            _dataSetObjekat = new DataSet();
            _adapterObjekat.Fill(_dataSetObjekat, _nazivTabele);

        }

        // Oslobađa memoriju i resurse koje koriste SqlDataAdapter i DataSet kada više nisu potrebni.
        private void ZatvoriAdapterDataset()
        {
            _adapterObjekat.Dispose();
            _dataSetObjekat.Dispose();
        }

        #endregion

        #region Javne metode

        // Prima običan SELECT upit, izvršava ga preko zajedničkog adaptera i vraća dobijene redove u DataSet-u.
        public DataSet DajPodatke(string selectUpit)
        // izdvaja podatke u odnosu na dat selectupit
        {
            KreirajAdapter(selectUpit, "", "", "");
            KreirajDataset();
            return _dataSetObjekat;
        }

        // Izvršava parametrizovan SELECT upit. Vrednosti se prosleđuju kao SqlParameter objekti,
        // pa se korisnički unos ne spaja direktno sa SQL tekstom. Rezultat se vraća kao DataSet.
        public DataSet DajPodatke(
            string selectUpit,
            params SqlParameter[] parametri)
        {
            DataSet rezultat = new DataSet();

            using SqlCommand komanda = new SqlCommand(
                selectUpit,
                _konekcijaObjekat.DajKonekciju());

            if (parametri != null && parametri.Length > 0)
            {
                komanda.Parameters.AddRange(parametri);
            }

            using SqlDataAdapter adapter = new SqlDataAdapter(komanda);
            adapter.Fill(rezultat, _nazivTabele);

            return rezultat;
        }

        // Izvršava parametrizovan INSERT, UPDATE ili DELETE upit.
        // ExecuteNonQuery vraća broj redova koji su promenjeni u bazi.
        public int IzvrsiAzuriranjeParametrizovano(
            string upit,
            params SqlParameter[] parametri)
        {
            SqlConnection konekcija = _konekcijaObjekat.DajKonekciju();

            using SqlCommand komanda = new SqlCommand(upit, konekcija);

            if (parametri != null && parametri.Length > 0)
            {
                komanda.Parameters.AddRange(parametri);
            }

            return komanda.ExecuteNonQuery();
        }

        // Izvršava upit koji treba da vrati samo jednu vrednost.
        // Koristi se, na primer, posle INSERT-a kada treba vratiti ID novododatog zapisa.
        public object? IzvrsiSkalar(
            string upit,
            params SqlParameter[] parametri)
        {
            SqlConnection konekcija = _konekcijaObjekat.DajKonekciju();

            using SqlCommand komanda = new SqlCommand(upit, konekcija);

            if (parametri != null && parametri.Length > 0)
            {
                komanda.Parameters.AddRange(parametri);
            }

            return komanda.ExecuteScalar();
        }

        // Vraća broj redova koji se trenutno nalaze u prvom DataTable objektu unutar učitanog DataSet-a.
        public int DajBrojSlogova()
        {
            int BrojSlogova = _dataSetObjekat.Tables[0].Rows.Count;
            return BrojSlogova;
        }

        // Izvršava jedan INSERT, UPDATE ili DELETE upit unutar SQL transakcije.
        // Ako se upit uspešno izvrši, transakcija se potvrđuje sa Commit(); u slučaju greške
        // radi se Rollback() kako bi baza ostala u prethodnom ispravnom stanju.
        public bool IzvrsiAzuriranje(string Upit)
        // izvrzava azuriranje unos/brisanje/izmena u odnosu na dati i upit
        {

            //
            bool uspeh = false;
            SqlConnection pomKonekcija;
            SqlCommand pomKomanda;
            SqlTransaction pomTransakcija = null;
            try
            {
                pomKonekcija = _konekcijaObjekat.DajKonekciju();
                // aktivan kod  

                // povezivanje
                pomKomanda = new SqlCommand();
                pomKomanda.Connection = pomKonekcija;
                pomKomanda = pomKonekcija.CreateCommand();
                // pokretanje
                // NE TREBA OPEN JER DOBIJAMO OTVORENU KONEKCIJU KROZ KONSTRUKTOR
                // mKonekcija.Open();
                pomTransakcija = pomKonekcija.BeginTransaction();
                pomKomanda.Transaction = pomTransakcija;
                pomKomanda.CommandText = Upit;
                pomKomanda.ExecuteNonQuery();
                pomTransakcija.Commit();
                uspeh = true;
            }
            catch
            {
                pomTransakcija.Rollback();
                uspeh = false;
            }
            return uspeh;
        }

        // Overload metoda koja prima više SQL upita i izvršava ih u jednoj transakciji.
        // Svi upiti moraju uspešno da se izvrše da bi se uradio Commit(); ako bilo koji padne,
        // Rollback() poništava sve prethodno izvršene upite iz iste transakcije.
        public bool IzvrsiAzuriranje(List<string> listaUpita)
        // izvrzava azuriranje unos/brisanje/izmena 
        // moze se dodeliti kao parametar lista od vise upita
        // sada transakcija ima smisla, jer izvrsava vise upita u paketu
        {

            //
            bool uspeh = false;
            SqlConnection pomKonekcija;
            SqlCommand pomKomanda;
            SqlTransaction pomTransakcija = null;
            try
            {
                pomKonekcija = _konekcijaObjekat.DajKonekciju();
                // aktivan kod  

                // povezivanje
                pomKomanda = new SqlCommand();
                pomKomanda.Connection = pomKonekcija;
                pomKomanda = pomKonekcija.CreateCommand();
                // pokretanje
                // NE TREBA OPEN JER DOBIJAMO OTVORENU KONEKCIJU KROZ KONSTRUKTOR
                // mKonekcija.Open();
                string pomUpit = "";
                pomTransakcija = pomKonekcija.BeginTransaction();
                pomKomanda.Transaction = pomTransakcija;
                for (int i = 0; i < listaUpita.Count(); i++)
                {
                    pomUpit = listaUpita[i];
                    pomKomanda.CommandText = pomUpit;
                    pomKomanda.ExecuteNonQuery();
                }
                pomTransakcija.Commit();
                uspeh = true;
            }
            catch
            {
                pomTransakcija.Rollback();
                uspeh = false;
            }
            return uspeh;
        }


        #endregion

    }
}
