<?php
class Validacija
{
    public static function obavezno(?string $vrednost, string $naziv): ?string
    { return trim((string)$vrednost)==='' ? "$naziv je obavezno polje." : null; }

    public static function duzina(string $vrednost, string $naziv, int $min, int $max): ?string
    {
        $d=mb_strlen(trim($vrednost));
        return ($d<$min || $d>$max) ? "$naziv mora imati između $min i $max karaktera." : null;
    }

    public static function mejl(string $mejl): ?string
    { return filter_var($mejl,FILTER_VALIDATE_EMAIL) ? null : 'Mejl adresa nije ispravna.'; }

    public static function telefon(string $telefon): ?string
    { return preg_match('/^[0-9+\-\/ ]{6,20}$/',$telefon) ? null : 'Telefon nije u ispravnom formatu.'; }

    public static function dokument(string $broj): ?string
    { return preg_match('/^[A-Za-z0-9\-\/]{5,30}$/',$broj) ? null : 'Broj dokumenta nije u ispravnom formatu.'; }

    public static function datumOpseg(string $od,string $do): ?string
    {
        if(!$od||!$do) return 'Datumi su obavezni.';
        if(!strtotime($od)||!strtotime($do)) return 'Datum nije ispravnog tipa.';
        return strtotime($do)<strtotime($od) ? 'Datum do ne sme biti pre datuma od.' : null;
    }
}
