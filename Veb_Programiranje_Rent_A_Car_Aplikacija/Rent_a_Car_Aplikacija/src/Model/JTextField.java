package Model;

import java.awt.Font;

@SuppressWarnings("serial")
public class JTextField extends javax.swing.JTextField{
	
	public JTextField(int velicinaTexta) {
		super();
		setFont(new Font("SansSerif", Font.BOLD, velicinaTexta));
		setHorizontalAlignment(JLabel.CENTER);
		setBorder(null);
	}

}
