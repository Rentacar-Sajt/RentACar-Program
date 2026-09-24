<?php
// Ova stranica je deo korisnickog interfejsa i povezuje formu sa kontrolerom/repozitorijumom.
require_once __DIR__.'/../servisi/Sesija.php'; Sesija::zahtevajPrijavu();
require_once __DIR__.'/../podaci/IznajmljivanjeRepozitorijum.php';
$id=(int)($_GET['id']??0); $r=(new IznajmljivanjeRepozitorijum())->dajPoId($id);
if(!$r){http_response_code(404);die('Iznajmljivanje nije pronađeno.');}
$poruka=Sesija::uzmiPoruku(); $statusKlasa=$r['Status']==='Aktivno'?'status-aktivan':'status-zavrsen';
?>
<!doctype html><html lang="sr"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1"><title>Detalji | Rent-a-Car</title><link rel="stylesheet" href="../javno/css/stil.css"></head><body>
<header class="zaglavlje"><strong>Rent-a-Car</strong><nav><a href="index.php">Kontrolna tabla</a><a href="izmeni.php?id=<?=$r['ID']?>">Izmeni</a><a href="../prijava/odjava.php">Odjava</a></nav></header>
<main class="stranica"><div class="breadcrumb"><a href="index.php">Početna</a><span>›</span><a href="index.php#spisak">Iznajmljivanja</a><span>›</span><strong>#<?=$r['ID']?></strong></div>
<section class="panel"><div class="sekcija-zaglavlje"><div><span class="eyebrow">DETALJI DOKUMENTA</span><h1>Iznajmljivanje #<?=$r['ID']?></h1><p>Detaljan prikaz ugovora i svih vozila u iznajmljivanju.</p></div><span class="status <?=$statusKlasa?>"><?=htmlspecialchars($r['Status'])?></span></div>
<?php if($poruka):?><div class="<?=($poruka['tip']??'uspeh')==='greska'?'greska':'poruka'?>"><?=htmlspecialchars($poruka['tekst'])?></div><?php endif;?>
<div class="info-mreza"><div class="info-kartica"><span>Klijent</span><strong><?=htmlspecialchars($r['Ime'].' '.$r['Prezime'])?></strong><div><?=htmlspecialchars($r['Mejl'])?></div></div><div class="info-kartica"><span>Period</span><strong><?=$r['DatumOd']?> — <?=$r['DatumDo']?></strong></div><div class="info-kartica"><span>Broj vozila</span><strong><?=count($r['Stavke'])?></strong></div></div>
<div class="tabela-okvir"><table><thead><tr><th>Vozilo</th><th>Registracija</th><th>Dana</th><th>Cena/dan</th><th>Iznos</th></tr></thead><tbody><?php foreach($r['Stavke'] as $s):?><tr><td><strong><?=htmlspecialchars($s['Marka'].' '.$s['Model'])?></strong></td><td><span class="registracija"><?=htmlspecialchars($s['Registracija'])?></span></td><td><?=$s['BrojDana']?></td><td><?=number_format((float)$s['CenaPoDanu'],2,',','.')?> RSD</td><td><strong><?=number_format((float)$s['Iznos'],2,',','.')?> RSD</strong></td></tr><?php endforeach;?></tbody></table></div>
<div class="ukupno-kartica"><span>Ukupna cena iznajmljivanja</span><strong><?=number_format((float)$r['UkupnaCena'],2,',','.')?> RSD</strong></div>
<div class="traka-akcija"><a class="dugme" target="_blank" href="../stampa/dokument.php?id=<?=$r['ID']?>">Štampaj ugovor</a><a class="dugme dugme-sekundarno" href="izmeni.php?id=<?=$r['ID']?>">Izmeni podatke</a><a class="dugme dugme-sekundarno" href="index.php#spisak">Nazad na pregled</a></div></section></main>
<footer class="podnozje"><span>Rent-a-Car sistem</span><span>© <?=date('Y')?> · Veb programiranje</span></footer></body></html>
