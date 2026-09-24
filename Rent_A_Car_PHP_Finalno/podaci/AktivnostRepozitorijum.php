<?php

// Ovde se cuva evidencija akcija koje korisnici rade u sistemu.
require_once __DIR__ . '/BazniRepozitorijum.php';
class AktivnostRepozitorijum extends BazniRepozitorijum
{
    public function evidentiraj(string $mejl,string $akcija,string $entitet,?int $id=null,string $opis=''): void
    { $this->izvrsiKomandu('INSERT INTO dbo.Aktivnosti(KorisnikMejl,Akcija,Entitet,EntitetID,Opis) VALUES(?,?,?,?,?)',[$mejl,$akcija,$entitet,$id,$opis]); }
    public function dajSve(int $limit=100): array
    { $limit=max(1,min(500,$limit)); return $this->izvrsiUpit("SELECT TOP $limit * FROM dbo.Aktivnosti ORDER BY DatumVreme DESC,ID DESC"); }
}
