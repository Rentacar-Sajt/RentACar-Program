package Model;

import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.time.temporal.ChronoUnit;

public class Iznajmljivanje<Automobil> {

    private int ID;
    private Korisnik korisnik;
    private Automobil automobil; // Promenjeno ime atributa za doslednost
    private LocalDateTime datum;
    private int sati;
    private double ukupno;
    private int status;
    private DateTimeFormatter formater = DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm");
    
    // status -> 0 u upotrebi
    // status -> 1 vraceno

    public Iznajmljivanje() {
        setDatum(LocalDateTime.now()); // Postavljanje trenutnog datuma i vremena
    }

    public int getID() {
        return ID;
    }

    public void setID(int ID) {
        this.ID = ID;
    }

    public Korisnik getKorisnik() {
        return korisnik;
    }

    public void setKorisnik(Korisnik korisnik) {
        this.korisnik = korisnik;
    }

    public Automobil getAutomobil() { // Promenjeno ime metode za doslednost
        return automobil;
    }

    public void setAutomobil(Automobil automobil) {
        this.automobil = automobil;
    }

    public String getDatum() {
        return formater.format(datum); // Formatira datum kao string
    }
    
    public LocalDateTime getLocalDateTime() {
        return datum;
    }
      
    public void setDatum(LocalDateTime datum) {
        this.datum = datum; // Postavljanje direktno bez potrebe za parsiranjem
    }
    

    public int getSati() {
        return sati;
    }

    public void setSati(int sati) {
        this.sati = sati;
    }

    public double getUkupno() {
        return ukupno;
    }

    public void setUkupno(double ukupno) {
        this.ukupno = ukupno;
    }

    public int getStatus() {
        return status;
    }

    public void setStatus(int status) {
        this.status = status;
    }

    public String getStatustoString() {
        long prosloSati = ChronoUnit.HOURS.between(datum, LocalDateTime.now());
        String statusString = "";
        if (getStatus() != 1 && prosloSati < getSati()) {
            statusString = "Preostalo";
        } else if (getStatus() != 1 && prosloSati >= getSati()) {
            statusString = "Odlozeno";
        } else if (getStatus() == 1) {
            statusString = "Vozilo je vraceno";
        }
        return statusString; // Ispravljeno da vraća string
    }
    public long getOdlozeniSati() {
    	long prosloSati = ChronoUnit.HOURS.between(datum, LocalDateTime.now());
    	return prosloSati-sati;
    }
}
