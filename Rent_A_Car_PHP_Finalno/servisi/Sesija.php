<?php

// Pomocna klasa za rad sa PHP sesijom i prijavljenim korisnikom.
class Sesija
{
    public static function pokreni(): void { if(session_status()!==PHP_SESSION_ACTIVE) session_start(); }
    public static function prijavljen(): bool { self::pokreni(); return !empty($_SESSION['korisnik']); }
    // Stranice koje traze prijavu pozivaju ovu proveru.
    public static function zahtevajPrijavu(): void { if(!self::prijavljen()){header('Location: index.php?stranica=prijava');exit;} }
    public static function korisnik(): ?array { self::pokreni(); return $_SESSION['korisnik']??null; }
    public static function jeAdmin(): bool { $k=self::korisnik(); return ($k['Uloga']??'')==='ADMIN'; }
    public static function zahtevajAdmina(): void { self::zahtevajPrijavu(); if(!self::jeAdmin()){http_response_code(403); die('Nemate dozvolu za ovu akciju. Potrebna je administratorska uloga.');} }
    // Posle uspesne prijave cuvam korisnika u sesiji.
    public static function prijavi(array $korisnik): void { self::pokreni();session_regenerate_id(true);unset($korisnik['Lozinka']);$_SESSION['korisnik']=$korisnik; }
    public static function osveziKorisnika(array $korisnik): void { self::pokreni(); unset($korisnik['Lozinka']); $_SESSION['korisnik']=$korisnik; }
    public static function odjavi(): void { self::pokreni();$_SESSION=[];if(ini_get('session.use_cookies')){$p=session_get_cookie_params();setcookie(session_name(),'',time()-42000,$p['path'],$p['domain'],$p['secure'],$p['httponly']);}session_destroy(); }
    public static function postaviPoruku(string $tekst,string $tip='uspeh'): void { self::pokreni();$_SESSION['flash_poruka']=['tekst'=>$tekst,'tip'=>$tip]; }
    public static function uzmiPoruku(): ?array { self::pokreni();$p=$_SESSION['flash_poruka']??null;unset($_SESSION['flash_poruka']);return $p; }
}
