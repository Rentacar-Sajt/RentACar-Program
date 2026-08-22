/* ==========================================================================
   OBJAŠNJENJE ZA ODBRANU PROJEKTA
   JavaScript obezbeđuje ponašanje stranice u pregledaču: validaciju, dinamičko računanje i/ili komunikaciju sa API-jem.
   Klijentski kod poboljšava korisničko iskustvo, ali konačne poslovne provere
   treba potvrditi i na serveru jer se kod u pregledaču može menjati.
   ========================================================================== */

function dodajCas() {

    const template = document.getElementById("casTemplate");
    const container = document.getElementById("casoviContainer");

    const klon = template.content.cloneNode(true);

    container.appendChild(klon);

    azurirajCasove();
}

function obrisiCas(dugme) {

    dugme.closest(".cas-item").remove();

    azurirajCasove();
}

function azurirajCasove() {

    const svi = document.querySelectorAll("#casoviContainer .cas-item");

    svi.forEach((cas, index) => {

        const input = cas.querySelector(".redniBrojInput");
        input.value = index + 1;

        input.name = `Casovi[${index}].RedniBrojCasa`;

        const select = cas.querySelector("select");
        select.name = `Casovi[${index}].TehnologijaObjekat.Id`;
    });
}