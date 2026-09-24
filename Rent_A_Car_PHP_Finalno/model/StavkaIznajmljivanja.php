<?php

// Stavka iznajmljivanja predstavlja jedno vozilo unutar konkretnog iznajmljivanja.
require_once __DIR__ . '/Vozilo.php';
class StavkaIznajmljivanja implements ArrayAccess
{
    // Stavka ima jedno konkretno vozilo, broj dana i cenu.
    // Ova stavka je deo kompozicije iznajmljivanja i u sebi cuva konkretno Vozilo.
    public function __construct(public ?int $id, public Vozilo $vozilo, public int $brojDana, public float $cenaPoDanu, public float $iznos) {}
    public function offsetExists(mixed $o): bool { return array_key_exists($o,$this->toArray()); }
    public function offsetGet(mixed $o): mixed { $a=$this->toArray(); return $a[$o]??null; }
    public function offsetSet(mixed $o,mixed $v): void {}
    public function offsetUnset(mixed $o): void {}
    public function toArray(): array { return ["ID"=>$this->id,"VoziloID"=>$this->vozilo->id,"BrojDana"=>$this->brojDana,"CenaPoDanu"=>$this->cenaPoDanu,"Iznos"=>$this->iznos,"Marka"=>$this->vozilo->marka,"Model"=>$this->vozilo->model,"Registracija"=>$this->vozilo->registracija,"Godiste"=>$this->vozilo->godiste,"Gorivo"=>$this->vozilo->gorivo,"Menjac"=>$this->vozilo->menjac,"Kategorija"=>$this->vozilo->kategorija]; }
}
