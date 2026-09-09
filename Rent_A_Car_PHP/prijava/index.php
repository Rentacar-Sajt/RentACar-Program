<?php
session_start();
require_once __DIR__ . '/../podaci/KorisnikRepozitorijum.php';
require_once __DIR__ . '/../validacija/Validacija.php';
$greska='';
if($_SERVER['REQUEST_METHOD']==='POST'){
    $mejl=trim($_POST['mejl']??''); $lozinka=$_POST['lozinka']??'';
    $greska=Validacija::mejl($mejl)??'';
    if(!$greska){
        $repo=new KorisnikRepozitorijum(); $k=$repo->prijava($mejl,$lozinka);
        if($k){$_SESSION['korisnik']=$k; header('Location: ../iznajmljivanja/index.php'); exit;}
        $greska='Pogrešan mejl ili lozinka.';
    }
}
?>
<!doctype html><html lang="sr"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1"><title>Prijava | Rent-a-Car</title><link rel="stylesheet" href="../javno/css/stil.css"></head><body class="prijava-stranica">
<main class="prijava-kartica"><div class="prijava-logo">R</div><h1>Dobro došli</h1><p class="podnaslov">Prijavite se za pristup Rent-a-Car sistemu.</p>
<?php if($greska):?><div class="greska"><?=htmlspecialchars($greska)?></div><?php endif;?>
<form method="post"><div class="forma-polje"><label>Mejl adresa</label><input type="email" name="mejl" placeholder="ime@primer.com" autocomplete="email" required maxlength="150"></div>
<div class="forma-polje"><label>Lozinka</label><input type="password" name="lozinka" placeholder="Unesite lozinku" autocomplete="current-password" required maxlength="100"></div><button type="submit">Prijavi se</button></form>
<div class="prijava-napomena">Rent-a-Car sistem za upravljanje iznajmljivanjima vozila</div></main></body></html>