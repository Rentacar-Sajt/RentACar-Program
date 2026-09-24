<?php

// Kontroler za prijavu i odjavu korisnika.
require_once __DIR__ . '/../servisi/Sesija.php';
require_once __DIR__ . '/../podaci/KorisnikRepozitorijum.php';
require_once __DIR__ . '/../validacija/Validacija.php';

class PrijavaKontroler
{
    public function prijava(): void
    {
        if (Sesija::prijavljen()) { header('Location: index.php?stranica=pocetna'); exit; }
        $greska=''; $mejl='';
        if ($_SERVER['REQUEST_METHOD']==='POST') {
            $mejl=trim($_POST['mejl']??''); $lozinka=$_POST['lozinka']??'';
            $greska=Validacija::obavezno($mejl,'Mejl') ?? Validacija::mejl($mejl) ?? Validacija::obavezno($lozinka,'Lozinka') ?? '';
            if (!$greska) {
                $k=(new KorisnikRepozitorijum())->prijava($mejl,$lozinka);
                if ($k) { Sesija::prijavi($k); header('Location: index.php?stranica=pocetna'); exit; }
                $greska='Pogrešan mejl ili lozinka.';
            }
        }
        require __DIR__ . '/../pogledi/prijava/index.php';
    }

    public function odjava(): void
    {
        Sesija::odjavi();
        header('Location: index.php?stranica=prijava');
        exit;
    }
}
