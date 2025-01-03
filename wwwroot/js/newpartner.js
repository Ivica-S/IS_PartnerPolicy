import { showToast } from './toast.js';
$(document).ready(function () {
    var partnerPolices = [];

    // Kada se klikne gumb za dodavanje police
    $('#addPolicyModal').on('show.bs.modal', function (e) {
        // Resetiraj formu
        $('#policyForm')[0].reset();
    });

    // Dodavanje nove police
    $('#policyForm').submit(function (e) {
        e.preventDefault();

        // Dohvati podatke iz forme
        var policy = {
            policyNumber: $('#policyNumber').val(),
            policyAmount: $('#policyAmount').val()
        };

        // Dodaj policu u niz partnerovih polica
        partnerPolices.push(policy);
        // Prikazivanje toast obavijesti
        showToast();
        // Zatvori modal
        $('#addPolicyModal').modal('hide');

        // Provjeri je li partner premašio uvjete za zvjezdicu
        checkPartnerStar();
    });

    // Provjera da li partner treba imati zvjezdicu
    function checkPartnerStar() {
        if (partnerPolices.length > 5 || partnerPolices.reduce((sum, policy) => sum + parseFloat(policy.policyAmount), 0) > 5000) {
            $('#FirstName').val('*' + $('#FirstName').val()); // Dodaj zvjezdicu prije imena partnera
        } else {
            $('#FirstName').val($('#FirstName').val().replace('*', '')); // Ukloni zvjezdicu ako uvjeti nisu ispunjeni
        }
    }

    // Funkcija za provjeru validnosti forme
    function isValidForm() {
        var valid = true;

        // Provjeri obavezna polja
        if ($('#FirstName').val().length < 2) {
            showToast('Ime mora sadržavati barem 2 znaka!');
            document.getElementById('FirstName').focus();
            valid = false
        };
        if ($('#LastName').val().length < 2) {
            showToast('Prezime mora sadržavati barem 2 znaka!');
            document.getElementById('LastName').focus();
            valid = false
        };
        var partnerNumber = document.getElementById('PartnerNumber').value;
        var vanjskiKod = document.getElementById('ExternalCode').value;

        // Provjera da li broj ima točno 20 znamenki
        if (partnerNumber.length !== 20) {
            // alert('Partner broj mora imati točno 20 znamenki!');
            showToast('Broj partnera mora imati točno 20 znamenki!');
            document.getElementById('PartnerNumber').focus();
            valid = false
        }
        if (vanjskiKod.length < 10 || vanjskiKod.length > 20) {
            showToast('Vanjski kod mora biti između 10 i 20 znakova.');
            document.getElementById('ExternalCode').focus();
            valid = false
        }

        return valid;
    }
    // Spremi partnera
    $('#partnerForm').submit(function (e) {
        e.preventDefault();
       
        if (isValidForm()) {
            //iz forme u objekt
            var partner = {
                firstName: $('#FirstName').val(),
                lastName: $('#LastName').val(),
                address: $('#Address').val(),
                partnerNumber: $('#PartnerNumber').val(),
                partnerTypeId: $('#PartnerTypeId').val(),
                gender: $('#Gender').val(),
                createdByUser: $('#CreatedByUser').val(),
                croatianPIN: $('#CroatianPIN').val(),
                isForeign: $('#IsForeign').is(':checked'),
                externalCode: $('#ExternalCode').val(),
            };


            // Pošalji podatke na backend za unos partnera
            fetch(addNewPartnerUrl, {
                method: 'POST', 
                headers: {
                    'Content-Type': 'application/json', 
                },
                body: JSON.stringify(partner),  // Pretvaranje objekta u JSON string
            })
                .then(response => {
                    // Provjera je li odgovor uspješan (status kod 2xx)
                    if (!response.ok) {
                        // Ako nije uspješan, prikazi odgovarajuću poruku
                        return response.json().then(errorData => {
                            // Ako postoje greške u odgovoru
                            if (errorData && Object.keys(errorData).length > 0) {
                                // Prolazi kroz greške (ako postoji)
                                for (let field in errorData) {
                                    // Prikazivanje grešaka korisniku, ovde možeš koristiti showToast
                                    showToast(`${field} : ${errorData[field].join(', ')}`);
                                }
                            } else {
                                showToast('Network response was not ok');
                            }
                            throw new Error('Podaci nisu ispravni.');
                        });
                    }
                    return response.json();  // Ako je uspješan odgovor, parsiraj ga
                })
                .then(data => {
                    showToast(data.message || 'Partner uspješno dodan!');

                    if (data.partnerId) {
                        // Spremaj partner ID u localStorage
                        localStorage.setItem('newPartnerId', data.partnerId);
                        window.location.href = '/Partner/Index'; // Preusmjerenje na listu partnera
                    }

                })
                .catch(error => {
                    console.error('Greška pri slanju zahtjeva:', error);
                    showToast(error.message || 'Došlo je do pogreške pri dodavanju partnera.');
                });
        }
    });
});