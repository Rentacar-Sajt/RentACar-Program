package Kontroler;

import java.awt.BorderLayout;
import java.awt.Color;
import java.awt.Dimension;
import java.awt.Font;
import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.Insets;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.sql.ResultSet;

import javax.swing.BorderFactory;
import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JTextField;
import javax.swing.JOptionPane;

import Model.BazaPodataka;
import Model.Korisnik;
import Model.Admin;

public class DodajNoviAuto {

    public void operacije(BazaPodataka bazaPodataka, JFrame okvir, Korisnik korisnik) {
        // Čišćenje prethodnog sadržaja okvira
        okvir.getContentPane().removeAll();
        okvir.setSize(1000, 660);
        okvir.setLocationRelativeTo(null);
        okvir.getContentPane().setBackground(new Color(250, 206, 27));
        okvir.setLayout(new BorderLayout());

        JLabel naslov = new JLabel("Dodavanje novog vozila", JLabel.CENTER);
        naslov.setFont(new Font("Arial", Font.BOLD, 30)); // Veći font
        naslov.setBorder(BorderFactory.createEmptyBorder(20, 0, 20, 0));
        okvir.add(naslov, BorderLayout.NORTH);

        JPanel panel = new JPanel(new GridBagLayout());
        panel.setBorder(BorderFactory.createEmptyBorder(20, 20, 20, 20));
        okvir.add(panel, BorderLayout.CENTER);

        GridBagConstraints gbc = new GridBagConstraints();
        gbc.insets = new Insets(10, 10, 10, 10);
        gbc.fill = GridBagConstraints.HORIZONTAL;
        gbc.weightx = 1.0;
        gbc.gridx = 0;

        // Polja za unos
        JTextField brendField = new JTextField(20); // Povećan broj kolona
        JTextField modelField = new JTextField(20);
        JTextField bojaField = new JTextField(20);
        JTextField godinaField = new JTextField(20);
        JTextField cenaField = new JTextField(20);

        // Postavljanje fonta za polja za unos
        Font font = new Font("Arial", Font.PLAIN, 25);
        brendField.setFont(font);
        modelField.setFont(font);
        bojaField.setFont(font);
        godinaField.setFont(font);
        cenaField.setFont(font);

        
        panel.add(new JLabel("Brend:"), gbc);
        
        panel.add(brendField, gbc);

        gbc.gridy = 2;
        panel.add(new JLabel("Model:"), gbc);
        gbc.gridy = 3;
        panel.add(modelField, gbc);

        gbc.gridy = 4;
        panel.add(new JLabel("Boja:"), gbc);
        gbc.gridy = 5;
        panel.add(bojaField, gbc);

        gbc.gridy = 6;
        panel.add(new JLabel("Godina:"), gbc);
        gbc.gridy = 7;
        panel.add(godinaField, gbc);

        gbc.gridy = 8;
        panel.add(new JLabel("Cena po satu:"), gbc);
        gbc.gridy = 9;
        panel.add(cenaField, gbc);

        JPanel dugmadPanel = new JPanel();  // Panel za dugmad
        dugmadPanel.setLayout(new GridBagLayout());
        gbc.gridx = 0;
        gbc.gridy = 0;
        gbc.weightx = 0.5;
        gbc.fill = GridBagConstraints.NONE;
        gbc.anchor = GridBagConstraints.CENTER;
        gbc.insets = new Insets(20, 20, 20, 20);

        JButton potvrdiButton = new JButton("Potvrdi");
        potvrdiButton.setPreferredSize(new Dimension(150, 50)); // Veća veličina dugmadi
        potvrdiButton.setFont(new Font("Arial", Font.BOLD, 18)); // Veći font za dugmadi
        dugmadPanel.add(potvrdiButton, gbc);

        gbc.gridx = 1;
        JButton povratakButton = new JButton("Povratak");
        povratakButton.setPreferredSize(new Dimension(150, 50)); // Veća veličina dugmadi
        povratakButton.setFont(new Font("Arial", Font.BOLD, 18)); // Veći font za dugmadi
        dugmadPanel.add(povratakButton, gbc);

        okvir.add(dugmadPanel, BorderLayout.SOUTH);

        potvrdiButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                String brend = brendField.getText();
                String model = modelField.getText();
                String boja = bojaField.getText();
                String godinaText = godinaField.getText();
                String cenaText = cenaField.getText();
                int dostupnost = 0;

                // Validacija unosa
                if (brend.isEmpty() || model.isEmpty() || boja.isEmpty() || godinaText.isEmpty() || cenaText.isEmpty()) {
                    JOptionPane.showMessageDialog(okvir, "Sva polja su obavezna.", "Greška", JOptionPane.ERROR_MESSAGE);
                    return;
                }

                int godina;
                double cena;
                try {
                    godina = Integer.parseInt(godinaText);
                    cena = Double.parseDouble(cenaText);
                } catch (NumberFormatException ex) {
                    JOptionPane.showMessageDialog(okvir, "Godina i cena moraju biti brojevi.", "Greška", JOptionPane.ERROR_MESSAGE);
                    return;
                }

                // SQL upit za unos novog automobila u bazu
                try {
                    // Dobijanje maksimalnog ID-a
                    ResultSet rs = bazaPodataka.getStatement().executeQuery("SELECT MAX(ID) AS maxID FROM kola;");
                    rs.next();
                    int ID = rs.getInt("maxID") + 1;

                    // Umetanje novog automobila u bazu
                    String insert = "INSERT INTO kola (ID, Brend, Model, Boja, Godina, Cena, Dostupnost) " +
                                    "VALUES (" + ID + ", '" + brend + "', '" + model + "', '" + boja + "', " + godina + ", " + cena + ", " + dostupnost + ");";
                    bazaPodataka.getStatement().executeUpdate(insert);

                    JOptionPane.showMessageDialog(okvir, "Automobil je dodat uspešno.");
                    rs.close();  // Zatvaranje ResultSet-a

                } catch (Exception ex) {
                    ex.printStackTrace();
                    JOptionPane.showMessageDialog(okvir, "Došlo je do greške prilikom dodavanja automobila.", "Greška", JOptionPane.ERROR_MESSAGE);
                }
            }
        });

        povratakButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                okvir.dispose();  // Zatvaranje trenutnog prozora

                // Kreiranje novog JFrame za admin meni
                JFrame adminMeniOkvir = new JFrame("Admin Meni");
                adminMeniOkvir.setSize(600, 400);
                adminMeniOkvir.setLocationRelativeTo(null);
                adminMeniOkvir.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
                adminMeniOkvir.getContentPane().setBackground(new Color(250, 206, 27)); // Postavljanje pozadine na istu boju

                // Prikaz admin menija
                Login_Prijava.prikaziAdminMeni(bazaPodataka, adminMeniOkvir, (Admin) korisnik);

                adminMeniOkvir.setVisible(true);
            }
        });

        // Osvježavanje GUI-a
        okvir.revalidate();
        okvir.repaint();
    }
}
