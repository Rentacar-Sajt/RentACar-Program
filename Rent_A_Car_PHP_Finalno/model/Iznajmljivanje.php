<?php

// Model za jedno iznajmljivanje. Ovde povezujem klijenta i sve stavke koje pripadaju tom iznajmljivanju.
require_once __DIR__ . '/Klijent.php';
require_once __DIR__ . '/StavkaIznajmljivanja.php';
class Iznajmljivanje implements ArrayAccess
{
    /** @var StavkaIznajmljivanja[] */
    // Lista stavki koje pripadaju ovom iznajmljivanju (kompozicija).
    public array $stavke=[];
    // ASOCIJACIJA: jedno iznajmljivanje je povezano sa jednim Klijent objektom.
    // Klijent postoji nezavisno od iznajmljivanja, zato ovo nije kompozicija.
    public function __construct(public ?int $id, public Klijent $klijent, public string $datumOd, public string $datumDo, public string $status, public float $ukupnaCena=0) {}
    // KOMPOZICIJA: stavke pripadaju konkretnom iznajmljivanju.
    // Dodavanjem stavke povezujem vozilo sa konkretnim iznajmljivanjem.
    public function dodajStavku(StavkaIznajmljivanja $stavka): void {
        $this->stavke[]=$stavka;
        // Svaka stavka ulazi u ukupnu cenu iznajmljivanja.
        $this->ukupnaCena += $stavka->iznos;
    }
    // Broj dana se koristi za obracun cene svake stavke.
    public function brojDana(): int { return max(1,(int)((strtotime($this->datumDo)-strtotime($this->datumOd))/86400)+1); }
    public function offsetExists(mixed $o): bool { return array_key_exists($o,$this->toArray()); }
    public function offsetGet(mixed $o): mixed { $a=$this->toArray(); return $a[$o]??null; }
    public function offsetSet(mixed $o,mixed $v): void {}
    public function offsetUnset(mixed $o): void {}
    public function toArray(): array { return ["ID"=>$this->id,"KlijentID"=>$this->klijent->id,"Ime"=>$this->klijent->ime,"Prezime"=>$this->klijent->prezime,"Mejl"=>$this->klijent->mejl,"Telefon"=>$this->klijent->telefon,"BrojDokumenta"=>$this->klijent->brojDokumenta,"DatumOd"=>$this->datumOd,"DatumDo"=>$this->datumDo,"Status"=>$this->status,"UkupnaCena"=>$this->ukupnaCena,"Stavke"=>array_map(fn($s)=>$s->toArray(),$this->stavke)]; }
}
