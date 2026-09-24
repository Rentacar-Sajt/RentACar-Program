<?php

// Repozitorijum za prijavu i administraciju korisnika.
require_once __DIR__ . '/BazniRepozitorijum.php';
require_once __DIR__ . '/../model/Korisnik.php';
class KorisnikRepozitorijum extends BazniRepozitorijum
{
    public function prijava(string $mejl,string $lozinka): ?array { $r=$this->izvrsiUpit('SELECT TOP 1 * FROM dbo.Korisnici WHERE Mejl=? AND Aktivan=1',[$mejl]); if(!$r||!password_verify($lozinka,$r[0]['Lozinka']))return null; return $r[0]; }
    public function dajSve(): array { return $this->izvrsiUpit('SELECT ID,Ime,Prezime,Mejl,Uloga,Aktivan FROM dbo.Korisnici ORDER BY Ime,Prezime'); }
    // Korisnik se takodje vraca kao objekat modela, a ne samo kao niz.
    public function dajPoId(int $id): ?Korisnik { $r=$this->izvrsiUpit('SELECT * FROM dbo.Korisnici WHERE ID=?',[$id]);if(!$r)return null;$k=$r[0];return new Korisnik((int)$k['ID'],(string)$k['Ime'],(string)$k['Prezime'],(string)$k['Mejl'],(string)$k['Lozinka'],(string)$k['Uloga'],(bool)$k['Aktivan']); }
    public function postojiMejl(string $mejl,?int $izuzmiId=null): bool { $sql='SELECT ID FROM dbo.Korisnici WHERE Mejl=?';$p=[$mejl];if($izuzmiId!==null){$sql.=' AND ID<>?';$p[]=$izuzmiId;}return (bool)$this->izvrsiUpit($sql,$p); }
    public function dodaj(string $ime,string $prezime,string $mejl,string $lozinka,string $uloga): int { $q=$this->konekcija->prepare('INSERT INTO dbo.Korisnici(Ime,Prezime,Mejl,Lozinka,Uloga,Aktivan) OUTPUT INSERTED.ID VALUES(?,?,?,?,?,1)');$q->execute([$ime,$prezime,$mejl,// Lozinku ne cuvam kao obican tekst.
        password_hash($lozinka,PASSWORD_DEFAULT),$uloga]);return (int)$q->fetchColumn(); }
    public function izmeni(int $id,string $ime,string $prezime,string $mejl,string $uloga,bool $aktivan): void { $this->izvrsiKomandu('UPDATE dbo.Korisnici SET Ime=?,Prezime=?,Mejl=?,Uloga=?,Aktivan=? WHERE ID=?',[$ime,$prezime,$mejl,$uloga,$aktivan?1:0,$id]); }
    public function promeniLozinku(int $id,string $nova): void { $this->izvrsiKomandu('UPDATE dbo.Korisnici SET Lozinka=? WHERE ID=?',[password_hash($nova,PASSWORD_DEFAULT),$id]); }
    public function proveriLozinku(int $id,string $lozinka): bool { $k=$this->dajPoId($id);return $k?password_verify($lozinka,$k['Lozinka']):false; }
}
