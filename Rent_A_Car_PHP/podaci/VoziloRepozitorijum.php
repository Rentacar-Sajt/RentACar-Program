<?php
require_once __DIR__ . '/BazniRepozitorijum.php';
class VoziloRepozitorijum extends BazniRepozitorijum
{
    public function dajSve(string $filter='',string $status=''): array { $sql='SELECT * FROM dbo.Vozila WHERE 1=1';$p=[];if($filter!==''){$f="%$filter%";$sql.=' AND (Marka LIKE ? OR Model LIKE ? OR Registracija LIKE ? OR Kategorija LIKE ?)';array_push($p,$f,$f,$f,$f);}if(in_array($status,['Slobodno','Iznajmljeno','Servis','Nedostupno'],true)){$sql.=' AND Status=?';$p[]=$status;}$sql.=' ORDER BY Marka,Model';return $this->izvrsiUpit($sql,$p); }
    public function dajSlobodna(): array { return $this->procitajPogled('SlobodnaVozila'); }
    public function dajStatistiku(): array { $r=$this->izvrsiUpit("SELECT COUNT(*) Ukupno,SUM(CASE WHEN Status=N'Slobodno' THEN 1 ELSE 0 END) Slobodna,SUM(CASE WHEN Status=N'Iznajmljeno' THEN 1 ELSE 0 END) Iznajmljena FROM dbo.Vozila");$s=$r[0]??[];return ['ukupno'=>(int)($s['Ukupno']??0),'slobodna'=>(int)($s['Slobodna']??0),'iznajmljena'=>(int)($s['Iznajmljena']??0)]; }
    public function dajPoId(int $id): ?array { $r=$this->izvrsiUpit('SELECT * FROM dbo.Vozila WHERE ID=?',[$id]);return $r[0]??null; }
    public function postojiRegistracija(string $r,?int $id=null): bool { $sql='SELECT ID FROM dbo.Vozila WHERE Registracija=?';$p=[$r];if($id!==null){$sql.=' AND ID<>?';$p[]=$id;}return (bool)$this->izvrsiUpit($sql,$p); }
    public function dodaj(array $p): int { $q=$this->konekcija->prepare('INSERT INTO dbo.Vozila(Marka,Model,Registracija,Godiste,Gorivo,Menjac,BrojSedista,Kilometraza,Kategorija,CenaPoDanu,Status) OUTPUT INSERTED.ID VALUES(?,?,?,?,?,?,?,?,?,?,?)');$q->execute([$p['marka'],$p['model'],$p['registracija'],$p['godiste'],$p['gorivo'],$p['menjac'],$p['brojSedista'],$p['kilometraza'],$p['kategorija'],$p['cenaPoDanu'],$p['status']]);return (int)$q->fetchColumn(); }
    public function izmeni(int $id,array $p): void { $this->izvrsiKomandu('UPDATE dbo.Vozila SET Marka=?,Model=?,Registracija=?,Godiste=?,Gorivo=?,Menjac=?,BrojSedista=?,Kilometraza=?,Kategorija=?,CenaPoDanu=?,Status=? WHERE ID=?',[$p['marka'],$p['model'],$p['registracija'],$p['godiste'],$p['gorivo'],$p['menjac'],$p['brojSedista'],$p['kilometraza'],$p['kategorija'],$p['cenaPoDanu'],$p['status'],$id]); }
    public function obrisi(int $id): void { $this->izvrsiKomandu('DELETE FROM dbo.Vozila WHERE ID=?',[$id]); }
}
