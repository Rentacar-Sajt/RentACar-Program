<?php

// Model Klijent predstavlja podatke o jednom klijentu.
class Klijent implements ArrayAccess
{
    public function __construct(
        public ?int $id, public string $ime, public string $prezime,
        public string $mejl, public string $telefon, public string $brojDokumenta
    ) {}
    public function offsetExists(mixed $o): bool { return in_array($o, ["ID","Ime","Prezime","Mejl","Telefon","BrojDokumenta","id","ime","prezime","mejl","telefon","brojDokumenta"], true); }
    public function offsetGet(mixed $o): mixed { return match($o) { "ID","id"=>$this->id, "Ime","ime"=>$this->ime, "Prezime","prezime"=>$this->prezime, "Mejl","mejl"=>$this->mejl, "Telefon","telefon"=>$this->telefon, "BrojDokumenta","brojDokumenta"=>$this->brojDokumenta, default=>null }; }
    public function offsetSet(mixed $o, mixed $v): void { $map=["ID"=>"id","Ime"=>"ime","Prezime"=>"prezime","Mejl"=>"mejl","Telefon"=>"telefon","BrojDokumenta"=>"brojDokumenta"]; if(isset($map[$o])) $this->{$map[$o]}=$v; }
    public function offsetUnset(mixed $o): void {}
    public function toArray(): array { return ["ID"=>$this->id,"Ime"=>$this->ime,"Prezime"=>$this->prezime,"Mejl"=>$this->mejl,"Telefon"=>$this->telefon,"BrojDokumenta"=>$this->brojDokumenta]; }
}
