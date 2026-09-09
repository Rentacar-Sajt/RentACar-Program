<?php
require_once __DIR__ . '/../servisi/Sesija.php';
Sesija::zahtevajPrijavu();
require_once __DIR__ . '/../podaci/IznajmljivanjeRepozitorijum.php';
require_once __DIR__ . '/../podaci/VoziloRepozitorijum.php';

$filter = trim($_GET['filter'] ?? '');
$repo = new IznajmljivanjeRepozitorijum();
$voziloRepo = new VoziloRepozitorijum();
$redovi = $repo->dajSve($filter);
$statistika = $repo->dajStatistiku();
$vozilaStat = $voziloRepo->dajStatistiku();
$poslednja = $repo->dajPoslednja(5);
$poruka = Sesija::uzmiPoruku();
$korisnik = $_SESSION['korisnik'];
?>
<!doctype html>
<html lang="sr">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>Kontrolna tabla | Rent-a-Car</title>
    <link rel="stylesheet" href="../javno/css/stil.css">
</head>
<body>
<header class="zaglavlje">
    <strong>Rent-a-Car</strong>
    <nav>
        <a class="aktivna" href="index.php">Kontrolna tabla</a>
        <a href="dodaj.php">Novo iznajmljivanje</a>
        <a href="../stampa/spisak.php" target="_blank">Štampa</a>
        <a href="../prijava/odjava.php">Odjava</a>
    </nav>
</header>

