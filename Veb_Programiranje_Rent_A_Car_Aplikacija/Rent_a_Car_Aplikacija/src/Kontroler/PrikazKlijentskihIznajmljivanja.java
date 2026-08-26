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
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JTable;
import javax.swing.SwingUtilities;
import javax.swing.table.DefaultTableModel;

import Model.Automobili;
import Model.BazaPodataka;
import Model.Iznajmljivanje;
import Model.Klijent;
import Model.Korisnik;
import Model.Operacije;

public class PrikazKlijentskihIznajmljivanja implements Operacije {

    private int korisnickiID;

    public PrikazKlijentskihIznajmljivanja(int korisnickiID) {
        this.korisnickiID = korisnickiID;
    }

    @Override
    public void operacije(BazaPodataka bazapodataka, JFrame okvir, Korisnik korisnik) {
        SwingUtilities.invokeLater(() -> {
            okvir.getContentPane().removeAll();
            okvir.setSize(1000, 800);
            okvir.setLocationRelativeTo(null);
            okvir.setLayout(new BorderLayout());

            JLabel naslov = new JLabel("Prikaz Iznajmljenih Vozila", JLabel.CENTER);
            naslov.setFont(new Font("Arial", Font.BOLD, 30));
            okvir.add(naslov, BorderLayout.NORTH);

            JPanel panel = new JPanel();
            panel.setLayout(new BorderLayout());

            String[] kolone = { "ID", "Ime", "Mejl", "Broj telefona", "ID Vozila", "Automobil", "Datum", "Sati", "Ukupno", "Status" };
            DefaultTableModel model = new DefaultTableModel(kolone, 0);

            try {
                // Aktivna iznajmljivanja klijenta čitaju se iz SQL View-a Prikaz_Iznajmljivanja.
                // View spaja tabele iznajmljivanjekola, korisnici i kola, pa nisu potrebni dodatni SELECT upiti.
                String select = "SELECT * FROM dbo.Prikaz_Iznajmljivanja "
                        + "WHERE korisnikID = " + korisnickiID + " AND status = 0;";

                ResultSet rs = bazapodataka.getStatement().executeQuery(select);

                while (rs.next()) {
                    String datum = rs.getString("datum") != null ? rs.getString("datum") : "N/A";
                    String status = rs.getInt("status") == 0 ? "Aktivno" : "Završeno";

                    Object[] red = {
                        rs.getInt("iznajmljivanjeID"),
                        rs.getString("Ime") + " " + rs.getString("Prezime"),
                        rs.getString("Mejl"),
                        rs.getString("BrojTelefona"),
                        rs.getInt("automobilID"),
                        rs.getString("Brend") + " " + rs.getString("Model") + " " + rs.getString("Boja"),
                        datum,
                        rs.getInt("sati"),
                        rs.getDouble("ukupno"),
                        status
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

            JPanel bottomPanel = new JPanel();
            bottomPanel.setLayout(new BorderLayout());

            JButton returnSelectedButton = new JButton("Vrati Izabrano Vozilo");
            returnSelectedButton.setFont(new Font("Arial", Font.BOLD, 20));
            returnSelectedButton.setPreferredSize(new Dimension(300, 50));
            returnSelectedButton.addActionListener(new ActionListener() {
                @Override
                public void actionPerformed(ActionEvent e) {
                    int selectedRow = tabela.getSelectedRow();
                    if (selectedRow != -1) {
                        int iznajmljivanjeID = (int) model.getValueAt(selectedRow, 0);
                        int automobilID = (int) model.getValueAt(selectedRow, 4);

                        int option = JOptionPane.showConfirmDialog(okvir, "Da li ste sigurni da želite vratiti izabrano vozilo?", "Potvrda", JOptionPane.YES_NO_OPTION);
                        if (option == JOptionPane.YES_OPTION) {
                            try {
                                // Ažuriranje statusa iznajmljivanja i vozila u bazi podataka
                                String updateIznajmljivanje = "UPDATE iznajmljivanjekola SET status = 1 WHERE ID = " + iznajmljivanjeID;
                                bazapodataka.getStatement().executeUpdate(updateIznajmljivanje);
                                String updateAutomobil = "UPDATE kola SET Dostupnost = 0 WHERE ID = " + automobilID;
                                bazapodataka.getStatement().executeUpdate(updateAutomobil);

                                JOptionPane.showMessageDialog(okvir, "Vozilo je uspešno vraćeno.");
                                // Osvježavanje tabele nakon vraćanja vozila
                                model.removeRow(selectedRow);
                            } catch (SQLException ex) {
                                ex.printStackTrace();
                            }
                        }
                    } else {
                        JOptionPane.showMessageDialog(okvir, "Niste izabrali nijedno vozilo za vraćanje.");
                    }
                }
            });

            bottomPanel.add(returnSelectedButton, BorderLayout.WEST);

            JButton povratakButton = new JButton("Povratak");
            povratakButton.setFont(new Font("Arial", Font.BOLD, 20));
            povratakButton.setPreferredSize(new Dimension(200, 50));
            povratakButton.addActionListener(new ActionListener() {
                @Override
                public void actionPerformed(ActionEvent e) {
                    okvir.dispose();  // Zatvoriti trenutni prozor

                    JFrame klijentMeniOkvir = new JFrame("Klijent Meni");
                    klijentMeniOkvir.setSize(600, 400);
                    klijentMeniOkvir.setLocationRelativeTo(null);
                    klijentMeniOkvir.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
                    klijentMeniOkvir.getContentPane().setBackground(new Color(250, 206, 27)); 

                    Login_Prijava.prikaziKlijentMeni(bazapodataka, klijentMeniOkvir, (Klijent) korisnik);
                    klijentMeniOkvir.setVisible(true);
                }
            });

            bottomPanel.add(povratakButton, BorderLayout.EAST);

            okvir.add(bottomPanel, BorderLayout.SOUTH);
            okvir.revalidate();
            okvir.repaint();
        });
    }
}
