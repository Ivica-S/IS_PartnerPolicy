
import { showToast } from './toast.js';
$(document).ready(function () {
    $('#savePolicyButton').click(async function () {
        // Dohvati PartnerId iz data-atributa HTML elementa
        var partnerId = $('#addPolicyModal').data('partner-id');
        var policyNumber = $('#PolicyNumber').val();
        var amount = $('#Amount').val();

        if (policyNumber && amount) {
            if (amount <= 0) {
                showToast('Iznos mora biti veći od 0.');
            } 
            try {
                const response = await fetch(addPolicyUrl, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        partnerId: partnerId,
                        policyNumber: policyNumber,
                        amount: amount
                    })
                });

                const data = await response.json();

                if (data.success) {
                    // Zatvori modal i osvježi listu polica
                    $('#addPolicyModal').modal('hide');
                    // Ažuriraj tablicu
                    const tbody = document.querySelector('table tbody');
                    if (tbody) {
                        tbody.innerHTML = ''; // Očisti postojeće redove
                        // Dodaj nove redove u tablicu
                        data.model.policys.forEach(polica => {
                            const newRow = document.createElement('tr');
                            newRow.innerHTML = `
                        <td>${polica.policaId}</td>
                        <td>${polica.policyNumber}</td>
                        <td>${polica.amount}</td>
                    `;
                            tbody.appendChild(newRow);
                        });
                    } else {
                        location.reload();
                    }
                    showToast('Polica je uspješno unešena');
                } else {
                    //alert('Greška pri dodavanju police.');
                    showToast('Greška pri dodavanju police. ' + data.message);
                }
            } catch (error) {
                console.error('Greška:', error);
                //alert('Došlo je do greške.');
                showToast('Došlo je do greške.!');
            }
        } else {
            //alert('Molimo vas da popunite sva polja.');
            showToast('Molimo vas da popunite sva polja.');
        }
    });
});