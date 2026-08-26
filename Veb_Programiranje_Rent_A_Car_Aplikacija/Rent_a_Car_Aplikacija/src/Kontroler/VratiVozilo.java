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
import java.time.format.DateTimeFormatter;
import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JTable;
import javax.swing.JTextField;
import javax.swing.table.DefaultTableModel;

import Model.Automobili;
import Model.BazaPodataka;
import Model.Iznajmljivanje;
import Model.Klijent;
import Model.Korisnik;

public class VratiVozilo {

    public void operacije(BazaPodataka bazapodataka, JFrame okvir, Korisnik korisnik) {
        // Postavljanje osnovnog okvira
        okvir.getContentPane().removeAll();
        okvir.setSize(1200, 800);
        okvir.setLocationRelativeTo(null);
        okvir.setLayout(new BorderLayout());

        // Naslov
        JLabel naslov = new JLabel("Vrati Vozilo", JLabel.CENTER);
        naslov.setFont(new Font("Arial", Font.BOLD, 30));
        okvir.add(naslov, BorderLayout.NORTH);

        // Panel za unos ID-a iznajmljivanja
        JPanel unosPanel = new JPanel();
        unosPanel.setLayout(new BorderLayout());
        unosPanel.setBackground(Color.LIGHT_GRAY);

        JLabel labelID = new JLabel("Unesite ID iznajmljivanja: ");
        labelID.setFont(new Font("Arial", Font.PLAIN, 20));
        JTextField textID = new JTextField();
        textID.setFont(new Font("Arial", Font.PLAIN, 20));
        textID.setPreferredSize(new Dimension(200, 40));

        JButton prikazButton = new JButton("Prikaži Sva Iznajmljivanja");
        prikazButton.setFont(new Font("Arial", Font.PLAIN, 20));
        prikazButton.setPreferredSize(new Dimension(250, 50));

        unosPanel.add(labelID, BorderLayout.WEST);
        unosPanel.add(textID, BorderLayout.CENTER);
        unosPanel.add(prikazButton, BorderLayout.EAST);

        okvir.add(unosPanel, BorderLayout.NORTH);

        // Tabela za prikaz svih iznajmljivanja
        String[] kolone = { "ID", "Ime", "Mejl", "Broj telefona", "ID Vozila", "Automobil", "Datum", "Sati", "Ukupno", "Status" };
        DefaultTableModel model = new DefaultTableModel(kolone, 0);
        JTable tabela = new JTable(model);
        tabela.setFont(new Font("Arial", Font.PLAIN, 18));
        tabela.setRowHeight(30);

        JScrollPane scrollPane = new JScrollPane(tabela);
        okvir.add(scrollPane, BorderLayout.CENTER);

        prikazButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                try {
                    // Log the user ID being used in the query
                    System.out.println("Korisnik ID: " + korisnik.getID());

                    // Construct the SQL query to select rentals by the user
                    String select = "SELECT * FROM iznajmljivanjekola WHERE korisnik = '" + korisnik.getID() + "';";
                    ResultSet rs = bazapodataka.getStatement().executeQuery(select);

                    // Clear the table before adding new data
                    model.setRowCount(0);

                    // Variable to count the number of rows retrieved
                    int rowCount = 0;

                    // Iterate through the result set
                    while (rs.next()) {
                        rowCount++;
                        System.out.println("Fetching record for rental ID: " + rs.getInt("ID")); // Log rental ID

                        Iznajmljivanje<Automobili> i = new Iznajmljivanje<>();
                        i.setID(rs.getInt("ID"));
                        int autoID = rs.getInt("automobil");

                        // Debugging date field
                        String datumString = rs.getString("datum");
                        System.out.println("Datum iz baze: " + datumString);

                        // Parse the date and time from the database string
                        LocalDateTime datumIVreme = LocalDateTime.parse(datumString, DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss"));
                        i.setDatum(datumIVreme);

                        i.setSati(rs.getInt("sati"));
                        i.setUkupno(rs.getDouble("ukupno"));
                        i.setStatus(rs.getInt("status"));
                        System.out.println("Status iznajmljivanja: " + rs.getInt("status"));

                        // Fetch vehicle details from the kola table
                        ResultSet rs3 = bazapodataka.getStatement().executeQuery("SELECT * FROM kola WHERE ID = '" + autoID + "';");
                        if (rs3.next()) {
                            Automobili automobili = new Automobili();
                            automobili.setID(rs3.getInt("ID"));
                            automobili.setBrend(rs3.getString("Brend"));
                            automobili.setModel(rs3.getString("Model"));
                            automobili.setBoja(rs3.getString("Boja"));
                            automobili.setGodina(rs3.getInt("Godina"));
                            automobili.setCena(rs3.getDouble("Cena"));
                            automobili.setDostupnost(rs3.getInt("Dostupnost"));
                            i.setAutomobil(automobili);

                            // Format the date and time for display
                            String datumFormatiran = i.getDatum().formatted(DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm"));

                            // Create a row to add to the table
                            Object[] red = {
                                i.getID(),
                                korisnik.getIme() + " " + korisnik.getPrezime(),
                                korisnik.getMejl(),
                                korisnik.getBrojTelefona(),
                                automobili.getID(),
                                automobili.getBrend() + " " + automobili.getModel() + " " + automobili.getBoja(),
                                datumFormatiran,
                                i.getSati(),
                                i.getUkupno(),
                                i.getStatustoString()
                            };
                            model.addRow(red);
                        } else {
                            System.out.println("Nema vozila sa ID-om: " + autoID); // Log if the vehicle is not found
                        }
                    }

                    // Log the total number of rows retrieved
                    System.out.println("Number of rows fetched: " + rowCount);

                    // Check if no records were found
                    if (rowCount == 0) {
                        System.out.println("No rental records found for this user.");
                    }

                } catch (SQLException ex) {
                    ex.printStackTrace();
                }
            }
        });





