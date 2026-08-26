package Model;
import java.awt.Font;
@SuppressWarnings("serial")
public class JLabel extends javax.swing.JLabel {
	
	public JLabel(String text, int velicinaFonta) {
		super(text);
		setFont(new Font("SansSerif", Font.BOLD, velicinaFonta));
		setBackground(null);
		setHorizontalAlignment(JLabel.CENTER);
		
	}

}
