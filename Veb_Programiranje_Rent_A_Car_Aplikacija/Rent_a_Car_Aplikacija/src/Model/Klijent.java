package Model;

import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import java.awt.GridLayout;

import Kontroler.IznajmiVozilo;
import Kontroler.PregledVozila;
import Kontroler.PrikazKlijentskihIznajmljivanja;
import Kontroler.PromenaSifre;
import Kontroler.VratiVozilo;
import Kontroler.IzmeniPodatke;
import Kontroler.Izlaz;

public class Klijent extends Korisnik {

    public Klijent() {
        super();
    }

    @Override
    public void prikaziListu(BazaPodataka bazaPodataka, JFrame okvir) {
        // Koristimo isti okvir i veličinu kao u Login_Prijava klasi
        okvir.getContentPane().removeAll();
        okvir.setLayout(new GridLayout(0, 1, 10, 10));
        okvir.setSize(1200, 800);  // Postavljanje veličine okvira na 1200x800
        okvir.setLocationRelativeTo(null);

        // Kreiranje dugmadi za klijentove opcije
        JButton pregledDostupnihVozilaButton = new JButton("Pregled svih vozila");
        JButton rezervisiVoziloButton = new JButton("Rezerviši vozilo");
        JButton pregledRezervacijaButton = new JButton("Pregled mojih rezervacija");
        
        JButton izmeniPodatkeButton = new JButton("Izmeni moje podatke");
        JButton promeniSifruButton = new JButton("Promeni šifru");
        JButton izlazButton = new JButton("Izlaz");

        // Dodavanje dugmadi u okvir
        okvir.add(pregledDostupnihVozilaButton);
        okvir.add(rezervisiVoziloButton);
        okvir.add(pregledRezervacijaButton);
        
        okvir.add(izmeniPodatkeButton);
        okvir.add(promeniSifruButton);
        okvir.add(izlazButton);

        // Osvježavanje GUI-a
        okvir.revalidate();
        okvir.repaint();

        // Dodavanje akcija za dugmad
        pregledDostupnihVozilaButton.addActionListener(e -> {
            PregledVozila pregledVozila = new PregledVozila();
            pregledVozila.operacije(bazaPodataka, okvir, this);
        });

        rezervisiVoziloButton.addActionListener(e -> {
            IznajmiVozilo iznajmiVozilo = new IznajmiVozilo();
            iznajmiVozilo.operacije(bazaPodataka, okvir, this);
        });

        pregledRezervacijaButton.addActionListener(e -> {
            PrikazKlijentskihIznajmljivanja prikazklijentskihiznajmljivanja = new PrikazKlijentskihIznajmljivanja(getID());
            prikazklijentskihiznajmljivanja.operacije(bazaPodataka, okvir, this);
        });

       

        izmeniPodatkeButton.addActionListener(e -> {
            IzmeniPodatke izmeniPodatke = new IzmeniPodatke();
            izmeniPodatke.operacije(bazaPodataka, okvir, this);
        });

        promeniSifruButton.addActionListener(e -> {
            PromenaSifre promenaSifre = new PromenaSifre();
            promenaSifre.operacije(bazaPodataka, okvir, this);
        });

        izlazButton.addActionListener(e -> {
            Izlaz izlaz = new Izlaz();
            izlaz.operacije(bazaPodataka, okvir, this);
            okvir.dispose();  // Zatvaranje prozora
        });
    }
}
