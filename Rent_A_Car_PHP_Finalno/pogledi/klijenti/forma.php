<?php
$izmena = !empty($klijent['ID']);
$naslovStranice = $izmena ? 'Izmena klijenta' : 'Novi klijent';
require __DIR__.'/../delovi/zaglavlje.php';
?>
<main class="stranica klijent-forma-stranica">
    <div class="breadcrumb">
        <a href="index.php?stranica=pocetna">Početna</a><span>›</span>
        <a href="index.php?stranica=klijenti">Klijenti</a><span>›</span>
        <strong><?=$izmena ? 'Izmena klijenta' : 'Novi klijent'?></strong>
    </div>

    <section class="novo-hero klijent-hero">
        <div>
            <span class="eyebrow"><?=$izmena ? 'IZMENA PODATAKA' : 'NOVI KLIJENT'?></span>
            <h1><?=$izmena ? 'Izmeni klijenta' : 'Dodaj novog klijenta'?></h1>
            <p><?=$izmena ? 'Ažurirajte kontakt i identifikacione podatke klijenta.' : 'Unesite osnovne podatke osobe koja će iznajmljivati vozilo.'?></p>
        </div>
        <div class="klijent-hero-oznaka">
            <span>👤</span>
            <strong><?=$izmena ? 'Klijent #'.(int)$klijent['ID'] : 'Evidencija klijenata'?></strong>
        </div>
    </section>

    <?php if($greska):?>
        <div class="greska obavestenje"><span><?=htmlspecialchars($greska)?></span></div>
    <?php endif;?>

    <form class="klijent-prof-forma" method="post" action="index.php?stranica=<?=$izmena ? 'klijent-izmeni&id='.(int)$klijent['ID'] : 'klijent-dodaj'?>" onsubmit="return proveriKlijenta(this)">
        <div class="klijent-forma-grid">
            <div class="panel forma-sekcija">
                <div class="forma-sekcija-zaglavlje">
                    <div class="sekcija-ikona">🪪</div>
                    <div>
                        <span class="sekcija-broj">OSNOVNI PODACI</span>
                        <h2>Podaci o klijentu</h2>
                        <p>Sva polja su obavezna. Mejl i broj dokumenta moraju biti jedinstveni.</p>
                    </div>
                </div>

                <div class="forma-red">
                    <div class="forma-polje">
                        <label for="ime">Ime</label>
                        <input id="ime" name="ime" minlength="2" maxlength="50" required placeholder="Unesite ime" value="<?=htmlspecialchars($klijent['Ime']??$klijent['ime']??'')?>">
                    </div>
                    <div class="forma-polje">
                        <label for="prezime">Prezime</label>
                        <input id="prezime" name="prezime" minlength="2" maxlength="50" required placeholder="Unesite prezime" value="<?=htmlspecialchars($klijent['Prezime']??$klijent['prezime']??'')?>">
                    </div>
                </div>

                <div class="forma-red">
                    <div class="forma-polje">
                        <label for="mejl">Mejl adresa</label>
                        <input id="mejl" type="email" name="mejl" maxlength="150" required placeholder="primer@email.com" value="<?=htmlspecialchars($klijent['Mejl']??$klijent['mejl']??'')?>">
                        <small>Koristi se kao kontakt podatak klijenta.</small>
                    </div>
                    <div class="forma-polje">
                        <label for="telefon">Telefon</label>
                        <input id="telefon" name="telefon" minlength="6" maxlength="20" pattern="[0-9+\-\/ ]{6,20}" required placeholder="+381 60 123 4567" value="<?=htmlspecialchars($klijent['Telefon']??$klijent['telefon']??'')?>">
                        <small>Dozvoljeni su brojevi, +, -, / i razmak.</small>
                    </div>
                </div>

                <div class="forma-polje forma-polje-istaknuto klijent-dokument-polje">
                    <label for="brojDokumenta">Broj ličnog dokumenta</label>
                    <input id="brojDokumenta" name="brojDokumenta" minlength="5" maxlength="30" pattern="[A-Za-z0-9\-\/]{5,30}" required placeholder="Na primer: 123456789" value="<?=htmlspecialchars($klijent['BrojDokumenta']??$klijent['brojDokumenta']??'')?>">
                    <small>Broj dokumenta mora biti jedinstven u evidenciji klijenata.</small>
                </div>
            </div>

            <aside class="panel klijent-rezime">
                <span class="sekcija-broj">POTVRDA</span>
                <h2><?=$izmena ? 'Sačuvaj izmene' : 'Sačuvaj klijenta'?></h2>
                <p>Pre čuvanja proverite da li su svi podaci tačno uneti.</p>

                <div class="klijent-check-lista">
                    <div><span>✓</span><p><strong>Ime i prezime</strong><small>Najmanje 2 karaktera</small></p></div>
                    <div><span>✓</span><p><strong>Kontakt</strong><small>Ispravan mejl i telefon</small></p></div>
                    <div><span>✓</span><p><strong>Lični dokument</strong><small>Jedinstven broj dokumenta</small></p></div>
                </div>

                <div class="rezime-napomena">
                    <span>ℹ</span>
                    <p>Validacija se izvršava u pregledaču i ponovo na serveru u PHP-u.</p>
                </div>

                <button class="dugme-siroko" type="submit"><?=$izmena ? 'Sačuvaj izmene' : 'Dodaj klijenta'?></button>
                <a class="dugme dugme-sekundarno dugme-siroko" href="index.php?stranica=klijenti">Odustani</a>
            </aside>
        </div>
    </form>
</main>
<?php require __DIR__.'/../delovi/podnozje.php'; ?>
