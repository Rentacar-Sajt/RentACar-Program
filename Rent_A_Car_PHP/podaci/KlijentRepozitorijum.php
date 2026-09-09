<?php
require_once __DIR__ . '/BazniRepozitorijum.php';

class KlijentRepozitorijum extends BazniRepozitorijum
{
    public function dajSve(string $filter = ''): array
    {
        if ($filter === '') {
            return $this->izvrsiUpit('SELECT * FROM dbo.Klijenti ORDER BY Prezime, Ime');
        }
        $f = "%$filter%";
        return $this->izvrsiUpit(
            'SELECT * FROM dbo.Klijenti WHERE Ime LIKE ? OR Prezime LIKE ? OR Mejl LIKE ? OR Telefon LIKE ? OR BrojDokumenta LIKE ? ORDER BY Prezime, Ime',
            [$f, $f, $f, $f, $f]
        );
    }

    public function dajPoId(int $id): ?array
    {
        $r = $this->izvrsiUpit('SELECT * FROM dbo.Klijenti WHERE ID=?', [$id]);
        return $r[0] ?? null;
    }

    public function postojiMejl(string $mejl, ?int $izuzmiId = null): bool
    {
        $sql = 'SELECT ID FROM dbo.Klijenti WHERE Mejl=?';
        $p = [$mejl];
        if ($izuzmiId !== null) { $sql .= ' AND ID<>?'; $p[] = $izuzmiId; }
        return count($this->izvrsiUpit($sql, $p)) > 0;
    }

    public function postojiDokument(string $broj, ?int $izuzmiId = null): bool
    {
        $sql = 'SELECT ID FROM dbo.Klijenti WHERE BrojDokumenta=?';
        $p = [$broj];
        if ($izuzmiId !== null) { $sql .= ' AND ID<>?'; $p[] = $izuzmiId; }
        return count($this->izvrsiUpit($sql, $p)) > 0;
    }

    public function dodaj(string $ime, string $prezime, string $mejl, string $telefon, string $brojDokumenta): int
    {
        $q = $this->konekcija->prepare('INSERT INTO dbo.Klijenti(Ime,Prezime,Mejl,Telefon,BrojDokumenta) OUTPUT INSERTED.ID VALUES(?,?,?,?,?)');
        $q->execute([$ime,$prezime,$mejl,$telefon,$brojDokumenta]);
        return (int)$q->fetchColumn();
    }

    public function izmeni(int $id, string $ime, string $prezime, string $mejl, string $telefon, string $brojDokumenta): void
    {
        $this->izvrsiKomandu('UPDATE dbo.Klijenti SET Ime=?,Prezime=?,Mejl=?,Telefon=?,BrojDokumenta=? WHERE ID=?', [$ime,$prezime,$mejl,$telefon,$brojDokumenta,$id]);
    }

    public function obrisi(int $id): void
    {
        $this->izvrsiKomandu('DELETE FROM dbo.Klijenti WHERE ID=?', [$id]);
    }
}
