using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaKlasa.TehnoloskeKlase
{
    // Uloga klase: BaznaKonekcijaNova grupiše podatke i/ili operacije koje pripadaju istoj funkcionalnoj celini.
    public class BaznaKonekcijaNova
    {
        private SqlConnection? konekcija;
        public string KonekcioniString { get; set; }

        public BaznaKonekcijaNova(string KonekcioniString) { 
                    this.KonekcioniString = KonekcioniString;
        }

        // Kreira i otvara konekciju prema SQL Server bazi koristeći podešeni konekcioni string.
        public void OtvoriKonekciju()
        {
            konekcija = new SqlConnection(KonekcioniString);
            konekcija.Open();

        }

        // Zatvara otvorenu konekciju prema bazi kako bi se oslobodili resursi.
        public void ZatvoriKonekciju()
        {
            if (konekcija != null && konekcija.State == System.Data.ConnectionState.Open)
            {
                konekcija.Close();
            }
        }


    }
}
