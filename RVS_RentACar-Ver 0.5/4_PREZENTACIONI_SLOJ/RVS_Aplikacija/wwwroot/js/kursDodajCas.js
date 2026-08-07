function dodajCas() {
    const template = document.getElementById("casTemplate");
    const container = document.getElementById("casoviContainer");

    if (!template || !container) return;

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
        // Редни број
        const input = cas.querySelector(".redniBrojInput");
        if (input) {
            input.value = index + 1;
            input.name = `Casovi[${index}].RedniBrojCasa`;
        }

        // Технологија (select)
        const select = cas.querySelector("select");
        if (select) {
            select.name = `Casovi[${index}].TehnologijaObjekat.Id`;
        }
    });
}