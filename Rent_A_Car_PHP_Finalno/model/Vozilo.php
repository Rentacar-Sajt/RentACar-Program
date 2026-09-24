<?php

// Model Vozilo predstavlja jedno vozilo iz rent-a-car baze.
class Vozilo implements ArrayAccess
{
    public function __construct(
        public int $id, public string $marka, public string $model, public string $registracija,
        public int $godiste, public string $gorivo, public string $menjac, public int $brojSedista,
        public int $kilometraza, public string $kategorija, public float $cenaPoDanu=0, public string $status='Slobodno'
    ) {}
    public function offsetExists(mixed $o): bool { return array_key_exists($o,$this->toArray()); }
    public function offsetGet(mixed $o): mixed { $a=$this->toArray(); return $a[$o]??null; }
    public function offsetSet(mixed $o, mixed $v): void { $map=["ID"=>"id","Marka"=>"marka","Model"=>"model","Registracija"=>"registracija","Godiste"=>"godiste","Gorivo"=>"gorivo","Menjac"=>"menjac","BrojSedista"=>"brojSedista","Kilometraza"=>"kilometraza","Kategorija"=>"kategorija","CenaPoDanu"=>"cenaPoDanu","Status"=>"status"]; if(isset($map[$o])) $this->{$map[$o]}=$v; }
    public function offsetUnset(mixed $o): void {}
    public function toArray(): array { return ["ID"=>$this->id,"Marka"=>$this->marka,"Model"=>$this->model,"Registracija"=>$this->registracija,"Godiste"=>$this->godiste,"Gorivo"=>$this->gorivo,"Menjac"=>$this->menjac,"BrojSedista"=>$this->brojSedista,"Kilometraza"=>$this->kilometraza,"Kategorija"=>$this->kategorija,"CenaPoDanu"=>$this->cenaPoDanu,"Status"=>$this->status]; }
}
