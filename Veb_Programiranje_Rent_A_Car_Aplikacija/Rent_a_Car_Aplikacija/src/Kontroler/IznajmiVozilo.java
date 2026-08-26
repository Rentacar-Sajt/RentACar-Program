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
import java.sql.SQLException;
import java.sql.Statement;
import java.text.SimpleDateFormat;
import java.util.Date;

import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JTable;
import javax.swing.JTextField;
import javax.swing.table.DefaultTableModel;
import javax.swing.table.JTableHeader;

import Model.Automobili;
import Model.BazaPodataka;
import Model.Klijent;
import Model.Korisnik;
import Model.Operacije;

public class IznajmiVozilo implements Operacije {

    @Override
    public void operacije(BazaPodataka bazaPodataka, JFrame okvir, Korisnik korisnik) {
        // Postavljanje osnovnog okvira
        okvir.getContentPane().removeAll();
        okvir.setSize(1200, 800);
        okvir.setLocationRelativeTo(null);
        okvir.setLayout(new BorderLayout());

        // Naslov
        JLabel naslov = new JLabel("Iznajmljivanje Vozila", JLabel.CENTER);
        naslov.setFont(new Font("Arial", Font.BOLD, 30));
        okvir.add(naslov, BorderLayout.NORTH);

        // Glavni panel
        JPanel mainPanel = new JPanel(new GridBagLayout());
        GridBagConstraints gbc = new GridBagConstraints();
        gbc.insets = new Insets(10, 10, 10, 10);
        gbc.fill = GridBagConstraints.HORIZONTAL;

        // Tabela za prikaz dostupnih vozila
        DefaultTableModel model = new DefaultTableModel();
        model.addColumn("ID");
        model.addColumn("Brend");
        model.addColumn("Model");
        model.addColumn("Boja");
        model.addColumn("Godina");
        model.addColumn("Cena");
        model.addColumn("Status");

        JTable tabela = new JTable(model);
        tabela.setFont(new Font("Arial", Font.PLAIN, 16));
        tabela.setRowHeight(30);
        JTableHeader header = tabela.getTableHeader();
        header.setFont(new Font("Arial", Font.BOLD, 18));
        header.setPreferredSize(new Dimension(header.getPreferredSize().width, 40));

        JScrollPane scrollPane = new JScrollPane(tabela);
        scrollPane.setPreferredSize(new Dimension(1100, 300));

        gbc.gridx = 0;
        gbc.gridy = 0;
        gbc.gridwidth = 2;
        mainPanel.add(scrollPane, gbc);

        // Popunjavanje modela podacima iz baze
        try {
            Statement stmt = bazaPodataka.getStatement();
            // Za prikaz vozila koja mogu da se iznajme koristi se SQL View DostupnaVozilaView.
            // U samom View-u je već postavljen uslov Dostupnost = 0.
            String query = "SELECT * FROM dbo.DostupnaVozilaView;";
            ResultSet rs = stmt.executeQuery(query);

            while (rs.next()) {
                int id = rs.getInt("ID");
                String brend = rs.getString("Brend");
                String modelStr = rs.getString("Model");
                String boja = rs.getString("Boja");
                int godina = rs.getInt("Godina");
                double cena = rs.getDouble("Cena");
                String status = "Dostupan";

                model.addRow(new Object[]{id, brend, modelStr, boja, godina, cena + "€", status});
            }
            rs.close();
        } catch (SQLException e) {
            e.printStackTrace();
        }

        // Polje za unos ID vozila
        JLabel idLabel = new JLabel("Unesite ID vozila koje želite da iznajmite:");
        idLabel.setFont(new Font("Arial", Font.PLAIN, 20));
        gbc.gridx = 0;
        gbc.gridy = 1;
        gbc.gridwidth = 1;
        mainPanel.add(idLabel, gbc);

        JTextField idField = new JTextField();
        idField.setFont(new Font("Arial", Font.PLAIN, 20));
        gbc.gridx = 1;
        gbc.gridy = 1;
        mainPanel.add(idField, gbc);

        // Polje za unos broja sati
        JLabel satiLabel = new JLabel("Unesite broj sati koliko želite da iznajmite vozilo:");
        satiLabel.setFont(new Font("Arial", Font.PLAIN, 20));
        gbc.gridx = 0;
        gbc.gridy = 2;
        mainPanel.add(satiLabel, gbc);

        JTextField satiField = new JTextField();
        satiField.setFont(new Font("Arial", Font.PLAIN, 20));
        gbc.gridx = 1;
        gbc.gridy = 2;
        mainPanel.add(satiField, gbc);

        // Dodavanje dugmadi
        JButton iznajmiButton = new JButton("Iznajmi");
        iznajmiButton.setFont(new Font("Arial", Font.BOLD, 20));
        gbc.gridx = 0;
        gbc.gridy = 3;
        mainPanel.add(iznajmiButton, gbc);

        JButton povratakButton = new JButton("Povratak");
        povratakButton.setFont(new Font("Arial", Font.BOLD, 20));
        gbc.gridx = 1;
        gbc.gridy = 3;
        mainPanel.add(povratakButton, gbc);

        okvir.add(mainPanel, BorderLayout.CENTER);

        // Akcija za dugme Iznajmi
        iznajmiButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                String idText = idField.getText().trim();
                String satiText = satiField.getText().trim();

                if (idText.isEmpty() || satiText.isEmpty()) {
                    JOptionPane.showMessageDialog(okvir, "Molimo popunite sva polja.", "Greška", JOptionPane.ERROR_MESSAGE);
                    return;
                }

                try {
                    int autoID = Integer.parseInt(idText);
                    int sati = Integer.parseInt(satiText);

                    Statement stmt = bazaPodataka.getStatement();
                    ResultSet rs = stmt.executeQuery("SELECT * FROM dbo.DostupnaVozilaView WHERE ID = " + autoID + ";");
                    if (rs.next()) {
                        Automobili automobili = new Automobili();
                        automobili.setID(rs.getInt("ID"));
                        automobili.setBrend(rs.getString("Brend"));
                        automobili.setModel(rs.getString("Model"));
                        automobili.setBoja(rs.getString("Boja"));
                        automobili.setGodina(rs.getInt("Godina"));
                        automobili.setCena(rs.getDouble("Cena"));
                        automobili.setDostupnost(rs.getInt("Dostupnost"));

                        if (automobili.isDostupnost() != 0) {
                            JOptionPane.showMessageDialog(okvir, "Vozilo nije dostupno za iznajmljivanje", "Greška", JOptionPane.ERROR_MESSAGE);
                            return;
                        }

                        // Generiši trenutni datum
                        SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd");
                        String datum = sdf.format(new Date());

                        // Pronađi poslednji ID u tabeli iznajmljivanjekola i inkrementiraj ga
                        ResultSet rs1 = stmt.executeQuery("SELECT MAX(ID) AS maxID FROM iznajmljivanjekola;");
                        int ID = 1; // Defaultna vrednost ako tabela je prazna
                        if (rs1.next()) {
                            ID = rs1.getInt("maxID") + 1;
                        }

                        double ukupno = automobili.getCena() * sati;
                        String insert = "INSERT INTO iznajmljivanjekola(ID, korisnik, automobil, datum, sati, ukupno, status) " +
                                        "VALUES (" + ID + ", '" + korisnik.getID() + "', " + automobili.getID() + ", '" + datum + "', " + sati + ", " + ukupno + ", '0');";
                        stmt.executeUpdate(insert);

                        // Ažuriraj dostupnost automobila
                        String update = "UPDATE kola SET Dostupnost = 1 WHERE ID = " + autoID + ";";
                        stmt.executeUpdate(update);

                        JOptionPane.showMessageDialog(okvir, "Vozilo je uspešno iznajmljeno.", "Uspeh", JOptionPane.INFORMATION_MESSAGE);

                    } else {
                        JOptionPane.showMessageDialog(okvir, "Vozilo sa datim ID-jem ne postoji.", "Greška", JOptionPane.ERROR_MESSAGE);
                    }

                } catch (NumberFormatException ex) {
                    JOptionPane.showMessageDialog(okvir, "ID vozila i broj sati moraju biti validni brojevi.", "Greška", JOptionPane.ERROR_MESSAGE);
                } catch (SQLException ex) {
                    ex.printStackTrace();
                }
            }
        });

        // Akcija za dugme Povratak
        povratakButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                okvir.dispose(); // Zatvaranje trenutnog prozora
                
                // Prikaz klijent menija
                JFrame klijentMeniOkvir = new JFrame("Klijent Meni");
                klijentMeniOkvir.setSize(600, 400);
                klijentMeniOkvir.setLocationRelativeTo(null);
                klijentMeniOkvir.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
                klijentMeniOkvir.getContentPane().setBackground(new Color(250, 206, 27)); // Postavljanje pozadine na istu boju

                Login_Prijava.prikaziKlijentMeni(bazaPodataka, klijentMeniOkvir, (Klijent) korisnik);
                klijentMeniOkvir.setVisible(true);
            }
        });

        // Osvježavanje GUI-a
        okvir.revalidate();
        okvir.repaint();
    }
}
