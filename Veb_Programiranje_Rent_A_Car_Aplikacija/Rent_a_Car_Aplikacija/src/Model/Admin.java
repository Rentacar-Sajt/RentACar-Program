package Model;

import java.awt.GridLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JOptionPane;
import Kontroler.AzuriranjeVozila;
import Kontroler.DodajNoviAuto;
import Kontroler.DodajNoviNalog;
import Kontroler.Izlaz;
import Kontroler.IzmeniPodatke;
import Kontroler.ObrisiVozilo;
import Kontroler.PregledVozila;
import Kontroler.PrikazSvihIznajmljivanja;
import Kontroler.PromenaSifre;

public class Admin extends Korisnik {

    public Admin() {
        super();
    }

    @Override
    public void prikaziListu(BazaPodataka bazaPodataka, JFrame okvir) {
        okvir.getContentPane().removeAll();  // Čišćenje prethodnih komponenti
        okvir.setLayout(new GridLayout(0, 1, 10, 10));  // Postavljanje layouta u GridLayout

        // Kreiranje dugmadi za svaku opciju
        JButton dodajVoziloButton = new JButton("Dodaj novo vozilo");
        JButton pregledVozilaButton = new JButton("Pregled vozila");
        JButton azurirajVoziloButton = new JButton("Ažuriranje vozila");
        JButton obrisiVoziloButton = new JButton("Brisanje vozila");
        JButton dodajAdminaButton = new JButton("Dodaj novog admina");
        JButton prikazIznajmljivanjaButton = new JButton("Prikaži iznajmljena vozila");
        JButton izmeniPodatkeButton = new JButton("Izmeni moje podatke");
        JButton promeniSifruButton = new JButton("Promeni šifru");
        JButton izlazButton = new JButton("Izlaz");

        // Dodavanje dugmadi u okvir
        okvir.add(dodajVoziloButton);
        okvir.add(pregledVozilaButton);
        okvir.add(azurirajVoziloButton);
        okvir.add(obrisiVoziloButton);
        okvir.add(dodajAdminaButton);
        okvir.add(prikazIznajmljivanjaButton);
        okvir.add(izmeniPodatkeButton);
        okvir.add(promeniSifruButton);
        okvir.add(izlazButton);

        // Postavljanje okvira da bude vidljiv
        okvir.setVisible(true);

        // Dodavanje akcija za dugmad
        dodajVoziloButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                DodajNoviAuto dodajNoviAuto = new DodajNoviAuto();
                dodajNoviAuto.operacije(bazaPodataka, okvir, Admin.this);
            }
        });

        pregledVozilaButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                PregledVozila pregledVozila = new PregledVozila();
                pregledVozila.operacije(bazaPodataka, okvir, Admin.this);
            }
        });

        azurirajVoziloButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                AzuriranjeVozila azuriranjeVozila = new AzuriranjeVozila();
                azuriranjeVozila.operacije(bazaPodataka, okvir, Admin.this);
            }
        });

        obrisiVoziloButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                ObrisiVozilo obrisiVozilo = new ObrisiVozilo();
                obrisiVozilo.operacije(bazaPodataka, okvir, Admin.this);
            }
        });

        dodajAdminaButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                DodajNoviNalog dodajNoviNalog = new DodajNoviNalog(1);  // 1 označava tip naloga kao admin
                dodajNoviNalog.operacije(bazaPodataka, okvir, Admin.this);
            }
        });

        prikazIznajmljivanjaButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                PrikazSvihIznajmljivanja prikazSvihIznajmljivanja = new PrikazSvihIznajmljivanja();
                prikazSvihIznajmljivanja.operacije(bazaPodataka, okvir, Admin.this);
            }
        });

        izmeniPodatkeButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                IzmeniPodatke izmeniPodatke = new IzmeniPodatke();
                izmeniPodatke.operacije(bazaPodataka, okvir, Admin.this);
            }
        });

        promeniSifruButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                PromenaSifre promenaSifre = new PromenaSifre();
                promenaSifre.operacije(bazaPodataka, okvir, Admin.this);
            }
        });

        izlazButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                Izlaz izlaz = new Izlaz();
                izlaz.operacije(bazaPodataka, okvir, Admin.this);
                okvir.dispose();  // Zatvaranje prozora
            }
        });
    }
}
