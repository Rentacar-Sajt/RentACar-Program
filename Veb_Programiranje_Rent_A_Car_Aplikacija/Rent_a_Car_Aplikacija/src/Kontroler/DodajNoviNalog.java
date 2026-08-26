package Kontroler;

import java.awt.BorderLayout;
import java.awt.Font;
import java.awt.GridLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import javax.swing.BorderFactory;
import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JTextField;
import javax.swing.JPasswordField;
import javax.swing.JOptionPane;

import Model.BazaPodataka;
import Model.Korisnik;

public class DodajNoviNalog {

    private int tipNaloga;  // 0 - Klijent, 1 - Admin

    public DodajNoviNalog(int tipNaloga) {
        this.tipNaloga = tipNaloga;
    }

    public void operacije(BazaPodataka bazaPodataka, JFrame okvir, Korisnik korisnik) {
        // Čišćenje prethodnog sadržaja okvira
        okvir.getContentPane().removeAll();
        okvir.setLayout(new BorderLayout());

        JLabel naslov = new JLabel("Dodavanje novog naloga", JLabel.CENTER);
        naslov.setFont(new Font("Arial", Font.BOLD, 24));
        naslov.setBorder(BorderFactory.createEmptyBorder(20, 0, 20, 0));
        okvir.add(naslov, BorderLayout.NORTH);

        JPanel panel = new JPanel(new GridLayout(5, 2, 10, 10));
        panel.setBorder(BorderFactory.createEmptyBorder(20, 20, 20, 20));

        // Polja za unos
        JTextField imeField = new JTextField();
        JTextField prezimeField = new JTextField();
        JTextField mejlField = new JTextField();
        JTextField telefonField = new JTextField();
        JPasswordField sifraField = new JPasswordField();

        panel.add(new JLabel("Ime:"));
        panel.add(imeField);
        panel.add(new JLabel("Prezime:"));
        panel.add(prezimeField);
        panel.add(new JLabel("Mejl:"));
        panel.add(mejlField);
        panel.add(new JLabel("BrojTelefona:"));
        panel.add(telefonField);
        panel.add(new JLabel("Sifra:"));
        panel.add(sifraField);

        okvir.add(panel, BorderLayout.CENTER);

        JPanel dugmadPanel = new JPanel();  // Panel za dugmad
        JButton potvrdiButton = new JButton("Potvrdi");
        JButton povratakButton = new JButton("Povratak");
        dugmadPanel.add(potvrdiButton);
        dugmadPanel.add(povratakButton);
        okvir.add(dugmadPanel, BorderLayout.SOUTH);

        potvrdiButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                String ime = imeField.getText();
                String prezime = prezimeField.getText();
                String mejl = mejlField.getText();
                String telefon = telefonField.getText();
                String sifra = new String(sifraField.getPassword());

                // Validacija unosa
                if (ime.isEmpty() || prezime.isEmpty() || mejl.isEmpty() || telefon.isEmpty() || sifra.isEmpty()) {
                    JOptionPane.showMessageDialog(okvir, "Sva polja su obavezna.", "Greška", JOptionPane.ERROR_MESSAGE);
                    return;
                }

                // SQL upit za unos novog korisnika u bazu
                try {
                    String query = "INSERT INTO korisnici (Ime, Prezime, Mejl, BrojTelefona, Sifra, Tip) VALUES ('"
                            + ime + "', '" + prezime + "', '" + mejl + "', '" + telefon + "', '" + sifra + "', "
                            + tipNaloga + ");";
                    System.out.println("Executing query: " + query);  // Ispis SQL upita
                    bazaPodataka.getStatement().executeUpdate(query);

                    JOptionPane.showMessageDialog(okvir, "Nalog je uspešno kreiran!");
                    okvir.dispose();
                } catch (Exception ex) {
                    ex.printStackTrace();  // Ispis detalja o grešci u konzolu
                    JOptionPane.showMessageDialog(okvir, "Došlo je do greške prilikom kreiranja naloga: " + ex.getMessage(), "Greška", JOptionPane.ERROR_MESSAGE);
                }
                okvir.dispose();  // Zatvaranje trenutnog prozora
                Login_Prijava.main(null); 
            }
        });

        povratakButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                okvir.dispose();  // Zatvaranje trenutnog prozora
                Login_Prijava.main(null);  // Ponovno pokretanje glavnog prijavnog ekrana
            }
        });

        // Osvježavanje GUI-a
        okvir.revalidate();
        okvir.repaint();
    }
}
