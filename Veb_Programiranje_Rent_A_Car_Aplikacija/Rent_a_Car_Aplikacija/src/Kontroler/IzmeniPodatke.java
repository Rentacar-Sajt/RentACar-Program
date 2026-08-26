package Kontroler;

import java.awt.Color;
import java.awt.Dimension;
import java.awt.Font;
import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.Insets;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.sql.SQLException;
import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JTextField;

import Model.Admin;
import Model.BazaPodataka;
import Model.Klijent;
import Model.Korisnik;
import Model.Operacije;

public class IzmeniPodatke implements Operacije {

    @Override
    public void operacije(BazaPodataka bazaPodataka, JFrame okvir, Korisnik korisnik) {
        // Podesavanje okvira
        okvir.getContentPane().removeAll();
        okvir.setSize(1200, 800);
        okvir.setLocationRelativeTo(null);
        okvir.setLayout(new GridBagLayout());
        okvir.getContentPane().setBackground(new Color(250, 206, 27));

        // Naslov
        JLabel naslov = new JLabel("Izmena Podataka", JLabel.CENTER);
        naslov.setFont(new Font("Arial", Font.BOLD, 35));
        naslov.setForeground(Color.DARK_GRAY);
        
        GridBagConstraints gbc = new GridBagConstraints();
        gbc.gridx = 0;
        gbc.gridy = 0;
        gbc.gridwidth = 2;
        gbc.insets = new Insets(20, 0, 40, 0);
        okvir.add(naslov, gbc);

        // Panel za unos podataka
        JPanel unosPanel = new JPanel(new GridBagLayout());
        unosPanel.setBackground(new Color(250, 206, 27));
        GridBagConstraints c = new GridBagConstraints();
        c.insets = new Insets(10, 10, 10, 10);

        // Unos Ime
        JLabel imeLabel = new JLabel("Unesi ime: ");
        imeLabel.setFont(new Font("Arial", Font.PLAIN, 25));
        c.gridx = 0;
        c.gridy = 0;
        unosPanel.add(imeLabel, c);

        JTextField imeField = new JTextField(20);
        imeField.setFont(new Font("Arial", Font.PLAIN, 25));
        imeField.setText(korisnik.getIme());
        c.gridx = 1;
        unosPanel.add(imeField, c);

        // Unos Prezime
        JLabel prezimeLabel = new JLabel("Unesi prezime: ");
        prezimeLabel.setFont(new Font("Arial", Font.PLAIN, 25));
        c.gridy = 1;
        c.gridx = 0;
        unosPanel.add(prezimeLabel, c);

        JTextField prezimeField = new JTextField(20);
        prezimeField.setFont(new Font("Arial", Font.PLAIN, 25));
        prezimeField.setText(korisnik.getPrezime());
        c.gridx = 1;
        unosPanel.add(prezimeField, c);

        // Unos Mejl
        JLabel mejlLabel = new JLabel("Unesi mejl: ");
        mejlLabel.setFont(new Font("Arial", Font.PLAIN, 25));
        c.gridy = 2;
        c.gridx = 0;
        unosPanel.add(mejlLabel, c);

        JTextField mejlField = new JTextField(20);
        mejlField.setFont(new Font("Arial", Font.PLAIN, 25));
        mejlField.setText(korisnik.getMejl());
        c.gridx = 1;
        unosPanel.add(mejlField, c);

        // Unos Broj Telefona
        JLabel brojTelefonaLabel = new JLabel("Unesi broj telefona: ");
        brojTelefonaLabel.setFont(new Font("Arial", Font.PLAIN, 25));
        c.gridy = 3;
        c.gridx = 0;
        unosPanel.add(brojTelefonaLabel, c);

        JTextField brojTelefonaField = new JTextField(20);
        brojTelefonaField.setFont(new Font("Arial", Font.PLAIN, 25));
        brojTelefonaField.setText(korisnik.getBrojTelefona());
        c.gridx = 1;
        unosPanel.add(brojTelefonaField, c);

        gbc.gridy = 1;
        gbc.gridwidth = 1;
        okvir.add(unosPanel, gbc);

        // Panel za dugmad
        JPanel dugmadPanel = new JPanel();
        dugmadPanel.setBackground(new Color(250, 206, 27));

        JButton izmeniButton = new JButton("Izmeni");
        izmeniButton.setFont(new Font("Arial", Font.BOLD, 25));
        izmeniButton.setForeground(Color.WHITE);
        izmeniButton.setBackground(Color.DARK_GRAY);
        izmeniButton.setPreferredSize(new Dimension(200, 50));

        JButton povratakButton = new JButton("Nazad");
        povratakButton.setFont(new Font("Arial", Font.BOLD, 25));
        povratakButton.setForeground(Color.WHITE);
        povratakButton.setBackground(Color.DARK_GRAY);
        povratakButton.setPreferredSize(new Dimension(200, 50));

        dugmadPanel.add(izmeniButton);
        dugmadPanel.add(povratakButton);

        gbc.gridy = 2;
        gbc.insets = new Insets(30, 0, 30, 0);
        okvir.add(dugmadPanel, gbc);

        // Akcija za izmenu podataka
        izmeniButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                String ime = imeField.getText();
                String prezime = prezimeField.getText();
                String mejl = mejlField.getText();
                String brojTelefona = brojTelefonaField.getText();

                String update = "UPDATE korisnici SET Ime= '" + ime + "',Prezime='" + prezime + "', Mejl = '" + mejl + "',BrojTelefona = '" + brojTelefona + "' WHERE ID = '" + korisnik.getID() + "';";

                try {
                    bazaPodataka.getStatement().executeUpdate(update);
                    JOptionPane.showMessageDialog(okvir, "Uspešno ste izmenili podatke.", "Uspešno", JOptionPane.INFORMATION_MESSAGE);
                } catch (SQLException ex) {
                    ex.printStackTrace();
                    JOptionPane.showMessageDialog(okvir, "Došlo je do greške prilikom izmene podataka.", "Greška", JOptionPane.ERROR_MESSAGE);
                }
            }
        });

        // Akcija za povratak
        povratakButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                okvir.dispose(); // Zatvaranje trenutnog prozora
                
                // Provera tipa korisnika i povratak na odgovarajući meni
                if (korisnik instanceof Admin) {
                    // Prikaz admin menija
                    JFrame adminMeniOkvir = new JFrame("Admin Meni");
                    adminMeniOkvir.setSize(600, 400);
                    adminMeniOkvir.setLocationRelativeTo(null);
                    adminMeniOkvir.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
                    adminMeniOkvir.getContentPane().setBackground(new Color(250, 206, 27)); // Postavljanje pozadine na istu boju

                    Login_Prijava.prikaziAdminMeni(bazaPodataka, adminMeniOkvir, (Admin) korisnik);
                    adminMeniOkvir.setVisible(true);
                } else if (korisnik instanceof Klijent) {
                    // Prikaz klijent menija
                    JFrame klijentMeniOkvir = new JFrame("Klijent Meni");
                    klijentMeniOkvir.setSize(600, 400);
                    klijentMeniOkvir.setLocationRelativeTo(null);
                    klijentMeniOkvir.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
                    klijentMeniOkvir.getContentPane().setBackground(new Color(250, 206, 27)); // Postavljanje pozadine na istu boju

                    Login_Prijava.prikaziKlijentMeni(bazaPodataka, klijentMeniOkvir, (Klijent) korisnik);
                    klijentMeniOkvir.setVisible(true);
                }
            }
        });

        // Osvježavanje GUI-a
        okvir.revalidate();
        okvir.repaint();
    }
}
