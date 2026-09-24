<?php
// Ova stranica je deo korisnickog interfejsa i povezuje formu sa kontrolerom/repozitorijumom.
require_once __DIR__.'/../servisi/Sesija.php'; Sesija::zahtevajPrijavu();
require_once __DIR__.'/../podaci/IznajmljivanjeRepozitorijum.php';
$r=(new IznajmljivanjeRepozitorijum())->dajPoId((int)($_GET['id']??0)); if(!$r)die('Dokument nije pronađen.');
?>
<!doctype html><html lang="sr"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1"><title>Ugovor #<?=$r['ID']?> | Rent-a-Car</title><link rel="stylesheet" href="../javno/css/stil.css"></head><body class="stampanje-stranica"><main class="kontejner dokument">
<div class="traka-akcija bez-stampe"><button onclick="window.print()">Štampaj dokument</button><button class="dugme-sekundarno" onclick="window.close()">Zatvori</button></div>
<div class="dokument-zaglavlje"><div><div class="dokument-logo"><span>R</span> RENT-A-CAR</div><div class="dokument-tip">Sistem za iznajmljivanje vozila</div></div><div class="dokument-meta"><strong>UGOVOR #<?=str_pad((string)$r['ID'],5,'0',STR_PAD_LEFT)?></strong><br>Datum štampe: <?=date('d.m.Y.')?></div></div>
<div class="naslov-dokumenta"><span>UGOVOR O IZNAJMLJIVANJU VOZILA</span><small>Dokument evidencije iznajmljivanja</small></div>
<div class="ugovor-sekcije"><section><h3>Podaci o klijentu</h3><dl><dt>Ime i prezime</dt><dd><?=htmlspecialchars($r['Ime'].' '.$r['Prezime'])?></dd><dt>Mejl adresa</dt><dd><?=htmlspecialchars($r['Mejl'])?></dd></dl></section><section><h3>Podaci o iznajmljivanju</h3><dl><dt>Datum preuzimanja</dt><dd><?=$r['DatumOd']?></dd><dt>Datum vraćanja</dt><dd><?=$r['DatumDo']?></dd><dt>Status</dt><dd><?=htmlspecialchars($r['Status'])?></dd></dl></section></div>
<h3 class="podnaslov-tabele">Iznajmljena vozila</h3><div class="tabela-okvir"><table><thead><tr><th>R.br.</th><th>Vozilo</th><th>Registracija</th><th>Dana</th><th>Cena po danu</th><th>Iznos</th></tr></thead><tbody><?php foreach($r['Stavke'] as $i=>$s):?><tr><td><?=$i+1?></td><td><strong><?=htmlspecialchars($s['Marka'].' '.$s['Model'])?></strong></td><td><?=htmlspecialchars($s['Registracija'])?></td><td><?=$s['BrojDana']?></td><td><?=number_format((float)$s['CenaPoDanu'],2,',','.')?> RSD</td><td><?=number_format((float)$s['Iznos'],2,',','.')?> RSD</td></tr><?php endforeach;?></tbody></table></div>
<div class="ukupno-dokument"><span>UKUPNO ZA PLAĆANJE</span><strong><?=number_format((float)$r['UkupnaCena'],2,',','.')?> RSD</strong></div>
<div class="ugovor-napomena">Potpisom dokumenta klijent potvrđuje da je upoznat sa evidentiranim podacima o periodu iznajmljivanja, vozilima i ukupnom cenom.</div>
<div class="potpisi"><div class="potpis">Potpis klijenta</div><div class="potpis">Potpis zaposlenog</div></div>
<div class="dokument-futer">Rent-a-Car · Evidencioni dokument generisan iz informacionog sistema</div>
</main></body></html>
