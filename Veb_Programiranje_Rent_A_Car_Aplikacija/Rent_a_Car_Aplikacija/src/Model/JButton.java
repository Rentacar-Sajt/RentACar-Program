package Model;

import java.awt.Color;
import java.awt.Font;

@SuppressWarnings("serial")
public class JButton extends javax.swing.JButton {
	
	public JButton(String text, int velicinaTexta) {
		super(text);
		setBackground(Color.black);
		setFont(new Font("SansSerif", Font.BOLD, velicinaTexta));
		setForeground(Color.white);
		setBorder(null);
	}

}
