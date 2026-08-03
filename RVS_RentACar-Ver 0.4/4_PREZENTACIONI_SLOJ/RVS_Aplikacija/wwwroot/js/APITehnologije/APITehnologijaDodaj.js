// Selektovanje forme i input polja
const formaDodaj = document.getElementById('formaDodaj');
const inputNaziv = document.getElementById('nazivTehnologije');

// Div za prikaz poruka
const divPoruka = document.createElement('div');
formaDodaj.prepend(divPoruka);

// Funkcija koja salje POST zahtev na REST API
async function dodajTehnologiju(event) {
    console.log("Submit clicked"); // Debug

    event.preventDefault();
    divPoruka.innerHTML = '';

    const nazivTehnologije = inputNaziv.value.trim();
    if (!nazivTehnologije) {
        divPoruka.innerHTML = `<div class="alert alert-warning">Unesite naziv tehnologije.</div>`;
        return;
    }

    try {
        const odgovor = await fetch('https://localhost:7015/api/Tehnologije', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'Accept': 'application/json' },
            body: JSON.stringify({ NazivTehnologije: nazivTehnologije }) // TAČNO ime svojstva
        });

        if (odgovor.ok) {
            divPoruka.innerHTML = `<div class="alert alert-success">Технологија је успешно додата!</div>`;
            inputNaziv.value = '';
        } else {
            let greskaTekst = '';
            try {
                greskaTekst = JSON.stringify(await odgovor.json());
            } catch {
                greskaTekst = await odgovor.text();
            }
            divPoruka.innerHTML = `<div class="alert alert-danger">Грешка: ${greskaTekst || odgovor.status}</div>`;
        }

    } catch (greska) {
        divPoruka.innerHTML = `<div class="alert alert-danger">Грешка: ${greska}</div>`;
    }
}

formaDodaj.addEventListener('submit', dodajTehnologiju);