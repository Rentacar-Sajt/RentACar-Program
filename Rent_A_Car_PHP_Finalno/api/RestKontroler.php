<?php

// Kontroler REST API-ja. Vraca podatke u JSON formatu i koristi iste modele kao web deo aplikacije.
require_once __DIR__ . '/../podaci/IznajmljivanjeRepozitorijum.php';
require_once __DIR__ . '/../podaci/KlijentRepozitorijum.php';
require_once __DIR__ . '/../podaci/VoziloRepozitorijum.php';
require_once __DIR__ . '/../validacija/Validacija.php';require_once __DIR__ . '/../model/Iznajmljivanje.php';require_once __DIR__ . '/../model/StavkaIznajmljivanja.php';

class RestKontroler
{
    public function iznajmljivanja(): void
    { RestRuter::odgovor(['podaci'=>(new IznajmljivanjeRepozitorijum())->dajSve()]); }

    public function iznajmljivanje(int $id): void
    {
        $z=(new IznajmljivanjeRepozitorijum())->dajPoId($id);
        if(!$z) { RestRuter::odgovor(['greska'=>'Iznajmljivanje nije pronađeno.'],404); return; }
        RestRuter::odgovor(['podaci'=>$z]);
    }

    public function dodajIznajmljivanje(array $p): void
    {
        $this->proveriIznajmljivanje($p);
        $repo=new IznajmljivanjeRepozitorijum();$k=(new KlijentRepozitorijum())->dajPoId((int)$p['klijentId']);$vRepo=new VoziloRepozitorijum();
        // ASOCIJACIJA: Iznajmljivanje dobija Klijent objekat.
        $obj=new Iznajmljivanje(null,$k,$p['datumOd'],$p['datumDo'],'Aktivno');// KOMPOZICIJA: svako izabrano vozilo postaje StavkaIznajmljivanja objekat unutar iznajmljivanja.
        foreach($p['vozila'] as $vid){$v=$vRepo->dajPoId((int)$vid);if(!$v)throw new Exception('Vozilo nije pronađeno.');$obj->dodajStavku(new StavkaIznajmljivanja(null,$v,$obj->brojDana(),$v->cenaPoDanu,$obj->brojDana()*$v->cenaPoDanu));}$id=$repo->dodaj($obj);
        RestRuter::odgovor(['poruka'=>'Iznajmljivanje je kreirano.','id'=>$id],201);
    }

    public function izmeniIznajmljivanje(int $id,array $p): void
    {
        $repo=new IznajmljivanjeRepozitorijum();
        if(!$repo->dajPoId($id)){RestRuter::odgovor(['greska'=>'Iznajmljivanje nije pronađeno.'],404);return;}
        $this->proveriIznajmljivanje($p);
        $k=(new KlijentRepozitorijum())->dajPoId((int)$p['klijentId']);$vRepo=new VoziloRepozitorijum();
        // ASOCIJACIJA: kod izmene ponovo povezujem iznajmljivanje sa Klijent objektom.
        $obj=new Iznajmljivanje($id,$k,$p['datumOd'],$p['datumDo'],'Aktivno');// KOMPOZICIJA: svako izabrano vozilo postaje StavkaIznajmljivanja objekat unutar iznajmljivanja.
        foreach($p['vozila'] as $vid){$v=$vRepo->dajPoId((int)$vid);if(!$v)throw new Exception('Vozilo nije pronađeno.');$obj->dodajStavku(new StavkaIznajmljivanja(null,$v,$obj->brojDana(),$v->cenaPoDanu,$obj->brojDana()*$v->cenaPoDanu));}$repo->izmeni($obj);
        RestRuter::odgovor(['poruka'=>'Iznajmljivanje je izmenjeno.']);
    }

    public function obrisiIznajmljivanje(int $id): void
    {
        $repo=new IznajmljivanjeRepozitorijum();
        if(!$repo->dajPoId($id)){RestRuter::odgovor(['greska'=>'Iznajmljivanje nije pronađeno.'],404);return;}
        $repo->obrisi($id);
        RestRuter::odgovor(['poruka'=>'Iznajmljivanje je obrisano.']);
    }

    public function klijenti(?int $id): void
    {
        $repo=new KlijentRepozitorijum();
        if($id!==null){$k=$repo->dajPoId($id); if(!$k){RestRuter::odgovor(['greska'=>'Klijent nije pronađen.'],404);return;} RestRuter::odgovor(['podaci'=>$k]);return;}
        RestRuter::odgovor(['podaci'=>$repo->dajSve()]);
    }

    public function vozila(): void
    { RestRuter::odgovor(['podaci'=>(new VoziloRepozitorijum())->dajSlobodna()]); }

    private function proveriIznajmljivanje(array $p): void
    {
        $klijentId=(int)($p['klijentId']??0); $od=(string)($p['datumOd']??''); $do=(string)($p['datumDo']??''); $vozila=$p['vozila']??[];
        if($klijentId<1 || !(new KlijentRepozitorijum())->dajPoId($klijentId)) throw new Exception('Klijent nije ispravan.');
        if($g=Validacija::datumOpseg($od,$do)) throw new Exception($g);
        if(!is_array($vozila) || count($vozila)===0) throw new Exception('Izaberite najmanje jedno vozilo.');
    }
}
