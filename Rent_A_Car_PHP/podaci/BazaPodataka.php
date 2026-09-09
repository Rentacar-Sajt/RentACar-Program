<?php
class BazaPodataka
{
    protected PDO $konekcija;

    public function __construct()
    {
        $podesavanja = require __DIR__ . '/../konfiguracija/podesavanja.php';
        $dsn = 'sqlsrv:Server=' . $podesavanja['server'] . ';Database=' . $podesavanja['baza'] . ';TrustServerCertificate=1';

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
