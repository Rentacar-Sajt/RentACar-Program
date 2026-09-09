<?php
require_once __DIR__ . '/Klijent.php';
require_once __DIR__ . '/StavkaIznajmljivanja.php';

class Iznajmljivanje
{
    /** @var StavkaIznajmljivanja[] */
    public array $stavke = [];

    public function __construct(
        public ?int $id,
        public Klijent $klijent,
        public string $datumOd,
        public string $datumDo,
        public string $status,
        public float $ukupnaCena = 0
    ) {}

    // Kompozicija: celina Iznajmljivanje sadrži listu svojih delova - stavki.
    public function dodajStavku(StavkaIznajmljivanja $stavka): void
    {
        $this->stavke[] = $stavka;
    }
}
