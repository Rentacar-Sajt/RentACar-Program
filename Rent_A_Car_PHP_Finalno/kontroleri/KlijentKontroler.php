<?php

// Kontroler koji prima zahteve za klijente i poziva odgovarajuci repozitorijum.
require_once __DIR__ . '/../servisi/Sesija.php';
require_once __DIR__ . '/../podaci/KlijentRepozitorijum.php';
require_once __DIR__ . '/../validacija/Validacija.php';
require_once __DIR__ . '/../podaci/AktivnostRepozitorijum.php';

class KlijentKontroler
{
    private function validiraj(array $p, KlijentRepozitorijum $repo, ?int $id=null): ?string
    {
        $ime=trim($p['ime']??''); $prezime=trim($p['prezime']??''); $mejl=trim($p['mejl']??''); $telefon=trim($p['telefon']??''); $dok=trim($p['brojDokumenta']??'');
        foreach ([['Ime',$ime],['Prezime',$prezime],['Mejl',$mejl],['Telefon',$telefon],['Broj dokumenta',$dok]] as [$n,$v]) { if($g=Validacija::obavezno($v,$n)) return $g; }
        if($g=Validacija::duzina($ime,'Ime',2,50)) return $g;
        if($g=Validacija::duzina($prezime,'Prezime',2,50)) return $g;
        if($g=Validacija::mejl($mejl)) return $g;
        if($g=Validacija::telefon($telefon)) return $g;
        if($g=Validacija::dokument($dok)) return $g;
        if($repo->postojiMejl($mejl,$id)) return 'Klijent sa tim mejlom već postoji.';
        if($repo->postojiDokument($dok,$id)) return 'Broj dokumenta mora biti jedinstven.';
        return null;
    }

    public function index(): void
    {
        Sesija::zahtevajPrijavu(); $filter=trim($_GET['filter']??''); $svi=(new KlijentRepozitorijum())->dajSve($filter); $str=max(1,(int)($_GET['str']??1)); $po=10; $ukupno=count($svi); $brojStr=max(1,(int)ceil($ukupno/$po)); $str=min($str,$brojStr); $redovi=array_slice($svi,($str-1)*$po,$po); $poruka=Sesija::uzmiPoruku();
        require __DIR__ . '/../pogledi/klijenti/index.php';
    }

    public function dodaj(): void
    {
        Sesija::zahtevajPrijavu(); $repo=new KlijentRepozitorijum(); $greska='';
        if($_SERVER['REQUEST_METHOD']==='POST'){
            $greska=$this->validiraj($_POST,$repo)??'';
            if(!$greska){$noviId=$repo->dodaj(trim($_POST['ime']),trim($_POST['prezime']),trim($_POST['mejl']),trim($_POST['telefon']),trim($_POST['brojDokumenta'])); (new AktivnostRepozitorijum())->evidentiraj(Sesija::korisnik()['Mejl']??'sistem','Dodavanje','Klijent',$noviId,trim($_POST['ime'].' '.$_POST['prezime'])); Sesija::postaviPoruku('Klijent je uspešno dodat.'); header('Location: index.php?stranica=klijenti'); exit;}
        }
        $klijent=$_POST; require __DIR__ . '/../pogledi/klijenti/forma.php';
    }

    public function izmeni(): void
    {
        Sesija::zahtevajPrijavu(); $id=(int)($_GET['id']??0); $repo=new KlijentRepozitorijum(); $klijent=$repo->dajPoId($id); if(!$klijent){http_response_code(404);die('Klijent nije pronađen.');} $greska='';
        if($_SERVER['REQUEST_METHOD']==='POST'){
            $greska=$this->validiraj($_POST,$repo,$id)??'';
            if(!$greska){$repo->izmeni($id,trim($_POST['ime']),trim($_POST['prezime']),trim($_POST['mejl']),trim($_POST['telefon']),trim($_POST['brojDokumenta'])); (new AktivnostRepozitorijum())->evidentiraj(Sesija::korisnik()['Mejl']??'sistem','Izmena','Klijent',$id,trim($_POST['ime'].' '.$_POST['prezime'])); Sesija::postaviPoruku('Podaci klijenta su izmenjeni.'); header('Location: index.php?stranica=klijenti'); exit;}
            $klijent = array_merge($klijent->toArray(), $_POST);
        }
        require __DIR__ . '/../pogledi/klijenti/forma.php';
    }

    public function obrisi(): void
    {
        Sesija::zahtevajPrijavu(); if($_SERVER['REQUEST_METHOD']!=='POST'){http_response_code(405);die('Nedozvoljena metoda.');}
        try{$brisanjeId=(int)($_POST['id']??0);(new KlijentRepozitorijum())->obrisi($brisanjeId);(new AktivnostRepozitorijum())->evidentiraj(Sesija::korisnik()['Mejl']??'sistem','Brisanje','Klijent',$brisanjeId,'Obrisan klijent'); Sesija::postaviPoruku('Klijent je obrisan.');}
        catch(Throwable $e){Sesija::postaviPoruku('Klijent se ne može obrisati dok ima evidentirana iznajmljivanja.','greska');}
        header('Location: index.php?stranica=klijenti'); exit;
    }
}
