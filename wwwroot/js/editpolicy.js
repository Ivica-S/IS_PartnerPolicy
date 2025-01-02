
import { showToast } from './toast.js';
$(document).ready(function () {
    $('#savePolicyButton').click(async function () {
        // Dohvati PartnerId iz data-atributa HTML elementa
        var partnerId = $('#addPolicyModal').data('partner-id');
        var policyNumber = $('#PolicyNumber').val();
        var amount = $('#Amount').val();

        if (isValidForm(policyNumber, amount)){
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
                        let totalAmount = 0; // Zbroj iznosa svih polica
                        // Dodaj nove redove u tablicu
                        data.model.policys.forEach(polica => {
                            const newRow = document.createElement('tr');
                            newRow.innerHTML = `
                        <td>${polica.policaId}</td>
                        <td>${polica.policyNumber}</td>
                        <td>${polica.amount}</td>
                    `;
                            tbody.appendChild(newRow);
                            // Dodaj Amount u ukupni iznos
                            totalAmount += polica.amount;
                        });
                        // Prikaz ukupnog iznosa
                        const totalAmountDiv = document.querySelector('.total-amount');
                        if (totalAmountDiv) {
                            totalAmountDiv.textContent = `Ukupno iznos svih polica: ${totalAmount} EUR`;
                        }
                        // Provjera treba li dodati crvenu zvjezdicu
                        const fullNameElement = document.querySelector('h2');  // Hvatamo h2 element za ime partnera
                        if (data.model.policys.length > 5 || totalAmount > 5000) {
                            fullNameElement.innerHTML = `<span class="text-danger">*</span> ${data.model.fullName} - pripadajuće police`;
                        } else {
                            fullNameElement.innerHTML = `${data.model.fullName} - pripadajuće police`;
                        }
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
        }
       
    });
    function isValidForm(policyNumber, amount) {
        var valid = true;
        if (policyNumber && amount) {
            if (amount <= 0) {
                showToast('Iznos mora biti veći od 0.');
            }
            if (policyNumber.length < 10 || policyNumber.length > 15) {
                showToast('Broj police mora biti između 10 i 15 znakova.');
                document.getElementById('ExternalCode').focus();
                valid = false
            }
        } else {
            //alert('Molimo vas da popunite sva polja.');
            showToast('Molimo vas da popunite sva polja.');
        }

        return valid;
    }
});