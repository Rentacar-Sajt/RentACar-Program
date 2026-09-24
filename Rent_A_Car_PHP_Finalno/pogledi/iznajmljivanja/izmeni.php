<?php
$naslovStranice='Izmena iznajmljivanja';
require __DIR__.'/../delovi/zaglavlje.php';
$post = ($_SERVER['REQUEST_METHOD'] === 'POST');
$od = $post ? ($_POST['datumOd'] ?? '') : $zapis['DatumOd'];
$do = $post ? ($_POST['datumDo'] ?? '') : $zapis['DatumDo'];
$klijentIzabran = $post ? (int)($_POST['klijentId'] ?? 0) : (int)$zapis['KlijentID'];
$trenutniId = $trenutniId ?? array_map(fn($s)=>(int)($s['VoziloID'] ?? 0), $zapis['Stavke'] ?? []);
$izabrana = $post ? array_map('intval', $_POST['vozila'] ?? []) : $trenutniId;
?>
<main class="stranica novo-iznajmljivanje-stranica">
    <div class="breadcrumb">
        <a href="index.php?stranica=pocetna">Početna</a><span>›</span>
        <a href="index.php?stranica=iznajmljivanja">Iznajmljivanja</a><span>›</span>
        <strong>Izmena #<?=$id?></strong>
    </div>

    <section class="novo-hero izmena-hero">
        <div>
            <span class="eyebrow">IZMENA UGOVORA #<?=$id?></span>
            <h1>Izmeni iznajmljivanje</h1>
            <p>Ažurirajte klijenta, period najma ili vozila koja pripadaju ovom ugovoru.</p>
        </div>
        <div class="koraci-unosa" aria-label="Koraci izmene">
            <span class="korak aktivan"><b>1</b> Klijent i period</span>
            <span class="korak"><b>2</b> Vozila</span>
            <span class="korak"><b>3</b> Potvrda izmena</span>
        </div>
    </section>

    <?php if($greska):?>
        <div class="greska obavestenje"><span><?=htmlspecialchars($greska)?></span></div>
    <?php endif;?>

    <form class="novo-iznajmljivanje-forma" method="post" action="index.php?stranica=iznajmljivanje-izmeni&id=<?=$id?>" onsubmit="return proveriIznajmljivanje(this)">
        <div class="novo-iznajmljivanje-grid">
            <div class="novo-glavni-sadrzaj">
                <section class="panel forma-sekcija">
                    <div class="forma-sekcija-zaglavlje">
                        <div class="sekcija-ikona">👤</div>
                        <div>
                            <span class="sekcija-broj">KORAK 1</span>
                            <h2>Klijent i period iznajmljivanja</h2>
                            <p>Promenite klijenta ili datume samo ako je potrebno.</p>
                        </div>
                    </div>

                    <div class="forma-polje forma-polje-istaknuto">
                        <label for="klijentId">Klijent</label>
                        <div class="select-omotac">
                            <select id="klijentId" name="klijentId" required>
                                <?php foreach($klijenti as $k):?>
                                    <option value="<?=$k['ID']?>" <?=$klijentIzabran === (int)$k['ID'] ? 'selected' : ''?>><?=htmlspecialchars($k['Ime'].' '.$k['Prezime'].' · '.$k['BrojDokumenta'])?></option>
                                <?php endforeach;?>
                            </select>
                        </div>
                        <div class="pomoc-red">
                            <small>Trenutno izabrani klijent je već vezan za ovaj ugovor.</small>
                            <a href="index.php?stranica=klijenti" class="mini-link">Pregled svih klijenata</a>
                        </div>
                    </div>

                    <div class="datumi-kartice">
                        <div class="datum-kartica">
                            <div class="datum-ikona">↗</div>
                            <div class="forma-polje">
                                <label for="datumOd">Datum preuzimanja</label>
                                <input id="datumOd" type="date" name="datumOd" required value="<?=htmlspecialchars($od)?>">
                            </div>
                        </div>
                        <div class="datum-kartica">
                            <div class="datum-ikona">↙</div>
                            <div class="forma-polje">
                                <label for="datumDo">Datum vraćanja</label>
                                <input id="datumDo" type="date" name="datumDo" required value="<?=htmlspecialchars($do)?>">
                            </div>
                        </div>
                    </div>
                </section>

                <section class="panel forma-sekcija vozila-sekcija">
                    <div class="forma-sekcija-zaglavlje vozila-zaglavlje">
                        <div class="sekcija-ikona">🚗</div>
                        <div>
                            <span class="sekcija-broj">KORAK 2</span>
                            <h2>Vozila na ugovoru</h2>
                            <p>Zadržite postojeća vozila ili promenite izbor dostupnih vozila.</p>
                        </div>
                        <span class="broj-rezultata"><?=count($vozila)?> ponuđeno</span>
                    </div>

                    <div class="vozila-alati">
                        <div class="pretraga-vozila">
                            <span>⌕</span>
                            <input type="search" id="pretragaVozila" placeholder="Pretraži po marki, modelu ili registraciji..." autocomplete="off">
                        </div>
                        <div class="izabrano-indikator"><span id="brojIzabranih">0</span> izabrano</div>
                    </div>

                    <?php if($vozila):?>
                        <div class="vozila-grid" id="vozilaGrid">
                            <?php foreach($vozila as $v):?>
                                <label class="vozilo-kartica" data-pretraga="<?=htmlspecialchars(strtolower($v['Marka'].' '.$v['Model'].' '.$v['Registracija']))?>">
                                    <input class="vozilo-checkbox" type="checkbox" name="vozila[]" value="<?=$v['ID']?>" data-cena="<?=htmlspecialchars((string)$v['CenaPoDanu'])?>" <?=in_array((int)$v['ID'], $izabrana, true) ? 'checked' : ''?>>
                                    <span class="check-oznaka">✓</span>
                                    <span class="vozilo-ikona">🚘</span>
                                    <span class="vozilo-podaci">
                                        <strong><?=htmlspecialchars($v['Marka'].' '.$v['Model'])?></strong>
                                        <span class="registracija"><?=htmlspecialchars($v['Registracija'])?></span>
                                    </span>
                                    <span class="vozilo-cena">
                                        <strong><?=number_format((float)$v['CenaPoDanu'],0,',','.')?> RSD</strong>
                                        <small>po danu</small>
                                    </span>
                                </label>
                            <?php endforeach;?>
                        </div>
                        <div class="prazno-stanje malo" id="nemaVozilaPretraga" hidden>
                            <div class="prazno-ikona">🔎</div>
                            <h3>Nema pronađenih vozila</h3>
                            <p>Pokušajte sa drugim pojmom za pretragu.</p>
                        </div>
                    <?php else:?>
                        <div class="prazno-stanje">
                            <div class="prazno-ikona">🚗</div>
                            <h3>Nema vozila za izbor</h3>
                            <p>Trenutno nema dostupnih vozila koja se mogu dodati ovom ugovoru.</p>
                        </div>
                    <?php endif;?>
                </section>
            </div>

            <aside class="novo-rezime panel">
                <div class="rezime-zaglavlje">
                    <span class="sekcija-broj">KORAK 3</span>
                    <h2>Pregled izmena</h2>
                    <p>Proverite podatke pre nego što sačuvate izmene.</p>
                </div>

                <div class="ugovor-mini-oznaka">
                    <span>Broj ugovora</span>
                    <strong>#<?=$id?></strong>
                </div>

                <div class="rezime-red">
                    <span>Klijent</span>
                    <strong id="rezimeKlijent">—</strong>
                </div>
                <div class="rezime-red">
                    <span>Period</span>
                    <strong id="rezimePeriod">—</strong>
                </div>
                <div class="rezime-red">
                    <span>Broj dana</span>
                    <strong id="rezimeDani">0</strong>
                </div>
                <div class="rezime-red">
                    <span>Vozila</span>
                    <strong id="rezimeVozila">0</strong>
                </div>

                <div class="rezime-ukupno">
                    <span>Procenjena ukupna cena</span>
                    <strong id="rezimeUkupno">0 RSD</strong>
                    <small>Prikaz se automatski osvežava dok menjate period ili vozila.</small>
                </div>

                <div class="rezime-napomena">
                    <span>ℹ</span>
                    <p>Izmena celine i svih stavki ugovora izvršava se u jednoj transakciji.</p>
                </div>

                <button class="dugme-siroko" type="submit">Sačuvaj izmene</button>
                <a class="dugme dugme-sekundarno dugme-siroko" href="index.php?stranica=iznajmljivanje-detalji&id=<?=$id?>">Odustani</a>
            </aside>
        </div>
    </form>
</main>
<?php require __DIR__.'/../delovi/podnozje.php'; ?>
