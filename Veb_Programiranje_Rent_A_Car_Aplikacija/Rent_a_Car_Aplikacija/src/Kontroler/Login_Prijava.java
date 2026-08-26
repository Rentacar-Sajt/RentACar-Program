package Kontroler;

import java.awt.BorderLayout;
import java.awt.Color;
import java.awt.Dimension;
import java.awt.Font;
import java.awt.GridLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import javax.swing.BorderFactory;
import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JPasswordField;
import javax.swing.JTextField;

import Model.Admin;
import Model.BazaPodataka;
import Model.Klijent;
import Model.Korisnik;

public class Login_Prijava {

    public static void main(String[] args) {
        BazaPodataka bazaPodataka = new BazaPodataka();

        JFrame okvir = new JFrame("Prijava");
        okvir.setSize(600, 330);
        okvir.setLocationRelativeTo(null);
        okvir.getContentPane().setBackground(new Color(250, 206, 27));
        okvir.setLayout(new BorderLayout());

        // Povećanje fonta za naslov
        JLabel naslov = new JLabel("Dobrodošli u rent a car aplikaciju", JLabel.CENTER);
        naslov.setFont(new Font("Arial", Font.BOLD, 24));  // Veličina fonta postavljena na 24
        naslov.setBorder(BorderFactory.createEmptyBorder(20, 0, 0, 0));
        okvir.add(naslov, BorderLayout.NORTH);

        JPanel panel = new JPanel(new GridLayout(3, 2, 15, 15));
        panel.setBackground(null);
        panel.setBorder(BorderFactory.createEmptyBorder(20, 20, 20, 20));

        // Kreiranje fonta za oznake i polja
        Font font = new Font("Arial", Font.BOLD, 20);

        JLabel labelMejl = new JLabel("Mejl:", JLabel.RIGHT);
        labelMejl.setFont(font);
        panel.add(labelMejl);

        JTextField mejl1 = new JTextField(22);
        mejl1.setFont(font);
        panel.add(mejl1);

        JLabel labelSifra = new JLabel("Šifra:", JLabel.RIGHT);
        labelSifra.setFont(font);
        panel.add(labelSifra);

        JPasswordField sifra1 = new JPasswordField(22);
        sifra1.setFont(font);
        panel.add(sifra1);

        JButton napraviNalog = new JButton("Napravi novi nalog");
        napraviNalog.setFont(font);
        panel.add(napraviNalog);

        JButton prijava = new JButton("Prijava");
        prijava.setFont(font);
        panel.add(prijava);

        okvir.add(panel, BorderLayout.CENTER);
        okvir.setVisible(true);
        okvir.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);

        prijava.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                String unetiMejl = mejl1.getText();
                String unetaSifra = new String(sifra1.getPassword());

