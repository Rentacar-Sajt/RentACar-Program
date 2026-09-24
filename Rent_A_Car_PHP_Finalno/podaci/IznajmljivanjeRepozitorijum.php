<?php

// Glavni repozitorijum za iznajmljivanja i njihove stavke.
require_once __DIR__ . '/BazniRepozitorijum.php';
require_once __DIR__ . '/../model/Iznajmljivanje.php';
require_once __DIR__ . '/../model/Klijent.php';
require_once __DIR__ . '/../model/StavkaIznajmljivanja.php';
require_once __DIR__ . '/../model/Vozilo.php';
require_once __DIR__ . '/KlijentRepozitorijum.php';
require_once __DIR__ . '/VoziloRepozitorijum.php';
class IznajmljivanjeRepozitorijum extends BazniRepozitorijum
{
 public function dajSve(string $filter='',array $f=[]):array{ $sql='SELECT p.* FROM dbo.PregledIznajmljivanja p WHERE 1=1';$p=[];if($filter!==''){$q="%$filter%";$sql.=" AND (Ime+' '+Prezime LIKE ? OR Mejl LIKE ? OR Status LIKE ? OR CAST(ID AS NVARCHAR(20)) LIKE ?)";array_push($p,$q,$q,$q,$q);}if(!empty($f['status'])&&in_array($f['status'],['Aktivno','Zavrseno'],true)){$sql.=' AND Status=?';$p[]=$f['status'];}if(!empty($f['klijentId'])){$sql.=' AND KlijentID=?';$p[]=(int)$f['klijentId'];}if(!empty($f['datumOd'])){$sql.=' AND DatumOd>=?';$p[]=$f['datumOd'];}if(!empty($f['datumDo'])){$sql.=' AND DatumDo<=?';$p[]=$f['datumDo'];}if(!empty($f['voziloId'])){$sql.=' AND EXISTS(SELECT 1 FROM dbo.StavkeIznajmljivanja s WHERE s.IznajmljivanjeID=p.ID AND s.VoziloID=?)';$p[]=(int)$f['voziloId'];}if(isset($f['minCena'])&&$f['minCena']!==''){$sql.=' AND UkupnaCena>=?';$p[]=(float)$f['minCena'];}if(isset($f['maxCena'])&&$f['maxCena']!==''){$sql.=' AND UkupnaCena<=?';$p[]=(float)$f['maxCena'];}return $this->izvrsiUpit($sql.' ORDER BY ID DESC',$p); }
 // Ucitavam ceo objekat iznajmljivanja zajedno sa klijentom i stavkama.
 public function dajPoId(int $id):?Iznajmljivanje{ $g=$this->izvrsiUpit('SELECT i.*,k.Ime,k.Prezime,k.Mejl,k.Telefon,k.BrojDokumenta FROM dbo.Iznajmljivanja i JOIN dbo.Klijenti k ON k.ID=i.KlijentID WHERE i.ID=?',[$id]);if(!$g)return null;$x=$g[0];// Asocijacija: jedno iznajmljivanje ima svog Klijent objekat.
// ASOCIJACIJA: iz podataka iz baze pravim Klijent objekat i povezujem ga sa iznajmljivanjem.
$klijent=new Klijent((int)$x['KlijentID'],(string)$x['Ime'],(string)$x['Prezime'],(string)$x['Mejl'],(string)$x['Telefon'],(string)$x['BrojDokumenta']);// Pravimo glavni Iznajmljivanje objekat.
// Ovde se vidi asocijacija: Iznajmljivanje dobija Klijent objekat.
$obj=new Iznajmljivanje((int)$x['ID'],$klijent,(string)$x['DatumOd'],(string)$x['DatumDo'],(string)$x['Status'],(float)$x['UkupnaCena']);$stavke=$this->izvrsiUpit('SELECT s.*,v.Marka,v.Model,v.Registracija,v.Godiste,v.Gorivo,v.Menjac,v.BrojSedista,v.Kilometraza,v.Kategorija,v.CenaPoDanu,v.Status FROM dbo.StavkeIznajmljivanja s JOIN dbo.Vozila v ON v.ID=s.VoziloID WHERE s.IznajmljivanjeID=? ORDER BY s.ID',[$id]);// KOMPOZICIJA: ucitavam sve stavke koje pripadaju ovom iznajmljivanju.
foreach($stavke as $s){$v=new Vozilo((int)$s['VoziloID'],(string)$s['Marka'],(string)$s['Model'],(string)$s['Registracija'],(int)$s['Godiste'],(string)$s['Gorivo'],(string)$s['Menjac'],(int)$s['BrojSedista'],(int)$s['Kilometraza'],(string)$s['Kategorija'],(float)$s['CenaPoDanu'],(string)$s['Status']);// Kompozicija: svaku stavku dodajem tom iznajmljivanju.
// Ovde stvarno dodajem StavkaIznajmljivanja objekat u iznajmljivanje - to je kompozicija.
$obj->dodajStavku(new StavkaIznajmljivanja((int)$s['ID'],$v,(int)$s['BrojDana'],(float)$s['CenaPoDanu'],(float)$s['Iznos']));}return $obj; }
 // Ocekivani ulaz je sada OOP objekat, a ne samo lista ID-jeva.
 public function dodaj(Iznajmljivanje $iznajmljivanje,string $korisnik=''):int{ if(!$iznajmljivanje->stavke)throw new Exception('Mora biti dodato najmanje jedno vozilo.');// Sve promene radim u jednoj transakciji da baza ne ostane napola izmenjena.
$this->konekcija->beginTransaction();try{$q=$this->konekcija->prepare("INSERT INTO dbo.Iznajmljivanja(KlijentID,DatumOd,DatumDo,Status,UkupnaCena,KreiraoKorisnik) OUTPUT INSERTED.ID VALUES(?,?,?,N'Aktivno',0,?)");$q->execute([$iznajmljivanje->klijent->id,$iznajmljivanje->datumOd,$iznajmljivanje->datumDo,$korisnik]);$id=(int)$q->fetchColumn();$u=0;// Svaka stavka predstavlja jedno vozilo koje se dodaje u iznajmljivanje.
foreach($iznajmljivanje->stavke as $stavka){$vid=$stavka->vozilo->id;$q=$this->konekcija->prepare('SELECT * FROM dbo.Vozila WITH(UPDLOCK,ROWLOCK) WHERE ID=?');$q->execute([$vid]);$v=$q->fetch(PDO::FETCH_ASSOC);if(!$v||$v['Status']!=='Slobodno')throw new Exception('Jedno od vozila više nije slobodno.');$d=$iznajmljivanje->brojDana();$iz=$d*(float)$v['CenaPoDanu'];$this->izvrsiKomandu('INSERT INTO dbo.StavkeIznajmljivanja(IznajmljivanjeID,VoziloID,BrojDana,CenaPoDanu,Iznos) VALUES(?,?,?,?,?)',[$id,$vid,$d,$v['CenaPoDanu'],$iz]);$this->izvrsiKomandu("UPDATE dbo.Vozila SET Status=N'Iznajmljeno' WHERE ID=?",[$vid]);$u+=$iz;}$this->izvrsiKomandu('UPDATE dbo.Iznajmljivanja SET UkupnaCena=? WHERE ID=?',[$u,$id]);$this->konekcija->commit();return $id;}catch(Throwable $e){if($this->konekcija->inTransaction())$this->konekcija->rollBack();throw $e;} }
 public function izmeni(Iznajmljivanje $iznajmljivanje,string $korisnik=''):void{ $id=(int)$iznajmljivanje->id;$z=$this->dajPoId($id);if(!$z||$z->status==='Zavrseno')throw new Exception('Završeno iznajmljivanje se ne može menjati.');if(!$iznajmljivanje->stavke)throw new Exception('Mora biti dodato najmanje jedno vozilo.');$this->konekcija->beginTransaction();try{$st=$this->izvrsiUpit('SELECT VoziloID FROM dbo.StavkeIznajmljivanja WHERE IznajmljivanjeID=?',[$id]);foreach($st as $s)$this->izvrsiKomandu("UPDATE dbo.Vozila SET Status=N'Slobodno' WHERE ID=?",[$s['VoziloID']]);$this->izvrsiKomandu('DELETE FROM dbo.StavkeIznajmljivanja WHERE IznajmljivanjeID=?',[$id]);$this->izvrsiKomandu('UPDATE dbo.Iznajmljivanja SET KlijentID=?,DatumOd=?,DatumDo=?,UkupnaCena=0,IzmenioKorisnik=? WHERE ID=?',[$iznajmljivanje->klijent->id,$iznajmljivanje->datumOd,$iznajmljivanje->datumDo,$korisnik,$id]);$u=0;foreach($iznajmljivanje->stavke as $stavka){$vid=$stavka->vozilo->id;$q=$this->konekcija->prepare('SELECT * FROM dbo.Vozila WITH(UPDLOCK,ROWLOCK) WHERE ID=?');$q->execute([$vid]);$v=$q->fetch(PDO::FETCH_ASSOC);if(!$v||$v['Status']!=='Slobodno')throw new Exception('Izabrano vozilo nije dostupno.');$d=$iznajmljivanje->brojDana();$iz=$d*(float)$v['CenaPoDanu'];$this->izvrsiKomandu('INSERT INTO dbo.StavkeIznajmljivanja(IznajmljivanjeID,VoziloID,BrojDana,CenaPoDanu,Iznos) VALUES(?,?,?,?,?)',[$id,$vid,$d,$v['CenaPoDanu'],$iz]);$this->izvrsiKomandu("UPDATE dbo.Vozila SET Status=N'Iznajmljeno' WHERE ID=?",[$vid]);$u+=$iz;}$this->izvrsiKomandu('UPDATE dbo.Iznajmljivanja SET UkupnaCena=? WHERE ID=?',[$u,$id]);$this->konekcija->commit();}catch(Throwable $e){if($this->konekcija->inTransaction())$this->konekcija->rollBack();throw $e;} }

