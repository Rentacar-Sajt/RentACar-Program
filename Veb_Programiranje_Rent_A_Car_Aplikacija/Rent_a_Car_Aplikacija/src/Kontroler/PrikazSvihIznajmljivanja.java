package Kontroler;

import java.awt.BorderLayout;
import java.awt.Color;
import java.awt.Dimension;
import java.awt.Font;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.LocalTime;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JTable;
import javax.swing.table.DefaultTableModel;

import Model.Admin;
import Model.Automobili;
import Model.BazaPodataka;
import Model.Iznajmljivanje;
import Model.Klijent;
import Model.Korisnik;
import Model.Operacije;

public class PrikazSvihIznajmljivanja implements Operacije {

    @Override
    public void operacije(BazaPodataka bazapodataka, JFrame okvir, Korisnik korisnik) {
        // Postavljanje osnovnog okvira
        okvir.getContentPane().removeAll();
        okvir.setSize(1000, 800);
        okvir.setLocationRelativeTo(null);
        okvir.setLayout(new BorderLayout());

        // Naslov
        JLabel naslov = new JLabel("Prikaz Trenutnih Iznajmljivanja", JLabel.CENTER);
        naslov.setFont(new Font("Arial", Font.BOLD, 30));
        okvir.add(naslov, BorderLayout.NORTH);

        // Panel za prikaz iznajmljenih vozila
        JPanel panel = new JPanel();
        panel.setLayout(new BorderLayout());

        String[] kolone = { "ID", "Ime", "Mejl", "Broj telefona", "ID Vozila", "Automobil", "Datum", "Sati", "Ukupno", "Status" };
        DefaultTableModel model = new DefaultTableModel(kolone, 0);

        ArrayList<Iznajmljivanje<Automobili>> iznajmljivanjekola = new ArrayList<>();
        ArrayList<Integer> autoIDs = new ArrayList<>();
        ArrayList<Integer> korisnickiIDs = new ArrayList<>();

        try {
            // Trenutna iznajmljivanja se čitaju direktno iz SQL View-a TrenutnaIznajmljivanja.
            // View već spaja korisnike, vozila i iznajmljivanja i prikazuje samo aktivne zapise.
            String select = "SELECT * FROM dbo.TrenutnaIznajmljivanja;";
            ResultSet rs = bazapodataka.getStatement().executeQuery(select);

            while (rs.next()) {
                Object[] red = {
                    rs.getInt("IznajmljivanjeID"),
                    rs.getString("Ime"),
                    rs.getString("Mejl"),
                    rs.getString("BrojTelefona"),
                    rs.getInt("VoziloID"),
                    rs.getString("Automobil"),
                    rs.getString("Datum"),
                    rs.getInt("Sati"),
                    rs.getDouble("Ukupno"),
                    rs.getString("Status")
                };
                model.addRow(red);
            }
            rs.close();
        } catch (SQLException e) {
            e.printStackTrace();
        }

        JTable tabela = new JTable(model);
        tabela.setFont(new Font("Arial", Font.PLAIN, 18));
        tabela.setRowHeight(30);

        JScrollPane scrollPane = new JScrollPane(tabela);
        panel.add(scrollPane, BorderLayout.CENTER);
        okvir.add(panel, BorderLayout.CENTER);

        // Dugme za povratak
        JButton povratakButton = new JButton("Povratak");
        povratakButton.setFont(new Font("Arial", Font.BOLD, 20));
        povratakButton.setPreferredSize(new Dimension(200, 50));
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
                Login_Prijava.prikaziAdminMeni(bazapodataka, adminMeniOkvir, (Admin) korisnik);

                adminMeniOkvir.setVisible(true);
            }
        });

        okvir.add(povratakButton, BorderLayout.SOUTH);

        // Osvježavanje GUI-a
        okvir.revalidate();
        okvir.repaint();
    }
}
