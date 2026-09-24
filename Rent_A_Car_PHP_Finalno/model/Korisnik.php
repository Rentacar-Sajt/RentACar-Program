<?php

// Model Korisnik cuva podatke o korisniku koji se prijavljuje u sistem.
class Korisnik implements ArrayAccess
{
 public function __construct(public ?int $id,public string $ime,public string $prezime,public string $mejl,public string $lozinka,public string $uloga='RADNIK',public bool $aktivan=true) {}
 public function offsetExists(mixed $o):bool{return array_key_exists($o,$this->toArray());}
 public function offsetGet(mixed $o):mixed{$a=$this->toArray();return $a[$o]??null;}
 public function offsetSet(mixed $o,mixed $v):void{} public function offsetUnset(mixed $o):void{}
 public function toArray():array{return ['ID'=>$this->id,'Ime'=>$this->ime,'Prezime'=>$this->prezime,'Mejl'=>$this->mejl,'Lozinka'=>$this->lozinka,'Uloga'=>$this->uloga,'Aktivan'=>$this->aktivan?1:0];}
}
