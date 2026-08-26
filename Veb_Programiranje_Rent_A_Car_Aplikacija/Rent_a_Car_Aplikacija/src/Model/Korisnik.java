package Model;



import javax.swing.JFrame;

public abstract class Korisnik {
	
private int ID;
private String ime;
private String prezime;
private String mejl;
private String brojTelefona;
private String sifra;

public Korisnik() {}

public int getID() {
	return ID;
}

public void setID(int iD) {
	ID = iD;
}

public String getIme() {
	return ime;
}

public void setIme(String ime) {
	this.ime = ime;
}

public String getBrojTelefona() {
	return brojTelefona;
}

public void setBrojTelefona(String brojTelefona) {
	this.brojTelefona = brojTelefona;
}

public String getMejl() {
	return mejl;
}

public void setMejl(String mejl) {
	this.mejl = mejl;
}

public String getSifra() {
	return sifra;
}

public void setSifra(String sifra) {
	this.sifra = sifra;
}

public String getPrezime() {
	return prezime;
}

public void setPrezime(String prezime) {
	this.prezime = prezime;
}

public abstract void prikaziListu(BazaPodataka bazapodataka, JFrame f);






}
