async function ucitajSveTehnologije(filter = '') {
    const teloTabele = document.getElementById('tabelaTehnologija');
    const blokPoruke = document.getElementById('poruka');

    teloTabele.innerHTML = '<tr><td colspan="3" class="text-center">Учитавање...</td></tr>';
    blokPoruke.innerHTML = '';

    try {
        // Састављање URL-а са filter параметром ако је потребно
        let url = 'https://localhost:7015/api/Tehnologije';
        if (filter) {
            url += `/naziv/${encodeURIComponent(filter)}`;
        }

        const odgovor = await fetch(url, {
            method: 'GET',
            headers: {
                'Accept': 'application/json'
            }
        });

        if (!odgovor.ok) {
            blokPoruke.innerHTML = `<div class="alert alert-danger">Грешка: ${odgovor.status}</div>`;
            teloTabele.innerHTML = `<tr><td colspan="3" class="text-center">Нема технологија</td></tr>`;
            return;
        }

        const listaTehnologija = await odgovor.json();

        if (listaTehnologija.length === 0) {
            teloTabele.innerHTML = `<tr><td colspan="3" class="text-center">Нема технологија</td></tr>`;
            return;
        }

        // Чишћење и додавање редова у таблицу
        teloTabele.innerHTML = '';
        listaTehnologija.forEach(tehnologija => {
            const redTabele = document.createElement('tr');

            redTabele.innerHTML = `
                <td>${tehnologija.id}</td>
                <td>${tehnologija.nazivTehnologije}</td>
                <td class="d-flex flex-wrap gap-1">
                    <a class="btn btn-sm btn-warning" href="/TehnologijeAPI/Izmeni?id=${tehnologija.id}">Измени</a>
                    <button class="btn btn-sm btn-danger">Обриши</button>
                </td>
            `;

            // Dugme za brisanje
            const dugmeObrisi = redTabele.querySelector('button.btn-danger');
            dugmeObrisi.addEventListener('click', async () => {
                if (!confirm('Да ли сте сигурни да желите да обришете ову технологију?')) return;

                try {
                    const odgovorBrisanje = await fetch(`https://localhost:7015/api/Tehnologije/${tehnologija.id}`, {
                        method: 'DELETE'
                    });

                    if (odgovorBrisanje.ok) {
                        ucitajSveTehnologije(filter); // Refresh tabele
                    } else {
                        blokPoruke.innerHTML = `<div class="alert alert-danger">Грешка при брисању: ${odgovorBrisanje.status}</div>`;
                    }
                } catch (greskaBrisanje) {
                    blokPoruke.innerHTML = `<div class="alert alert-danger">Грешка при брисању: ${greskaBrisanje}</div>`;
                }
            });

            teloTabele.appendChild(redTabele);
        });

    } catch (greska) {
        blokPoruke.innerHTML = `<div class="alert alert-danger">Грешка: ${greska}</div>`;
        teloTabele.innerHTML = `<tr><td colspan="3" class="text-center">Нема технологија</td></tr>`;
    }
}

// Kacenje na DOM
document.addEventListener('DOMContentLoaded', () => {
    ucitajSveTehnologije();

    // Filter forma submit
    const formaFilter = document.getElementById('filterForma');
    formaFilter.addEventListener('submit', event => {
        event.preventDefault();
        const filterVrednost = document.getElementById('filterInput').value.trim();
        ucitajSveTehnologije(filterVrednost);
    });

    // Dugme za prikazi svih
    const dugmePrikaziSve = document.getElementById('prikaziSve');
    dugmePrikaziSve.addEventListener('click', () => {
        document.getElementById('filterInput').value = '';
        ucitajSveTehnologije();
    });
});