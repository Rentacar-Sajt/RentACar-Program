/* ==========================================================================
   OBJAŠNJENJE ZA ODBRANU PROJEKTA
   JavaScript obezbeđuje ponašanje stranice u pregledaču: validaciju, dinamičko računanje i/ili komunikaciju sa API-jem.
   Klijentski kod poboljšava korisničko iskustvo, ali konačne poslovne provere
   treba potvrditi i na serveru jer se kod u pregledaču može menjati.
   ========================================================================== */

document.addEventListener("DOMContentLoaded", function () {
    const forma = document.querySelector("form[asp-action='Dodaj']");
    if (!forma) return;

    forma.addEventListener("submit", function (e) {
        let validno = true;

        // Provera NazivKursa
        const nazivInput = forma.querySelector("[name='NazivKursa']");
        if (!nazivInput.value.trim()) {
            alert("Polje 'Naziv kursa' mora biti popunjeno!");
            nazivInput.focus();
            validno = false;
            e.preventDefault();
            return false;
        }

        // Provera OpisKursa
        const opisInput = forma.querySelector("[name='OpisKursa']");
        if (!opisInput.value.trim()) {
            alert("Polje 'Opis kursa' mora biti popunjeno!");
            opisInput.focus();
            validno = false;
            e.preventDefault();
            return false;
        }

        // Provera TehnologijaId
        const tehnologijaSelect = forma.querySelector("[name='TehnologijaId']");
        if (!tehnologijaSelect.value) {
            alert("Morate odabrati tehnologiju!");
            tehnologijaSelect.focus();
            validno = false;
            e.preventDefault();
            return false;
        }

        return validno;
    });
});
