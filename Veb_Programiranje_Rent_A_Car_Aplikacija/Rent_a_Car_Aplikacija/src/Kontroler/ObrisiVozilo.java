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
import javax.swing.JPanel;
import javax.swing.JTextField;
import javax.swing.JOptionPane;
import javax.swing.JScrollPane;
import javax.swing.JTable;
import javax.swing.SwingConstants;
import javax.swing.table.DefaultTableModel;

import Model.Admin;
import Model.BazaPodataka;
import Model.Korisnik;
import Model.Operacije;

public class ObrisiVozilo implements Operacije {

    private DefaultTableModel tableModel;
    private JTable tabela;

    @Override
    public void operacije(BazaPodataka bazaPodataka, JFrame okvir, Korisnik korisnik) {
        okvir.getContentPane().removeAll();
        okvir.setSize(1200, 800);
        okvir.setLocationRelativeTo(null);
        okvir.setLayout(new BorderLayout());
        okvir.getContentPane().setBackground(new Color(250, 206, 27));

        // Naslov
        JLabel naslov = new JLabel("Brisanje Vozila", JLabel.CENTER);
        naslov.setFont(new Font("Arial", Font.BOLD, 30));
        naslov.setBorder(BorderFactory.createEmptyBorder(20, 0, 20, 0));
        okvir.add(naslov, BorderLayout.NORTH);

        // Panel za unos ID-a vozila
        JPanel unosPanel = new JPanel();
        unosPanel.setBackground(new Color(250, 206, 27));
        JLabel idLabel = new JLabel("Unesite ID vozila za brisanje: ");
        idLabel.setFont(new Font("Arial", Font.PLAIN, 20));
        JTextField idField = new JTextField(10);
        idField.setFont(new Font("Arial", Font.PLAIN, 20));
        JButton pretraziButton = new JButton("Prikazi Vozila");
        pretraziButton.setFont(new Font("Arial", Font.BOLD, 20));
        pretraziButton.setPreferredSize(new Dimension(200, 50));
        JButton obrisiButton = new JButton("Obrisi Izabrano");
        obrisiButton.setFont(new Font("Arial", Font.BOLD, 20));
        obrisiButton.setPreferredSize(new Dimension(200, 50));

        unosPanel.add(idLabel);
        unosPanel.add(idField);
        unosPanel.add(pretraziButton);
        unosPanel.add(obrisiButton);
        okvir.add(unosPanel, BorderLayout.NORTH);

        // Dugme za povratak
        JButton povratakButton = new JButton("Povratak");
        povratakButton.setFont(new Font("Arial", Font.BOLD, 20));
        povratakButton.setPreferredSize(new Dimension(150, 50));
        JPanel dugmadPanel = new JPanel();
        dugmadPanel.setBackground(new Color(250, 206, 27));
        dugmadPanel.add(povratakButton);
        okvir.add(dugmadPanel, BorderLayout.SOUTH);

        // Kreiranje modela tabele i tabele
        tableModel = new DefaultTableModel();
        tableModel.addColumn("ID");
        tableModel.addColumn("Brend");
        tableModel.addColumn("Model");
        tableModel.addColumn("Boja");
        tableModel.addColumn("Godina");
        tableModel.addColumn("Cena");

        tabela = new JTable(tableModel);
        tabela.setFont(new Font("Arial", Font.PLAIN, 16));
        tabela.setRowHeight(30);
        tabela.setSelectionMode(javax.swing.ListSelectionModel.SINGLE_SELECTION);
        JScrollPane scrollPane = new JScrollPane(tabela);
        scrollPane.setPreferredSize(new Dimension(800, 300));

        JPanel tabelaPanel = new JPanel();
        tabelaPanel.add(scrollPane);
        okvir.add(tabelaPanel, BorderLayout.CENTER);

        // Popunjavanje tabele podacima iz baze
        popuniTabelu(bazaPodataka);

        // Akcija za prikaz vozila
        pretraziButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                popuniTabelu(bazaPodataka);
            }
        });

        // Akcija za brisanje izabranog vozila
        obrisiButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                int selectedRow = tabela.getSelectedRow();
                if (selectedRow != -1) {
                    int id = (int) tableModel.getValueAt(selectedRow, 0);
                    try {
                        String obrisi = "DELETE FROM kola WHERE ID = " + id + ";";
                        int rowsAffected = bazaPodataka.getStatement().executeUpdate(obrisi);
                        if (rowsAffected > 0) {
                            JOptionPane.showMessageDialog(okvir, "Vozilo je uspešno obrisano.", "Uspešno", JOptionPane.INFORMATION_MESSAGE);
                            // Ažuriranje tabele nakon brisanja vozila
                            popuniTabelu(bazaPodataka);
                        } else {
                            JOptionPane.showMessageDialog(okvir, "Vozilo sa izabranim ID-om ne postoji ili je već obrisano.", "Greška", JOptionPane.ERROR_MESSAGE);
                        }
                    } catch (SQLException ex) {
                        ex.printStackTrace();
                        JOptionPane.showMessageDialog(okvir, "Došlo je do greške prilikom brisanja vozila.", "Greška", JOptionPane.ERROR_MESSAGE);
                    }
                } else {
                    JOptionPane.showMessageDialog(okvir, "Molimo izaberite vozilo iz tabele koje želite da obrišete.", "Greška", JOptionPane.WARNING_MESSAGE);
                }
            }
        });

        // Akcija za povratak na prethodni ekran
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

    // Metoda za popunjavanje tabele podacima iz baze
    private void popuniTabelu(BazaPodataka bazaPodataka) {
        tableModel.setRowCount(0); // Očisti postojeće redove
        try {
            ResultSet rs = bazaPodataka.getStatement().executeQuery("SELECT ID, Brend, Model, Boja, Godina, Cena FROM kola WHERE Dostupnost < 2;");
            while (rs.next()) {
                int id = rs.getInt("ID");
                String brend = rs.getString("Brend");
                String model = rs.getString("Model");
                String boja = rs.getString("Boja");
                int godina = rs.getInt("Godina");
                double cena = rs.getDouble("Cena");

                tableModel.addRow(new Object[]{id, brend, model, boja, godina, cena + "€"});
            }
            rs.close();
        } catch (SQLException e) {
            e.printStackTrace();
        }
    }
}
