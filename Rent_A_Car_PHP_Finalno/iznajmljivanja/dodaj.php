<?php
// Ova stranica je deo korisnickog interfejsa i povezuje formu sa kontrolerom/repozitorijumom.
require_once __DIR__ . '/../servisi/Sesija.php'; Sesija::zahtevajPrijavu();
require_once __DIR__ . '/../podaci/VoziloRepozitorijum.php'; require_once __DIR__ . '/../podaci/IznajmljivanjeRepozitorijum.php'; require_once __DIR__ . '/../validacija/Validacija.php';
$vRepo=new VoziloRepozitorijum(); $vozila=$vRepo->dajSlobodna(); $greska='';
if($_SERVER['REQUEST_METHOD']==='POST'){
 $od=$_POST['datumOd']??''; $do=$_POST['datumDo']??''; $izabrana=$_POST['vozila']??[];
 $greska=Validacija::datumOpseg($od,$do)??''; if(!$greska && !$izabrana)$greska='Izaberite najmanje jedno vozilo.';
 if(!$greska){try{$r=new IznajmljivanjeRepozitorijum();$id=$r->dodajLegacy((int)$_SESSION['korisnik']['ID'],$od,$do,$izabrana);Sesija::postaviPoruku('Iznajmljivanje je uspešno kreirano.');header('Location: detalji.php?id='.$id.'&uspeh=1');exit;}catch(Throwable $e){$greska=$e->getMessage();}}
}
?>
<!doctype html><html lang="sr"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1"><title>Novo iznajmljivanje | Rent-a-Car</title><link rel="stylesheet" href="../javno/css/stil.css"><script src="../javno/js/validacija.js"></script></head><body><header class="zaglavlje"><strong>Rent-a-Car</strong><nav><a href="index.php">Nazad na pregled</a><a href="../prijava/odjava.php">Odjava</a></nav></header><main class="kontejner"><h1>Novo iznajmljivanje</h1><p class="podnaslov">Unesite period i izaberite jedno ili više dostupnih vozila.</p><?php if($greska):?><div class="greska"><?=htmlspecialchars($greska)?></div><?php endif;?>
<form method="post" onsubmit="return proveriIznajmljivanje(this)"><div class="forma-red"><div class="forma-polje"><label>Datum od</label><input type="date" name="datumOd" required></div><div class="forma-polje"><label>Datum do</label><input type="date" name="datumDo" required></div></div><h2>Vozila — stavke iznajmljivanja</h2><div class="stavke-lista">
<?php foreach($vozila as $v):?><label class="stavka"><input type="checkbox" name="vozila[]" value="<?=$v['ID']?>"><span><strong><?=htmlspecialchars($v['Marka'].' '.$v['Model'])?></strong> · <?=htmlspecialchars($v['Registracija'])?> · <?=number_format((float)$v['CenaPoDanu'],2,',','.')?> RSD/dan</span></label><?php endforeach;?></div>
<div class="traka-akcija"><button>Sačuvaj iznajmljivanje</button><a class="dugme dugme-sekundarno" href="index.php">Odustani</a></div></form></main></body></html>