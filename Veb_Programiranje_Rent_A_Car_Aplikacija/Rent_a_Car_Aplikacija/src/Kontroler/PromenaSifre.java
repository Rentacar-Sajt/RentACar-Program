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
import javax.swing.JPasswordField;

import Model.Admin;
import Model.BazaPodataka;
import Model.Klijent;
import Model.Korisnik;
import Model.Operacije;

public class PromenaSifre implements Operacije {

    @Override
    public void operacije(BazaPodataka bazaPodataka, JFrame okvir, Korisnik korisnik) {
        // Podesavanje okvira
        okvir.getContentPane().removeAll();
        okvir.setSize(1200, 800);
        okvir.setLocationRelativeTo(null);
        okvir.setLayout(new GridBagLayout());
        okvir.getContentPane().setBackground(new Color(250, 206, 27));

        // Naslov
        JLabel naslov = new JLabel("Promena Šifre", JLabel.CENTER);
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

        // Unos stare šifre
        JLabel staraSifraLabel = new JLabel("Unesite staru šifru:");
        staraSifraLabel.setFont(new Font("Arial", Font.PLAIN, 25));
        c.gridx = 0;
        c.gridy = 0;
        unosPanel.add(staraSifraLabel, c);

        JPasswordField staraSifraField = new JPasswordField(20);
        staraSifraField.setFont(new Font("Arial", Font.PLAIN, 25));
        c.gridx = 1;
        unosPanel.add(staraSifraField, c);

        // Unos nove šifre
        JLabel novaSifraLabel = new JLabel("Unesite novu šifru:");
        novaSifraLabel.setFont(new Font("Arial", Font.PLAIN, 25));
        c.gridy = 1;
        c.gridx = 0;
        unosPanel.add(novaSifraLabel, c);

        JPasswordField novaSifraField = new JPasswordField(20);
        novaSifraField.setFont(new Font("Arial", Font.PLAIN, 25));
        c.gridx = 1;
        unosPanel.add(novaSifraField, c);

        // Potvrda nove šifre
        JLabel potvrdiSifruLabel = new JLabel("Potvrdite novu šifru:");
        potvrdiSifruLabel.setFont(new Font("Arial", Font.PLAIN, 25));
        c.gridy = 2;
        c.gridx = 0;
        unosPanel.add(potvrdiSifruLabel, c);

        JPasswordField potvrdiSifruField = new JPasswordField(20);
        potvrdiSifruField.setFont(new Font("Arial", Font.PLAIN, 25));
        c.gridx = 1;
        unosPanel.add(potvrdiSifruField, c);

        gbc.gridy = 1;
        gbc.gridwidth = 1;
        okvir.add(unosPanel, gbc);

        // Panel za dugmad
        JPanel dugmadPanel = new JPanel();
        dugmadPanel.setBackground(new Color(250, 206, 27));

        JButton promenaSifreButton = new JButton("Promeni Šifru");
        promenaSifreButton.setFont(new Font("Arial", Font.BOLD, 25));
        promenaSifreButton.setForeground(Color.WHITE);
        promenaSifreButton.setBackground(Color.DARK_GRAY);
        promenaSifreButton.setPreferredSize(new Dimension(250, 50));

        JButton povratakButton = new JButton("Nazad");
        povratakButton.setFont(new Font("Arial", Font.BOLD, 25));
        povratakButton.setForeground(Color.WHITE);
        povratakButton.setBackground(Color.DARK_GRAY);
        povratakButton.setPreferredSize(new Dimension(200, 50));

        dugmadPanel.add(promenaSifreButton);
        dugmadPanel.add(povratakButton);

        gbc.gridy = 2;
        gbc.insets = new Insets(30, 0, 30, 0);
        okvir.add(dugmadPanel, gbc);

        // Akcija za promenu šifre
        promenaSifreButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                String staraSifra = new String(staraSifraField.getPassword());
                String novaSifra = new String(novaSifraField.getPassword());
                String potvrdiSifru = new String(potvrdiSifruField.getPassword());

                if (!staraSifra.equals(korisnik.getSifra())) {
                    JOptionPane.showMessageDialog(okvir, "Pogrešna stara šifra.", "Greška", JOptionPane.ERROR_MESSAGE);
                    return;
                }

                if (!novaSifra.equals(potvrdiSifru)) {
                    JOptionPane.showMessageDialog(okvir, "Nova šifra se ne poklapa sa potvrdom.", "Greška", JOptionPane.ERROR_MESSAGE);
                    return;
                }

                try {
                    String update = "UPDATE korisnici SET Sifra = '" + novaSifra + "' WHERE ID = '" + korisnik.getID() + "';";
                    bazaPodataka.getStatement().executeUpdate(update);
                    korisnik.setSifra(novaSifra); // Ažuriranje šifre u objektu korisnika
                    JOptionPane.showMessageDialog(okvir, "Šifra je uspešno promenjena.", "Uspešno", JOptionPane.INFORMATION_MESSAGE);
                } catch (SQLException ex) {
                    ex.printStackTrace();
                    JOptionPane.showMessageDialog(okvir, "Došlo je do greške prilikom promene šifre.", "Greška", JOptionPane.ERROR_MESSAGE);
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