                try {
                    Statement statement = bazaPodataka.getStatement();
                    if (statement == null) {
                        throw new SQLException("Statement nije inicijalizovan.");
                    }

                    String query = "SELECT * FROM korisnici WHERE Mejl = '" + unetiMejl + "' AND Sifra = '" + unetaSifra + "';";
                    ResultSet rs = statement.executeQuery(query);

                    if (rs.next()) {
                        Korisnik korisnik;
                        int tip = rs.getInt("Tip");
                        if (tip == 0) {
                            korisnik = new Klijent();
                        } else if (tip == 1) {
                            korisnik = new Admin();
                        } else {
                            throw new SQLException("Nepoznat tip korisnika.");
                        }

                        korisnik.setID(rs.getInt("ID"));
                        korisnik.setIme(rs.getString("Ime"));
                        korisnik.setPrezime(rs.getString("Prezime"));
                        korisnik.setMejl(rs.getString("Mejl"));
                        korisnik.setBrojTelefona(rs.getString("BrojTelefona"));
                        korisnik.setSifra(rs.getString("Sifra"));

                        JOptionPane.showMessageDialog(okvir, "Dobrodošli " + korisnik.getIme() + "!");

                        // Prebacivanje na odgovarajući meni
                        if (korisnik instanceof Admin) {
                            prikaziAdminMeni(bazaPodataka, okvir, (Admin) korisnik);
                        } else {
                            prikaziKlijentMeni(bazaPodataka, okvir, (Klijent) korisnik);
                        }
                    } else {
                        JOptionPane.showMessageDialog(okvir, "Neispravan mejl ili šifra. Pokušajte ponovo.", "Greška", JOptionPane.ERROR_MESSAGE);
                    }

                    rs.close();
                } catch (SQLException ex) {
                    ex.printStackTrace();
                    JOptionPane.showMessageDialog(okvir, "Došlo je do greške prilikom prijave.", "Greška", JOptionPane.ERROR_MESSAGE);
                }
            }
        });

        napraviNalog.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                DodajNoviNalog dodajNoviNalog = new DodajNoviNalog(0);  // 0 označava tip naloga kao klijent
                dodajNoviNalog.operacije(bazaPodataka, okvir, null);
            }
        });
    }

    // Metoda za prikaz adminovog menija
    static void prikaziAdminMeni(BazaPodataka bazaPodataka, JFrame okvir, Admin admin) {
        okvir.getContentPane().removeAll();  // Čišćenje prethodnih komponenti
        okvir.setSize(600, 330);  // Postavljanje veličine okvira na 1200x800
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

        // Osvježavanje GUI-a
        okvir.revalidate();
        okvir.repaint();

        // Dodavanje akcija za dugmad
        dodajVoziloButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                DodajNoviAuto dodajNoviAuto = new DodajNoviAuto();
                dodajNoviAuto.operacije(bazaPodataka, okvir, admin);
            }
        });

        pregledVozilaButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                PregledVozila pregledVozila = new PregledVozila();
                pregledVozila.operacije(bazaPodataka, okvir, admin);
            }
        });

        azurirajVoziloButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                AzuriranjeVozila azuriranjeVozila = new AzuriranjeVozila();
                azuriranjeVozila.operacije(bazaPodataka, okvir, admin);
            }
        });

        obrisiVoziloButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                ObrisiVozilo obrisiVozilo = new ObrisiVozilo();
                obrisiVozilo.operacije(bazaPodataka, okvir, admin);
            }
        });

        dodajAdminaButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                DodajNoviNalog dodajNoviNalog = new DodajNoviNalog(1);  // 1 označava tip naloga kao admin
                dodajNoviNalog.operacije(bazaPodataka, okvir, admin);
            }
        });

        prikazIznajmljivanjaButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                PrikazSvihIznajmljivanja prikazSvihIznajmljivanja = new PrikazSvihIznajmljivanja();
                prikazSvihIznajmljivanja.operacije(bazaPodataka, okvir, admin);
            }
        });

        izmeniPodatkeButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                IzmeniPodatke izmeniPodatke = new IzmeniPodatke();
                izmeniPodatke.operacije(bazaPodataka, okvir, admin);
            }
        });

        promeniSifruButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                PromenaSifre promenaSifre = new PromenaSifre();
                promenaSifre.operacije(bazaPodataka, okvir, admin);
            }
        });

        izlazButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                Izlaz izlaz = new Izlaz();
                izlaz.operacije(bazaPodataka, okvir, admin);
                okvir.dispose();  // Zatvaranje prozora
            }
        });
    }

    static void prikaziKlijentMeni(BazaPodataka bazaPodataka, JFrame okvir, Klijent klijent) {
        okvir.getContentPane().removeAll();  // Čišćenje prethodnih komponenti
        okvir.setSize(600, 330);  // Postavljanje veličine okvira na 1200x800
        okvir.setLayout(new GridLayout(0, 1, 10, 10));  // Postavljanje layouta u GridLayout

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
            pregledVozila.operacije(bazaPodataka, okvir, klijent);
        });

        rezervisiVoziloButton.addActionListener(e -> {
            IznajmiVozilo iznajmiVozilo = new IznajmiVozilo();
            iznajmiVozilo.operacije(bazaPodataka, okvir, klijent);
        });

        pregledRezervacijaButton.addActionListener(e -> {
            PrikazKlijentskihIznajmljivanja prikazKlijentskihIznajmljivanja = new PrikazKlijentskihIznajmljivanja(klijent.getID());
            prikazKlijentskihIznajmljivanja.operacije(bazaPodataka, okvir, klijent);
        });


        izmeniPodatkeButton.addActionListener(e -> {
            IzmeniPodatke izmeniPodatke = new IzmeniPodatke();
            izmeniPodatke.operacije(bazaPodataka, okvir, klijent);
        });

        promeniSifruButton.addActionListener(e -> {
            PromenaSifre promenaSifre = new PromenaSifre();
            promenaSifre.operacije(bazaPodataka, okvir, klijent);
        });

        izlazButton.addActionListener(e -> {
            Izlaz izlaz = new Izlaz();
            izlaz.operacije(bazaPodataka, okvir, klijent);
            okvir.dispose();  // Zatvaranje prozora
        });
    }
}
