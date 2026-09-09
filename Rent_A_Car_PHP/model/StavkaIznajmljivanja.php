<?php
require_once __DIR__ . '/Vozilo.php';

class StavkaIznajmljivanja
{
    public function __construct(
        public ?int $id,
        public Vozilo $vozilo,
        public int $brojDana,
        public float $cenaPoDanu,
        public float $iznos
    ) {}
}