        // Dugme za vraćanje vozila
        JPanel vratiPanel = new JPanel();
        vratiPanel.setLayout(new BorderLayout());

        JButton vratiButton = new JButton("Vrati Vozilo");
        vratiButton.setFont(new Font("Arial", Font.PLAIN, 20));
        vratiButton.setPreferredSize(new Dimension(200, 50));
        vratiPanel.add(vratiButton, BorderLayout.CENTER);

        // Dugme za povratak na klijentov meni
        JButton povratakButton = new JButton("Povratak na Meni");
        povratakButton.setFont(new Font("Arial", Font.PLAIN, 20));
        povratakButton.setPreferredSize(new Dimension(200, 50));
        vratiPanel.add(povratakButton, BorderLayout.SOUTH);

        okvir.add(vratiPanel, BorderLayout.SOUTH);

        vratiButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                try {
                    int ID = Integer.parseInt(textID.getText());
                    String select = "SELECT * FROM iznajmljivanjekola WHERE ID = '" + ID + "';";
                    ResultSet rs = bazapodataka.getStatement().executeQuery(select);

                    if (rs.next()) {
                        int autoID = rs.getInt("Automobil");

                        // Update rental status
                        String updateRental = "UPDATE iznajmljivanjekola SET Status='1' WHERE ID = '" + ID + "';";
                        bazapodataka.getStatement().execute(updateRental);

                        // Update vehicle availability status
                        String updateVehicle = "UPDATE kola SET Dostupnost='0' WHERE ID = '" + autoID + "';";
                        bazapodataka.getStatement().execute(updateVehicle);

                        JOptionPane.showMessageDialog(okvir, "Uspešno ste vratili automobil.", "Informacija", JOptionPane.INFORMATION_MESSAGE);

                        // Osvježavanje tabele nakon vraćanja vozila
                        model.setRowCount(0); // Čišćenje modela tabele
                        prikazButton.doClick(); // Ponovno učitavanje podataka
                    } else {
                        JOptionPane.showMessageDialog(okvir, "Nema iznajmljivanja sa zadatim ID-om.", "Greška", JOptionPane.ERROR_MESSAGE);
                    }
                } catch (SQLException ex) {
                    ex.printStackTrace();
                } catch (NumberFormatException ex) {
                    JOptionPane.showMessageDialog(okvir, "Molimo unesite ispravan ID.", "Greška", JOptionPane.ERROR_MESSAGE);
                }
            }
        });

        povratakButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                okvir.dispose(); // Zatvaranje trenutnog prozora

                // Prikaz klijent menija
                JFrame klijentMeniOkvir = new JFrame("Klijent Meni");
                klijentMeniOkvir.setSize(1200, 800); // Veličina klijent menija
                klijentMeniOkvir.setLocationRelativeTo(null);
                klijentMeniOkvir.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
                klijentMeniOkvir.getContentPane().setBackground(new Color(250, 206, 27)); // Postavljanje pozadine na istu boju

                Login_Prijava.prikaziKlijentMeni(bazapodataka, klijentMeniOkvir, (Klijent) korisnik);
                klijentMeniOkvir.setVisible(true);
            }
        });

        // Osvježavanje GUI-a
        okvir.revalidate();
        okvir.repaint();
    }
}
