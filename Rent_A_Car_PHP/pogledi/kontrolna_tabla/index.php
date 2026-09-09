<?php $naslovStranice='Kontrolna tabla'; require __DIR__.'/../delovi/zaglavlje.php'; ?>
<main class="stranica">
<div class="breadcrumb"><span>Početna</span><span>›</span><strong>Kontrolna tabla</strong></div>
<section class="hero-panel"><div><span class="eyebrow">SISTEM ZA UPRAVLJANJE IZNAJMLJIVANJEM</span><h1>Dobro došli, <?=htmlspecialchars($korisnik['Ime'])?></h1><p>Korisnik sistema evidentira klijente, vozila i ugovore o iznajmljivanju.</p></div><div class="hero-akcije"><a class="dugme" href="index.php?stranica=iznajmljivanje-dodaj">+ Novo iznajmljivanje</a><a class="dugme dugme-sekundarno" href="index.php?stranica=klijenti">Klijenti</a></div></section>
<?php if($poruka):?><div class="<?=($poruka['tip']??'uspeh')==='greska'?'greska':'poruka'?> obavestenje" data-obavestenje><span><?=htmlspecialchars($poruka['tekst'])?></span><button type="button" class="zatvori-obavestenje">×</button></div><?php endif;?>
<section class="stat-grid">
<article class="stat-kartica"><div class="stat-ikona">🚗</div><div><span>Ukupno vozila</span><strong><?=$vozilaStat['ukupno']?></strong><small><?=$vozilaStat['slobodna']?> slobodno</small></div></article>
<article class="stat-kartica"><div class="stat-ikona">👥</div><div><span>Klijenti</span><strong><?=$brojKlijenata?></strong><small>Poslovna evidencija</small></div></article>
<article class="stat-kartica"><div class="stat-ikona">🔑</div><div><span>Aktivna iznajmljivanja</span><strong><?=$statistika['aktivna']?></strong><small>Trenutno aktivno</small></div></article>
<article class="stat-kartica"><div class="stat-ikona">📄</div><div><span>Ukupno ugovora</span><strong><?=$statistika['ukupno']?></strong><small><?=$statistika['zavrsena']?> završeno</small></div></article>
</section>
<section class="panel"><div class="sekcija-zaglavlje"><div><h2>Poslednja iznajmljivanja</h2><p>Najnoviji evidentirani dokumenti.</p></div><a class="dugme dugme-sekundarno" href="index.php?stranica=iznajmljivanja">Prikaži sve</a></div>
<?php if(!$poslednja):?><div class="prazno-stanje"><div class="prazno-ikona">📋</div><h3>Nema iznajmljivanja</h3></div><?php else:?><div class="tabela-okvir bez-gornje-margine"><table><thead><tr><th>ID</th><th>Klijent</th><th>Period</th><th>Status</th><th>Ukupno</th><th></th></tr></thead><tbody>
<?php foreach($poslednja as $r):?><tr><td><strong>#<?=$r['ID']?></strong></td><td><?=htmlspecialchars($r['Ime'].' '.$r['Prezime'])?></td><td><?=$r['DatumOd']?> — <?=$r['DatumDo']?></td><td><span class="status <?=$r['Status']==='Aktivno'?'status-aktivan':'status-zavrsen'?>"><?=htmlspecialchars($r['Status'])?></span></td><td><strong><?=number_format((float)$r['UkupnaCena'],2,',','.')?> RSD</strong></td><td><a class="link-akcija" href="index.php?stranica=iznajmljivanje-detalji&id=<?=$r['ID']?>">Otvori →</a></td></tr><?php endforeach;?>
</tbody></table></div><?php endif;?></section>

</main>
<?php require __DIR__.'/../delovi/podnozje.php'; ?>
