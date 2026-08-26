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

import javax.swing.BorderFactory;
import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JTable;
import javax.swing.JTextField;
import javax.swing.SwingConstants;
import javax.swing.table.DefaultTableModel;

import Model.Admin;
import Model.Automobili;
import Model.BazaPodataka;
import Model.Korisnik;
import Model.Operacije;

public class AzuriranjeVozila implements Operacije {
    private DefaultTableModel model;
    private BazaPodataka bazaPodataka;
    private JFrame okvir;

    public void operacije(BazaPodataka bazaPodataka, JFrame okvir, Korisnik korisnik) {
        this.bazaPodataka = bazaPodataka;
        this.okvir = okvir;
        
        okvir.getContentPane().removeAll();
        okvir.setSize(1200, 800);
        okvir.setLocationRelativeTo(null);
        okvir.setLayout(new BorderLayout());
        okvir.getContentPane().setBackground(new Color(250, 206, 27));

        // Kreiranje naslova
        JLabel naslov = new JLabel("Ažuriranje Vozila", JLabel.CENTER);
        naslov.setFont(new Font("Arial", Font.BOLD, 30));
        naslov.setBorder(BorderFactory.createEmptyBorder(20, 0, 20, 0));
        okvir.add(naslov, BorderLayout.NORTH);

        // Panel za unos ID-a vozila
        JPanel unosPanel = new JPanel();
        unosPanel.setBackground(new Color(250, 206, 27));
        JLabel idLabel = new JLabel("Unesite ID vozila: ");
        idLabel.setFont(new Font("Arial", Font.PLAIN, 20));
        JTextField idField = new JTextField(10);
        idField.setFont(new Font("Arial", Font.PLAIN, 20));
        JButton pretraziButton = new JButton("Pretraži");
        pretraziButton.setFont(new Font("Arial", Font.BOLD, 20));
        pretraziButton.setPreferredSize(new Dimension(150, 50));

        unosPanel.add(idLabel);
        unosPanel.add(idField);
        unosPanel.add(pretraziButton);
        okvir.add(unosPanel, BorderLayout.NORTH);

        // Panel za unos podataka vozila
        JPanel podaciPanel = new JPanel();
        podaciPanel.setLayout(new BorderLayout());
        podaciPanel.setBackground(new Color(250, 206, 27));
        JLabel porukaLabel = new JLabel("Nabavite podatke vozila iz baze da biste ih ažurirali.", SwingConstants.CENTER);
        porukaLabel.setFont(new Font("Arial", Font.PLAIN, 20));
        podaciPanel.add(porukaLabel, BorderLayout.NORTH);

        // Polja za unos
        JTextField brendField = new JTextField(20);
        JTextField modelField = new JTextField(20);
        JTextField bojaField = new JTextField(20);
        JTextField godinaField = new JTextField(20);
        JTextField cenaField = new JTextField(20);

        // Postavljanje fonta za polja za unos
        Font font = new Font("Arial", Font.PLAIN, 20);
        brendField.setFont(font);
        modelField.setFont(font);
        bojaField.setFont(font);
        godinaField.setFont(font);
        cenaField.setFont(font);

        JPanel formPanel = new JPanel();
        formPanel.setLayout(new GridLayout(5, 2, 10, 10));
        formPanel.add(new JLabel("Brend:", SwingConstants.RIGHT));
        formPanel.add(brendField);
        formPanel.add(new JLabel("Model:", SwingConstants.RIGHT));
        formPanel.add(modelField);
        formPanel.add(new JLabel("Boja:", SwingConstants.RIGHT));
        formPanel.add(bojaField);
        formPanel.add(new JLabel("Godina:", SwingConstants.RIGHT));
        formPanel.add(godinaField);
        formPanel.add(new JLabel("Cena:", SwingConstants.RIGHT));
        formPanel.add(cenaField);
        podaciPanel.add(formPanel, BorderLayout.CENTER);

        // Panel za dugmad
        JPanel dugmadPanel = new JPanel();
        dugmadPanel.setBackground(new Color(250, 206, 27));
        JButton azurirajButton = new JButton("Ažuriraj");
        azurirajButton.setFont(new Font("Arial", Font.BOLD, 20));
        azurirajButton.setPreferredSize(new Dimension(150, 50));
        JButton povratakButton = new JButton("Povratak");
        povratakButton.setFont(new Font("Arial", Font.BOLD, 20));
        povratakButton.setPreferredSize(new Dimension(150, 50));
        dugmadPanel.add(azurirajButton);
        dugmadPanel.add(povratakButton);
        podaciPanel.add(dugmadPanel, BorderLayout.SOUTH);

        okvir.add(podaciPanel, BorderLayout.CENTER);
        
        // Kreiranje modela tabele
        model = new DefaultTableModel();
        model.addColumn("ID");
        model.addColumn("Brend");
        model.addColumn("Model");
        model.addColumn("Boja");
        model.addColumn("Godina");
        model.addColumn("Cena");

        // Kreiranje tabele i dodavanje u JScrollPane
        JTable tabela = new JTable(model);
        tabela.setFont(new Font("Arial", Font.PLAIN, 16));
        tabela.setRowHeight(30);
        JScrollPane scrollPane = new JScrollPane(tabela);
        scrollPane.setPreferredSize(new Dimension(800, 300));

        JPanel tabelaPanel = new JPanel();
        tabelaPanel.add(scrollPane);
        okvir.add(tabelaPanel, BorderLayout.SOUTH);

        // Osveži tabelu sa početnim podacima
        osveziTabelu();

        pretraziButton.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                String idText = idField.getText();
                try {
                    int id = Integer.parseInt(idText);
                    ResultSet rs = bazaPodataka.getStatement().executeQuery("SELECT * FROM kola WHERE ID = " + id + ";");
                    if (rs.next()) {
                        Automobili automobili = new Automobili();
                        automobili.setID(rs.getInt("ID"));
                        automobili.setBrend(rs.getString("Brend"));
                        automobili.setModel(rs.getString("Model"));
                        automobili.setBoja(rs.getString("Boja"));
                        automobili.setGodina(rs.getInt("Godina"));
                        automobili.setCena(rs.getDouble("Cena"));
                        automobili.setDostupnost(rs.getInt("Dostupnost"));

                        if (automobili.isDostupnost() > 1) {
                            JOptionPane.showMessageDialog(okvir, "Vozilo ne postoji!", "Greška", JOptionPane.ERROR_MESSAGE);
                            return;
                        }

                        brendField.setText(automobili.getBrend());
                        modelField.setText(automobili.getModel());
                        bojaField.setText(automobili.getBoja());
                        godinaField.setText(String.valueOf(automobili.getGodina()));
                        cenaField.setText(String.valueOf(automobili.getCena()));
                        porukaLabel.setText("Unesite nove podatke za vozilo sa ID-om: " + id);
                    } else {
                        JOptionPane.showMessageDialog(okvir, "Vozilo sa unetim ID-om ne postoji.", "Greška", JOptionPane.ERROR_MESSAGE);
                    }
                    rs.close();
                } catch (SQLException ex) {
                    ex.printStackTrace();
                    JOptionPane.showMessageDialog(okvir, "Došlo je do greške prilikom pretrage vozila.", "Greška", JOptionPane.ERROR_MESSAGE);
                }
            }
        });

        azurirajButton.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                String brend = brendField.getText();
                String model = modelField.getText();
                String boja = bojaField.getText();
                String godinaText = godinaField.getText();
                String cenaText = cenaField.getText();
                String idText = idField.getText();

                if (brend.isEmpty() || model.isEmpty() || boja.isEmpty() || godinaText.isEmpty() || cenaText.isEmpty() || idText.isEmpty()) {
                    JOptionPane.showMessageDialog(okvir, "Sva polja su obavezna.", "Greška", JOptionPane.ERROR_MESSAGE);
                    return;
                }

                int godina;
                double cena;
                int id;

                try {
                    godina = Integer.parseInt(godinaText);
                    cena = Double.parseDouble(cenaText);
                    id = Integer.parseInt(idText);

                    // Ažuriranje podataka u bazi
                    String updateQuery = "UPDATE kola SET Brend = ?, Model = ?, Boja = ?, Godina = ?, Cena = ? WHERE ID = ?";
                    try (var preparedStatement = bazaPodataka.getConnection().prepareStatement(updateQuery)) {
                        preparedStatement.setString(1, brend);
                        preparedStatement.setString(2, model);
                        preparedStatement.setString(3, boja);
                        preparedStatement.setInt(4, godina);
                        preparedStatement.setDouble(5, cena);
                        preparedStatement.setInt(6, id);

                        int rowsUpdated = preparedStatement.executeUpdate();

                        if (rowsUpdated > 0) {
                            JOptionPane.showMessageDialog(okvir, "Podaci o vozilu su uspešno ažurirani.", "Uspešno", JOptionPane.INFORMATION_MESSAGE);
                            osveziTabelu(); // Osveži tabelu nakon ažuriranja
                        } else {
                            JOptionPane.showMessageDialog(okvir, "Došlo je do greške prilikom ažuriranja vozila.", "Greška", JOptionPane.ERROR_MESSAGE);
                        }
                    } catch (NumberFormatException ex) {
                        JOptionPane.showMessageDialog(okvir, "Unesite validne brojeve za godinu i cenu.", "Greška", JOptionPane.ERROR_MESSAGE);
                    } catch (SQLException ex) {
                        ex.printStackTrace();
                        JOptionPane.showMessageDialog(okvir, "Došlo je do greške prilikom ažuriranja vozila.", "Greška", JOptionPane.ERROR_MESSAGE);
                    }
                } catch (NumberFormatException ex) {
                    JOptionPane.showMessageDialog(okvir, "Unesite validne brojeve za godinu i cenu.", "Greška", JOptionPane.ERROR_MESSAGE);
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

        okvir.revalidate();
        okvir.repaint();
    }

    // Metoda za osvežavanje tabele
    private void osveziTabelu() {
        model.setRowCount(0); // Očisti postojeće redove
        try {
            String query = "SELECT ID, Brend, Model, Boja, Godina, Cena FROM kola WHERE Dostupnost = 0;";
            
            // Kreiranje Statement objekta
            var statement = bazaPodataka.getConnection().createStatement();
            ResultSet rs = statement.executeQuery(query);

            while (rs.next()) {
                int id = rs.getInt("ID");
                String brend = rs.getString("Brend");
                String modelStr = rs.getString("Model");
                String boja = rs.getString("Boja");
                int godina = rs.getInt("Godina");
                double cena = rs.getDouble("Cena");

                model.addRow(new Object[]{id, brend, modelStr, boja, godina, cena + "€"});
            }
            rs.close();
        } catch (SQLException e) {
            e.printStackTrace();
        }
    }
}