 public function dodajLegacy(int $klijentId,string $od,string $do,array $vozila,string $korisnik=''): int
 {
   $k=(new KlijentRepozitorijum())->dajPoId($klijentId); if(!$k) throw new Exception('Klijent nije pronađen.');
   $vRepo=new VoziloRepozitorijum(); // Pravimo glavni Iznajmljivanje objekat.
// ASOCIJACIJA: iznajmljivanje dobija Klijent objekat.
$obj=new Iznajmljivanje(null,$k,$od,$do,'Aktivno');
   // KOMPOZICIJA: za svako izabrano vozilo pravim stavku i dodajem je iznajmljivanju.
   foreach($vozila as $vid){$v=$vRepo->dajPoId((int)$vid);if(!$v)throw new Exception('Vozilo nije pronađeno.');// Ovde stvarno dodajem StavkaIznajmljivanja objekat u iznajmljivanje - to je kompozicija.
$obj->dodajStavku(new StavkaIznajmljivanja(null,$v,$obj->brojDana(),$v->cenaPoDanu,$obj->brojDana()*$v->cenaPoDanu));}
   return $this->dodaj($obj,$korisnik);
 }
 public function izmeniLegacy(int $id,int $klijentId,string $od,string $do,array $vozila,string $korisnik=''): void
 {
   $k=(new KlijentRepozitorijum())->dajPoId($klijentId); if(!$k) throw new Exception('Klijent nije pronađen.');
   $vRepo=new VoziloRepozitorijum(); // Pravimo glavni Iznajmljivanje objekat.
// ASOCIJACIJA: izmenjeno iznajmljivanje je ponovo povezano sa Klijent objektom.
$obj=new Iznajmljivanje($id,$k,$od,$do,'Aktivno');
   // KOMPOZICIJA: za svako izabrano vozilo pravim stavku i dodajem je iznajmljivanju.
   foreach($vozila as $vid){$v=$vRepo->dajPoId((int)$vid);if(!$v)throw new Exception('Vozilo nije pronađeno.');// Ovde stvarno dodajem StavkaIznajmljivanja objekat u iznajmljivanje - to je kompozicija.
$obj->dodajStavku(new StavkaIznajmljivanja(null,$v,$obj->brojDana(),$v->cenaPoDanu,$obj->brojDana()*$v->cenaPoDanu));}
   $this->izmeni($obj,$korisnik);
 }
 public function zavrsi(int $id,string $datum,string $napomena=''):int{$z=$this->dajPoId($id);if(!$z)throw new Exception('Iznajmljivanje nije pronađeno.');if($z->status==='Zavrseno')throw new Exception('Iznajmljivanje je već završeno.');if(strtotime($datum)<strtotime($z->datumOd))throw new Exception('Datum vraćanja ne može biti pre datuma preuzimanja.');$kas=max(0,(int)((strtotime($datum)-strtotime($z->datumDo))/86400));$this->konekcija->beginTransaction();try{$this->izvrsiKomandu("UPDATE dbo.Iznajmljivanja SET Status=N'Zavrseno',DatumVracanja=?,KasnjenjeDana=?,NapomenaVracanja=? WHERE ID=?",[$datum,$kas,$napomena,$id]);$this->izvrsiKomandu("UPDATE v SET Status=N'Slobodno' FROM dbo.Vozila v JOIN dbo.StavkeIznajmljivanja s ON s.VoziloID=v.ID WHERE s.IznajmljivanjeID=? AND v.Status=N'Iznajmljeno'",[$id]);$this->konekcija->commit();return $kas;}catch(Throwable $e){if($this->konekcija->inTransaction())$this->konekcija->rollBack();throw $e;}}
 public function dajStatistiku():array{$r=$this->izvrsiUpit("SELECT COUNT(*) Ukupno,SUM(CASE WHEN Status=N'Aktivno' THEN 1 ELSE 0 END) Aktivna,SUM(CASE WHEN Status=N'Zavrseno' THEN 1 ELSE 0 END) Zavrsena FROM dbo.Iznajmljivanja");$s=$r[0]??[];return ['ukupno'=>(int)($s['Ukupno']??0),'aktivna'=>(int)($s['Aktivna']??0),'zavrsena'=>(int)($s['Zavrsena']??0)];}
 public function dajPoslednja(int $n=5):array{$n=max(1,min(10,$n));return $this->izvrsiUpit("SELECT TOP $n * FROM dbo.PregledIznajmljivanja ORDER BY ID DESC");}
 public function obrisi(int $id):void{$this->izvrsiProceduru('dbo.ObrisiIznajmljivanje',[$id]);}
}