<main class="stranica">
    <div class="breadcrumb"><span>Početna</span><span>›</span><strong>Kontrolna tabla</strong></div>

    <section class="hero-panel">
        <div>
            <span class="eyebrow">SISTEM ZA UPRAVLJANJE VOZILIMA</span>
            <h1>Dobro došli, <?=htmlspecialchars($korisnik['Ime'])?></h1>
            <p>Brz pregled poslovanja i upravljanje iznajmljivanjima na jednom mestu.</p>
        </div>
        <div class="hero-akcije">
            <a class="dugme" href="dodaj.php">+ Novo iznajmljivanje</a>
            <a class="dugme dugme-sekundarno" href="#spisak">Pregled evidencije</a>
        </div>
    </section>

    <?php if ($poruka): ?>
        <div class="<?=($poruka['tip'] ?? 'uspeh') === 'greska' ? 'greska' : 'poruka'?> obavestenje" data-obavestenje>
            <span><?=htmlspecialchars($poruka['tekst'])?></span><button type="button" class="zatvori-obavestenje" aria-label="Zatvori">×</button>
        </div>
    <?php endif; ?>

    <section class="stat-grid">
        <article class="stat-kartica"><div class="stat-ikona">🚗</div><div><span>Ukupno vozila</span><strong><?=$vozilaStat['ukupno']?></strong><small><?=$vozilaStat['slobodna']?> trenutno slobodno</small></div></article>
        <article class="stat-kartica"><div class="stat-ikona">🔑</div><div><span>Aktivna iznajmljivanja</span><strong><?=$statistika['aktivna']?></strong><small>Vozila kod klijenata</small></div></article>
        <article class="stat-kartica"><div class="stat-ikona">✓</div><div><span>Završena iznajmljivanja</span><strong><?=$statistika['zavrsena']?></strong><small>Završeni ugovori</small></div></article>
        <article class="stat-kartica"><div class="stat-ikona">📄</div><div><span>Ukupno ugovora</span><strong><?=$statistika['ukupno']?></strong><small>Kompletna evidencija</small></div></article>
    </section>

    <section class="panel poslednja-sekcija">
        <div class="sekcija-zaglavlje"><div><h2>Poslednja iznajmljivanja</h2><p>Najnoviji evidentirani ugovori.</p></div></div>
        <?php if (!$poslednja): ?>
            <div class="prazno-stanje"><div class="prazno-ikona">📋</div><h3>Još nema iznajmljivanja</h3><p>Kreirajte prvo iznajmljivanje da bi se ovde pojavila evidencija.</p><a class="dugme" href="dodaj.php">Dodaj iznajmljivanje</a></div>
        <?php else: ?>
        <div class="tabela-okvir bez-gornje-margine"><table><thead><tr><th>ID</th><th>Klijent</th><th>Period</th><th>Status</th><th>Ukupno</th><th>Detalji</th></tr></thead><tbody>
        <?php foreach($poslednja as $r): $statusKlasa = $r['Status']==='Aktivno' ? 'status-aktivan' : 'status-zavrsen'; ?>
            <tr><td><strong>#<?=$r['ID']?></strong></td><td><?=htmlspecialchars($r['Ime'].' '.$r['Prezime'])?></td><td><?=$r['DatumOd']?> — <?=$r['DatumDo']?></td><td><span class="status <?=$statusKlasa?>"><?=htmlspecialchars($r['Status'])?></span></td><td><strong><?=number_format((float)$r['UkupnaCena'],2,',','.')?> RSD</strong></td><td><a class="link-akcija" href="detalji.php?id=<?=$r['ID']?>">Otvori →</a></td></tr>
        <?php endforeach; ?>
        </tbody></table></div>
        <?php endif; ?>
    </section>

    <section class="panel" id="spisak">
        <div class="sekcija-zaglavlje"><div><h2>Pregled svih iznajmljivanja</h2><p>Filtrirajte, pregledajte, izmenite ili odštampajte podatke.</p></div><span class="broj-rezultata"><?=count($redovi)?> rezultat<?=count($redovi)===1?'':'a'?></span></div>
        <form class="filter-forma bez-stampe">
            <input class="filter-input" name="filter" value="<?=htmlspecialchars($filter)?>" placeholder="Pretraži po klijentu, statusu ili ID-u">
            <button>Filtriraj</button>
            <?php if($filter!==''):?><a class="dugme dugme-sekundarno" href="index.php#spisak">Očisti filter</a><?php endif;?>
            <a class="dugme dugme-sekundarno" href="../stampa/spisak.php?filter=<?=urlencode($filter)?>" target="_blank">Štampa spiska</a>
        </form>

        <?php if (!$redovi): ?>
            <div class="prazno-stanje"><div class="prazno-ikona">🔎</div><h3>Nema pronađenih rezultata</h3><p>Promenite kriterijum pretrage ili očistite filter.</p><a class="dugme dugme-sekundarno" href="index.php#spisak">Prikaži sve</a></div>
        <?php else: ?>
        <div class="tabela-okvir"><table><thead><tr><th>ID</th><th>Klijent</th><th>Period</th><th>Status</th><th>Ukupno</th><th class="bez-stampe">Akcije</th></tr></thead><tbody>
        <?php foreach($redovi as $r): $statusKlasa = $r['Status']==='Aktivno' ? 'status-aktivan' : 'status-zavrsen'; ?>
            <tr><td><strong>#<?=$r['ID']?></strong></td><td><?=htmlspecialchars($r['Ime'].' '.$r['Prezime'])?></td><td><?=$r['DatumOd']?> — <?=$r['DatumDo']?></td><td><span class="status <?=$statusKlasa?>"><?=htmlspecialchars($r['Status'])?></span></td><td><strong><?=number_format((float)$r['UkupnaCena'],2,',','.')?> RSD</strong></td><td class="bez-stampe"><div class="akcije"><a href="detalji.php?id=<?=$r['ID']?>">Detalji</a><a href="izmeni.php?id=<?=$r['ID']?>">Izmeni</a><a href="obrisi.php?id=<?=$r['ID']?>" onclick="return confirm('Da li ste sigurni da želite da obrišete iznajmljivanje #<?=$r['ID']?>? Ova akcija ne može da se poništi.')">Obriši</a></div></td></tr>
        <?php endforeach; ?>
        </tbody></table></div>
        <?php endif; ?>
    </section>
</main>
<footer class="podnozje"><span>Rent-a-Car sistem</span><span>© <?=date('Y')?> · Veb programiranje</span></footer>
<script>
document.querySelectorAll('.zatvori-obavestenje').forEach(function(btn){btn.addEventListener('click',function(){btn.closest('[data-obavestenje]').remove();});});
setTimeout(function(){document.querySelectorAll('[data-obavestenje]').forEach(function(x){x.classList.add('sakrij');});},4500);
</script>
</body></html>
