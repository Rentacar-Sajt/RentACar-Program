<?php
$naslovStranice='Novo iznajmljivanje';
require __DIR__.'/../delovi/zaglavlje.php';
$izabranaVozila = array_map('strval', $_POST['vozila'] ?? []);
?>
<main class="stranica novo-iznajmljivanje-stranica">
    <div class="breadcrumb">
        <a href="index.php?stranica=pocetna">Početna</a><span>›</span>
        <a href="index.php?stranica=iznajmljivanja">Iznajmljivanja</a><span>›</span>
        <strong>Novo iznajmljivanje</strong>
    </div>

    <section class="novo-hero">
        <div>
            <span class="eyebrow">NOVI UGOVOR</span>
            <h1>Novo iznajmljivanje</h1>
            <p>Izaberite klijenta, period najma i jedno ili više dostupnih vozila.</p>
        </div>
        <div class="koraci-unosa" aria-label="Koraci unosa">
            <span class="korak aktivan"><b>1</b> Klijent i period</span>
            <span class="korak"><b>2</b> Vozila</span>
            <span class="korak"><b>3</b> Potvrda</span>
        </div>
    </section>

    <?php if($greska):?>
        <div class="greska obavestenje"><span><?=htmlspecialchars($greska)?></span></div>
    <?php endif;?>

    <form class="novo-iznajmljivanje-forma" method="post" action="index.php?stranica=iznajmljivanje-dodaj" onsubmit="return proveriIznajmljivanje(this)">
        <div class="novo-iznajmljivanje-grid">
            <div class="novo-glavni-sadrzaj">
                <section class="panel forma-sekcija">
                    <div class="forma-sekcija-zaglavlje">
                        <div class="sekcija-ikona">👤</div>
                        <div>
                            <span class="sekcija-broj">KORAK 1</span>
                            <h2>Klijent i period iznajmljivanja</h2>
                            <p>Odaberite klijenta iz evidencije i unesite datume preuzimanja i vraćanja.</p>
                        </div>
                    </div>

                    <div class="forma-polje forma-polje-istaknuto">
                        <label for="klijentId">Klijent</label>
                        <div class="select-omotac">
                            <select id="klijentId" name="klijentId" required>
                                <option value="">Izaberite klijenta</option>
                                <?php foreach($klijenti as $k):?>
                                    <option value="<?=$k['ID']?>" <?=((int)($_POST['klijentId']??0)===(int)$k['ID'])?'selected':''?>><?=htmlspecialchars($k['Ime'].' '.$k['Prezime'].' · '.$k['BrojDokumenta'])?></option>
                                <?php endforeach;?>
                            </select>
                        </div>
                        <div class="pomoc-red">
                            <small>Klijent je osoba koja iznajmljuje vozilo i odvojena je od korisnika sistema.</small>
                            <a href="index.php?stranica=klijent-dodaj" class="mini-link">+ Dodaj novog klijenta</a>
                        </div>
                    </div>

                    <div class="datumi-kartice">
                        <div class="datum-kartica">
                            <div class="datum-ikona">↗</div>
                            <div class="forma-polje">
                                <label for="datumOd">Datum preuzimanja</label>
                                <input id="datumOd" type="date" name="datumOd" required value="<?=htmlspecialchars($_POST['datumOd']??'')?>">
                            </div>
                        </div>
                        <div class="datum-kartica">
                            <div class="datum-ikona">↙</div>
                            <div class="forma-polje">
                                <label for="datumDo">Datum vraćanja</label>
                                <input id="datumDo" type="date" name="datumDo" required value="<?=htmlspecialchars($_POST['datumDo']??'')?>">
                            </div>
                        </div>
                    </div>
                </section>

                <section class="panel forma-sekcija vozila-sekcija">
                    <div class="forma-sekcija-zaglavlje vozila-zaglavlje">
                        <div class="sekcija-ikona">🚗</div>
                        <div>
                            <span class="sekcija-broj">KORAK 2</span>
                            <h2>Izaberite vozila</h2>
                            <p>Možete izabrati jedno ili više slobodnih vozila za isti ugovor.</p>
                        </div>
                        <span class="broj-rezultata" id="brojDostupnihVozila"><?=count($vozila)?> dostupno</span>
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
                                <input class="vozilo-checkbox" type="checkbox" name="vozila[]" value="<?=$v['ID']?>" data-cena="<?=htmlspecialchars((string)$v['CenaPoDanu'])?>" <?=in_array((string)$v['ID'],$izabranaVozila,true)?'checked':''?>>
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
                            <h3>Trenutno nema slobodnih vozila</h3>
                            <p>Vozilo mora imati status „Slobodno“ da bi moglo biti dodato u novo iznajmljivanje.</p>
                        </div>
                    <?php endif;?>
                </section>
            </div>

            <aside class="novo-rezime panel">
                <div class="rezime-zaglavlje">
                    <span class="sekcija-broj">KORAK 3</span>
                    <h2>Pregled iznajmljivanja</h2>
                    <p>Proverite osnovne podatke pre čuvanja.</p>
                </div>

                <div class="rezime-red">
                    <span>Klijent</span>
                    <strong id="rezimeKlijent">Nije izabran</strong>
                </div>
                <div class="rezime-red">
                    <span>Period</span>
                    <strong id="rezimePeriod">Nije unet</strong>
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
                    <small>Cena se računa prema broju dana i cenama izabranih vozila.</small>
                </div>

                <div class="rezime-napomena">
                    <span>ℹ</span>
                    <p>Čuvanje ugovora i svih njegovih stavki izvršava se u jednoj transakciji.</p>
                </div>

                <button class="dugme-siroko" type="submit">Sačuvaj iznajmljivanje</button>
                <a class="dugme dugme-sekundarno dugme-siroko" href="index.php?stranica=iznajmljivanja">Odustani</a>
            </aside>
        </div>
    </form>
</main>
<?php require __DIR__.'/../delovi/podnozje.php'; ?>
