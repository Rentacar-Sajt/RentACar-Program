using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaKlasa.TehnoloskeKlase
{
    public class BaznaKonekcijaNova
    {
        private SqlConnection? konekcija;
        public string KonekcioniString { get; set; }

        public BaznaKonekcijaNova(string KonekcioniString) { 
                    this.KonekcioniString = KonekcioniString;
        }

        public void OtvoriKonekciju()
        {
            konekcija = new SqlConnection(KonekcioniString);
            konekcija.Open();

        }

        public void ZatvoriKonekciju()
        {
            if (konekcija != null && konekcija.State == System.Data.ConnectionState.Open)
            {
                konekcija.Close();
            }
        }


    }
}
