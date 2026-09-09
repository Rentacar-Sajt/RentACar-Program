<?php
class Korisnik
{
    public function __construct(
        public ?int $id,
        public string $ime,
        public string $prezime,
        public string $mejl,
        public string $lozinka,
        public string $uloga = 'RADNIK'
    ) {}
}
