<?php
require_once __DIR__ . '/BazaPodataka.php';

abstract class BazniRepozitorijum extends BazaPodataka
{
    protected function izvrsiUpit(string $sql, array $parametri = []): array
    {
        $komanda = $this->konekcija->prepare($sql);
        $komanda->execute($parametri);
        return $komanda->fetchAll(PDO::FETCH_ASSOC);
    }

    protected function izvrsiKomandu(string $sql, array $parametri = []): int
    {
        $komanda = $this->konekcija->prepare($sql);
        $komanda->execute($parametri);
        return $komanda->rowCount();
    }

    protected function izvrsiProceduru(string $naziv, array $parametri = []): array
    {
        $mesta = implode(', ', array_fill(0, count($parametri), '?'));
        $sql = 'EXEC ' . $naziv . ($mesta !== '' ? ' ' . $mesta : '');
        $komanda = $this->konekcija->prepare($sql);
        $komanda->execute(array_values($parametri));
        return $komanda->columnCount() > 0 ? $komanda->fetchAll(PDO::FETCH_ASSOC) : [];
    }

    protected function procitajPogled(string $nazivPogleda, string $uslov = '1=1', array $parametri = []): array
    {
        return $this->izvrsiUpit("SELECT * FROM dbo.$nazivPogleda WHERE $uslov", $parametri);
    }
}
