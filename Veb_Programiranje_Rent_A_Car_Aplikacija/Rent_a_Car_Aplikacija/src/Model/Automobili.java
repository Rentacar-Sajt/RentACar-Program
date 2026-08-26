package Model;

public class Automobili {
  private int ID;
  private String brend;
  private String model;
  private String boja;
  private int godina;
  private double cena;
  private int dostupnost;
  
  // 0 -> Dostupan
  // 1 -> Iznajmljen
  // 2 -> Obrisan
  
  public Automobili() {}
  
  public int getID() {
	return ID;
  }
  public void setID(int ID) {
	  this.ID = ID;
  }
  public String getBrend() {
	  return brend;
  }
  public void setBrend(String brend) {
	  this.brend = brend;
  }
  public String getModel() {
	  return model;
  }
  public void setModel(String model) {
	  this.model = model;
  }
  public String getBoja() {
	  return boja;
  }
  public void setBoja(String boja) {
	  this.boja = boja;
  }
  public int getGodina() {
	  return godina;
  }
  public void setGodina(int godina) {
	  this.godina = godina;
  }
  public double getCena() {
	  return cena;
  }
  public void setCena(double cena) {
	  this.cena = cena;
  }
  public int isDostupnost() {
	  return dostupnost;
  }
  public void setDostupnost(int dostupnost) {
	  this.dostupnost = dostupnost;
  }
  
	  
  
}
