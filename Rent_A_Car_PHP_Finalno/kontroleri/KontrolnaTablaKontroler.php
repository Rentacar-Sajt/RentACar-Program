<?php

// Kontroler pocetne stranice sa osnovnom statistikom sistema.
require_once __DIR__ . '/../servisi/Sesija.php';
require_once __DIR__ . '/../podaci/IznajmljivanjeRepozitorijum.php';
require_once __DIR__ . '/../podaci/VoziloRepozitorijum.php';
require_once __DIR__ . '/../podaci/KlijentRepozitorijum.php';

class KontrolnaTablaKontroler
{
    public function index(): void
    {
        Sesija::zahtevajPrijavu();
        $iRepo=new IznajmljivanjeRepozitorijum();
        $vRepo=new VoziloRepozitorijum();
        $kRepo=new KlijentRepozitorijum();
        $statistika=$iRepo->dajStatistiku();
        $vozilaStat=$vRepo->dajStatistiku();
        $brojKlijenata=count($kRepo->dajSve());
        $poslednja=$iRepo->dajPoslednja(5);
        $poruka=Sesija::uzmiPoruku();
        $korisnik=Sesija::korisnik();
        require __DIR__ . '/../pogledi/kontrolna_tabla/index.php';
    }
}
