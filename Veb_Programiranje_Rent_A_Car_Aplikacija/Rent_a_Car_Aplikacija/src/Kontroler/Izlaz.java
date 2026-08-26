package Kontroler;

import java.awt.Font;
import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import java.awt.BorderLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

import Model.BazaPodataka;
import Model.Korisnik;
import Model.Operacije;

public class Izlaz implements Operacije {

    @Override
    public void operacije(BazaPodataka bazaPodataka, JFrame okvir, Korisnik korisnik) {
        // Prikazivanje poruke zahvalnosti
        int odgovor = JOptionPane.showConfirmDialog(
                okvir,
                "Hvala što ste koristili aplikaciju. Da li želite da izađete?",
                "Izlaz",
                JOptionPane.YES_NO_OPTION,
                JOptionPane.QUESTION_MESSAGE
        );

        // Ako korisnik odabere "Da", zatvaramo aplikaciju
        if (odgovor == JOptionPane.YES_OPTION) {
            okvir.dispose();
            System.exit(0);  // Ova komanda zatvara ceo program
        }
    }
}
