<?php
class Klijent
{
    public function __construct(
        public ?int $id,
        public string $ime,
        public string $prezime,
        public string $mejl,
        public string $telefon,
        public string $brojDokumenta
    ) {}
}
