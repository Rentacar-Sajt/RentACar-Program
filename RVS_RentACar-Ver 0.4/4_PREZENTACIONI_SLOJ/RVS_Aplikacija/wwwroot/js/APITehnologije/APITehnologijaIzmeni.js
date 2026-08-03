// Selektovanje elemenata
const formaIzmeni = document.getElementById('formaIzmeni');
const inputNaziv = document.getElementById('nazivTehnologije');
const inputId = document.getElementById('tehnologijaId');
const divPoruka = document.getElementById('divPoruka');

// Prvo se preuzmu podaci REST GET po ID-u
async function ucitajTehnologiju() {
    const id = inputId.value;
    try {
        const odgovor = await fetch(`https://localhost:7015/api/Tehnologije/${id}`);
        if (odgovor.ok) {
            const data = await odgovor.json();
            inputNaziv.value = data.nazivTehnologije || data.NazivTehnologije; // case ovisno
        } else {
            divPoruka.innerHTML = `<div class="alert alert-danger">Грешка при учитавању технологије</div>`;
        }
    } catch (err) {
        divPoruka.innerHTML = `<div class="alert alert-danger">Грешка: ${err}</div>`;
    }
}

// Funkcija za submit izmene
async function izmeniTehnologiju(event) {
    event.preventDefault();
    divPoruka.innerHTML = '';

    const id = inputId.value;
    const naziv = inputNaziv.value.trim();

    if (!naziv) {
        divPoruka.innerHTML = `<div class="alert alert-warning">Unesite naziv tehnologije.</div>`;
        return;
    }

    try {
        const odgovor = await fetch(`https://localhost:7015/api/Tehnologije/${id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json', 'Accept': 'application/json' },
            body: JSON.stringify({ Id: parseInt(id), NazivTehnologije: naziv })
        });

        if (odgovor.ok) {
            divPoruka.innerHTML = `<div class="alert alert-success">Технологија је успешно измењена!</div>`;
        } else {
            let greskaTekst = '';
            try {
                greskaTekst = JSON.stringify(await odgovor.json());
            } catch {
                greskaTekst = await odgovor.text();
            }
            divPoruka.innerHTML = `<div class="alert alert-danger">Грешка: ${greskaTekst || odgovor.status}</div>`;
        }

    } catch (err) {
        divPoruka.innerHTML = `<div class="alert alert-danger">Грешка: ${err}</div>`;
    }
}

// Event listener
formaIzmeni.addEventListener('submit', izmeniTehnologiju);

window.addEventListener('DOMContentLoaded', ucitajTehnologiju);