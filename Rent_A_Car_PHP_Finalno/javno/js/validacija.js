function proveriIznajmljivanje(forma){
    const klijent=forma.querySelector('[name="klijentId"]');
    const od=forma.querySelector('[name="datumOd"]').value;
    const doDat=forma.querySelector('[name="datumDo"]').value;
    const vozila=forma.querySelectorAll('input[name="vozila[]"]:checked');
    if(!klijent || !klijent.value){alert('Izaberite klijenta.');return false;}
    if(!od || !doDat){alert('Svi datumi moraju biti popunjeni.');return false;}
    if(new Date(doDat)<new Date(od)){alert('Datum do ne sme biti pre datuma od.');return false;}
    if(vozila.length===0){alert('Izaberite najmanje jedno vozilo.');return false;}
    return true;
}
function proveriKlijenta(forma){
    const ime=forma.ime.value.trim(), prezime=forma.prezime.value.trim(), mejl=forma.mejl.value.trim(), telefon=forma.telefon.value.trim(), dokument=forma.brojDokumenta.value.trim();
    if(!ime||!prezime||!mejl||!telefon||!dokument){alert('Sva polja su obavezna.');return false;}
    if(ime.length<2||prezime.length<2){alert('Ime i prezime moraju imati najmanje 2 karaktera.');return false;}
    if(!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(mejl)){alert('Mejl nije ispravan.');return false;}
    if(!/^[0-9+\-\/ ]{6,20}$/.test(telefon)){alert('Telefon nije u ispravnom formatu.');return false;}
    if(!/^[A-Za-z0-9\-\/]{5,30}$/.test(dokument)){alert('Broj dokumenta nije u ispravnom formatu.');return false;}
    return true;
}

function inicijalizujNovoIznajmljivanje(){
    const forma=document.querySelector('.novo-iznajmljivanje-forma');
    if(!forma) return;

    const klijent=forma.querySelector('[name="klijentId"]');
    const datumOd=forma.querySelector('[name="datumOd"]');
    const datumDo=forma.querySelector('[name="datumDo"]');
    const checkboxovi=[...forma.querySelectorAll('input[name="vozila[]"]')];
    const pretraga=document.getElementById('pretragaVozila');

    const rezimeKlijent=document.getElementById('rezimeKlijent');
    const rezimePeriod=document.getElementById('rezimePeriod');
    const rezimeDani=document.getElementById('rezimeDani');
    const rezimeVozila=document.getElementById('rezimeVozila');
    const rezimeUkupno=document.getElementById('rezimeUkupno');
    const brojIzabranih=document.getElementById('brojIzabranih');

    const formatDatum=(v)=>{
        if(!v) return '';
        const [g,m,d]=v.split('-');
        return `${d}.${m}.${g}.`;
    };

    const brojDana=()=>{
        if(!datumOd.value||!datumDo.value) return 0;
        const od=new Date(datumOd.value+'T00:00:00');
        const doDat=new Date(datumDo.value+'T00:00:00');
        const razlika=Math.floor((doDat-od)/86400000)+1;
        return razlika>0?razlika:0;
    };

    const osvezi=()=>{
        if(klijent && klijent.value){
            rezimeKlijent.textContent=klijent.options[klijent.selectedIndex].text.split(' · ')[0];
        }else if(rezimeKlijent){
            rezimeKlijent.textContent='Nije izabran';
        }

        const dani=brojDana();
        if(rezimePeriod){
            rezimePeriod.textContent=(datumOd.value&&datumDo.value)?`${formatDatum(datumOd.value)} – ${formatDatum(datumDo.value)}`:'Nije unet';
        }
        if(rezimeDani) rezimeDani.textContent=dani;

        const izabrani=checkboxovi.filter(c=>c.checked);
        if(rezimeVozila) rezimeVozila.textContent=izabrani.length;
        if(brojIzabranih) brojIzabranih.textContent=izabrani.length;

        const dnevno=izabrani.reduce((s,c)=>s+(parseFloat(c.dataset.cena)||0),0);
        const ukupno=dani*dnevno;
        if(rezimeUkupno) rezimeUkupno.textContent=new Intl.NumberFormat('sr-RS',{maximumFractionDigits:0}).format(ukupno)+' RSD';
    };

    [klijent,datumOd,datumDo,...checkboxovi].filter(Boolean).forEach(el=>el.addEventListener('change',osvezi));

    if(pretraga){
        pretraga.addEventListener('input',()=>{
            const pojam=pretraga.value.trim().toLocaleLowerCase('sr');
            let vidljivih=0;
            document.querySelectorAll('.vozilo-kartica').forEach(kartica=>{
                const tekst=(kartica.dataset.pretraga||'').toLocaleLowerCase('sr');
                const prikazi=!pojam||tekst.includes(pojam);
                kartica.hidden=!prikazi;
                if(prikazi) vidljivih++;
            });
            const prazno=document.getElementById('nemaVozilaPretraga');
            if(prazno) prazno.hidden=vidljivih!==0;
        });
    }

    osvezi();
}

document.addEventListener('DOMContentLoaded',inicijalizujNovoIznajmljivanje);
