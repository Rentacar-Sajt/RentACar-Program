package Kontroler;

import java.awt.BorderLayout;
import java.awt.Color;
import java.awt.Dimension;
import java.awt.Font;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.sql.ResultSet;
import java.sql.SQLException;

import javax.swing.BorderFactory;
import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JTable;
import javax.swing.table.DefaultTableModel;
import javax.swing.table.JTableHeader;

import Model.Admin;
import Model.BazaPodataka;
import Model.Korisnik;
import Model.Klijent;
import Model.Operacije;

public class PregledVozila implements Operacije {

    @Override
    public void operacije(BazaPodataka bazaPodataka, JFrame okvir, Korisnik korisnik) {
        // Čišćenje prethodnog sadržaja okvira
        okvir.getContentPane().removeAll();
        okvir.setSize(1000, 800); // Veća veličina okvira
        okvir.setLocationRelativeTo(null);
        okvir.setLayout(new BorderLayout());

        // Kreiranje naslova
        JLabel naslov = new JLabel("Pregled Vozila", JLabel.CENTER);
        naslov.setFont(new Font("Arial", Font.BOLD, 30)); // Veći font za naslov
        naslov.setBorder(BorderFactory.createEmptyBorder(20, 0, 20, 0));
        okvir.add(naslov, BorderLayout.NORTH);

        // Kreiranje modela tabele
        DefaultTableModel model = new DefaultTableModel();
        model.addColumn("ID");
        model.addColumn("Brend");
        model.addColumn("Model");
        model.addColumn("Boja");
        model.addColumn("Godina");
        model.addColumn("Cena");
        model.addColumn("Status");

        // Podaci za pregled vozila čitaju se iz SQL View-a VozilaView.
        // View sakriva direktan pristup tabeli kola i aplikaciji vraća samo potrebne kolone.
        try {
            String query = "SELECT ID, Brend, Model, Boja, Godina, Cena, Dostupnost FROM dbo.VozilaView;";
            ResultSet rs = bazaPodataka.getStatement().executeQuery(query);

            while (rs.next()) {
                int id = rs.getInt("ID");
                String brend = rs.getString("Brend");
                String modelStr = rs.getString("Model");
                String boja = rs.getString("Boja");
                int godina = rs.getInt("Godina");
                double cena = rs.getDouble("Cena");
                int dostupnost = rs.getInt("Dostupnost");
                String status = dostupnost == 0 ? "Dostupan" : "Nije dostupan";

                model.addRow(new Object[]{id, brend, modelStr, boja, godina, cena + "€", status});
            }
            rs.close();
        } catch (SQLException e) {
            e.printStackTrace();
        }

        // Kreiranje tabele i dodavanje u JScrollPane
        JTable tabela = new JTable(model);
        tabela.setFont(new Font("Arial", Font.PLAIN, 16)); // Veći font za tabelu
        tabela.setRowHeight(30); // Veća visina redova

        JTableHeader header = tabela.getTableHeader();
        header.setFont(new Font("Arial", Font.BOLD, 18)); // Veći font za zaglavlja kolona
        header.setPreferredSize(new Dimension(header.getPreferredSize().width, 40)); // Veće zaglavlje kolona

        JScrollPane scrollPane = new JScrollPane(tabela);
        scrollPane.setPreferredSize(new Dimension(1150, 700)); // Veća veličina JScrollPane-a
        okvir.add(scrollPane, BorderLayout.CENTER);

        // Panel za dugmad
        JPanel dugmadPanel = new JPanel();

        // Dodavanje dugmeta za povratak
        JButton povratakButton = new JButton("Povratak");
        povratakButton.setPreferredSize(new Dimension(200, 50)); // Veća veličina dugmadi
        povratakButton.setFont(new Font("Arial", Font.BOLD, 18)); // Veći font za dugmadi
        dugmadPanel.add(povratakButton);

        // Dodavanje dugmeta za vraćanje vozila
        JButton vratiVoziloButton = new JButton("Vrati Vozilo");
        vratiVoziloButton.setPreferredSize(new Dimension(200, 50)); // Veća veličina dugmadi
        vratiVoziloButton.setFont(new Font("Arial", Font.BOLD, 18)); // Veći font za dugmadi
        dugmadPanel.add(vratiVoziloButton);

        okvir.add(dugmadPanel, BorderLayout.SOUTH);

        // Akcija za dugme povratak
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

        // Akcija za dugme vrati vozilo
        vratiVoziloButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                int selectedRow = tabela.getSelectedRow();
                if (selectedRow != -1) {
                    int voziloID = (int) model.getValueAt(selectedRow, 0);
                    String status = (String) model.getValueAt(selectedRow, 6);
                    if ("Nije dostupan".equals(status)) {
                        // Ažuriranje statusa vozila u bazi podataka
                        try {
                            String updateQuery = "UPDATE kola SET Dostupnost = 0 WHERE ID = " + voziloID;
                            bazaPodataka.getStatement().executeUpdate(updateQuery);

                            // Ažuriranje statusa u tabeli
                            model.setValueAt("Dostupan", selectedRow, 6);

                            JOptionPane.showMessageDialog(okvir, "Vozilo je uspešno vraćeno.");
                        } catch (SQLException ex) {
                            ex.printStackTrace();
                            JOptionPane.showMessageDialog(okvir, "Došlo je do greške prilikom vraćanja vozila.");
                        }
                    } else {
                        JOptionPane.showMessageDialog(okvir, "Izabrano vozilo je već dostupno.");
                    }
                } else {
                    JOptionPane.showMessageDialog(okvir, "Niste izabrali nijedno vozilo.");
                }
            }
        });

        // Osvježavanje GUI-a
        okvir.revalidate();
        okvir.repaint();
    }
}
