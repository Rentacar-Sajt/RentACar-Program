<?php

// Ova klasa pravi konekciju sa SQL Server bazom.
class BazaPodataka
{
    protected PDO $konekcija;

    public function __construct()
    {
        // Ucitavam podesavanja da ne bih upisivao podatke za bazu na vise mesta.
        $podesavanja = require __DIR__ . '/../konfiguracija/podesavanja.php';
        $dsn = 'sqlsrv:Server=' . $podesavanja['server'] . ';Database=' . $podesavanja['baza'] . ';TrustServerCertificate=1';

        // PDO_SQLSRV koristi ove podatke da se poveze na SQL Server.
        $this->konekcija = new PDO(
            $dsn,
            $podesavanja['korisnik'] !== '' ? $podesavanja['korisnik'] : null,
            $podesavanja['lozinka'] !== '' ? $podesavanja['lozinka'] : null,
            [PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION]
        );
    }

    public function dajKonekciju(): PDO
    {
        return $this->konekcija;
    }
}
