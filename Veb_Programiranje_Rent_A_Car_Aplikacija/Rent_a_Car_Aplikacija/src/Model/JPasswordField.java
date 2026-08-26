package Model;

import java.awt.Font;

@SuppressWarnings("serial")
public class JPasswordField extends javax.swing.JPasswordField {
	
	public JPasswordField(int velicinaTexta) {
		setFont(new Font("SansSerif", Font.BOLD, velicinaTexta));
		setHorizontalAlignment(JLabel.CENTER);
		setBorder(null);
	}

}
