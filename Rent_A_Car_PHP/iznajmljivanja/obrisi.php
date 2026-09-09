<?php
require_once __DIR__.'/../servisi/Sesija.php';
Sesija::zahtevajPrijavu();
require_once __DIR__.'/../podaci/IznajmljivanjeRepozitorijum.php';
$id=(int)($_GET['id']??0);
if($id){
    try {
        (new IznajmljivanjeRepozitorijum())->obrisi($id);
        Sesija::postaviPoruku('Iznajmljivanje #'.$id.' je uspešno obrisano.');
    } catch(Throwable $e) {
        Sesija::postaviPoruku('Brisanje nije uspelo: '.$e->getMessage(), 'greska');
    }
}
header('Location:index.php');
