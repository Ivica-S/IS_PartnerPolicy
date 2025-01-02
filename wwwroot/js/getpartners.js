
import { showToast } from './toast.js';
$(document).ready(function () {
    // Učitaj sve partnere na početku
    loadPartners();

    // Funkcija za učitavanje svih partnera
    function loadPartners() {
        fetch('/Partner/GetPartners')
            .then(response => {
                if (!response.ok) {
                    throw new Error('Greška pri učitavanju partnera');
                }
                return response.json(); //Pretvori odgovor u JSON
            })
            .then(data => {
                let rows = '';
                const tableBody = document.querySelector('#partnersTable tbody');
                if (!tableBody) return;  // Ako tabela ne postoji, izlazimo iz funkcije

                tableBody.innerHTML = '';  // Očistimo postojeći sadržaj tabele
                const fragment = document.createDocumentFragment();  // Koristimo fragment za bolje performanse

                if (data.message) {
                    showToast('Greška pri učitavanju partnera!' + data.message);
                    return;
                }
                // Dinamički
                data.forEach(partner => {
                    const row = createRow(partner);
                    //totalPrice += stavka.ukupnaCijena;
                    fragment.appendChild(row);
                });

                tableBody.appendChild(fragment);  // Dodajemo sve redove u table
                ColorNewAddedPartner();
                document.querySelectorAll('.btn-edit-partner').forEach(function (link) {
                    link.addEventListener('click', function (event) {
                        // Spriječi da se klik širi prema roditeljskom elementu (tr)
                        event.stopPropagation();
                    });
                });
            })
            .catch(error => {
                console.error('Greška:', error);
                showToast('Greška pri učitavanju partnera!');
                //alert('Greška pri učitavanju partnera!');
            });
    }

    function ColorNewAddedPartner() {
        // Provjera postoji li novi partner u localStorage
        //localStorage.setItem('newPartnerId', 34); //TEST
        var newPartnerId = localStorage.getItem('newPartnerId');
        if (newPartnerId) {
            // Ako postoji, tražimo redak sa tim ID-em i bojimo ga
            var partnerRow = $('#partnersTable tr[data-id="' + newPartnerId + '"]');
            partnerRow.addClass('new-partner-highlight'); // Dodajemo klasu za bojenje retka

            // Ukloni novi partner ID iz localStorage nakon što je označen
            localStorage.removeItem('newPartnerId');
        }
    }

    function createRow(partner) {
        const row = document.createElement('tr');
        row.setAttribute('data-id', partner.partnerId);
        row.classList.add('viewDetailsBtn');
        // Provjera uvjeta za crvenu zvjezdicu
        let fullName = partner.fullName;
        if (checkPartnerPolicyCountAndAmount(partner)) {
            fullName = '<span style="color: red;">*</span> ' + fullName;  // Dodaj crvenu zvjezdicu
        }
        row.appendChild(createTextCell('fullName', fullName));
        row.appendChild(createTextCell('partnerNumber', partner.partnerNumber));
        row.appendChild(createTextCell('croatianPIN', partner.croatianPIN));
        row.appendChild(createTextCell('createdByUser', partner.createdByUser));
        row.appendChild(createTextCell('isForeign', (partner.isForeign ? "DA" : "NE")));
        row.appendChild(createTextCell('partnerTypeId', getPartnerTypeText(partner.partnerTypeId, partner.partnerTypeOptions)));
        row.appendChild(createTextCell('gender', partner.gender));
        row.appendChild(createButtonCell(partner, 'Partner', 'btn-outline-primary', '', () => editPartner(partner.partnerId)));

        return row;
    }

    function createTextCell(id, value) {
        const td = document.createElement('td');
        td.classList.add(`text-center`);
        td.innerHTML = value;
        return td;
    }
    //Uredi partnera i Uredi policu
    function createButtonCell(partner, title, buttonClass, iconClass, onClick) {
        const td = document.createElement('td');
        td.classList.add('td-btn-action');
        const buttonPartner = document.createElement('a');
        buttonPartner.setAttribute('data-id', partner.partnerId);
        buttonPartner.setAttribute('href', `/Partner/EditPartner/${partner.partnerId}`);
        buttonPartner.classList.add('btn','gridlistapartnera', buttonClass,'me-2');
        buttonPartner.title = title;
        buttonPartner.innerHTML = `<span><i class="bi bi-pencil-fill"></i> <i class="bi bi-person-fill"></i></span>`;// + title;

        //buttonPartner.style.marginRight = '10px'; 
        td.appendChild(buttonPartner);

        // Provjeri uvjete za danger klasu
        let badgeClass = "bg-success";  // Defaultna boja za badge
        if (checkPartnerPolicyCountAndAmount(partner)) {
            badgeClass = "bg-danger";  // Ako ima više od 5 polica ili ukupan iznos polica prelazi 5000, promijeni u bg-danger
        }
        const buttonPolicy = document.createElement('a');
        buttonPolicy.setAttribute('data-id', partner.partnerId);
        buttonPolicy.setAttribute('href', `/Policy/EditPolicy/${partner.partnerId}`);
        buttonPolicy.classList.add('btn', 'gridlistapartnera', 'btn-outline-success');
        buttonPolicy.title = title + " polica";
        //buttonPolicy.innerHTML = `<i class="bi bi-pencil-fill"></i> <i class="bi bi-file-earmark"</i>` + `<span class="badge ${badgeClass} bg-grey">${partner.policys.length}</span>`;
        buttonPolicy.innerHTML = `
                                    
                                    <span ><i class="bi bi-file-earmark"></i> <span class="badge ${badgeClass}">${partner.policys.length}</span></span>
                                `;
  
        td.appendChild(buttonPolicy);
        return td;
    }
    //Provjera za kolicinu i iznos police
    function checkPartnerPolicyCountAndAmount(partner) {
        let isDanger = false;
        if (partner.policys.length > 5 || (partner.policys.reduce((total, policy) => total + parseFloat(policy.amount), 0) > 5000)) {
            isDanger = true;  // Ako ima više od 5 polica ili ukupan iznos polica prelazi 5000
        }
        return isDanger;
    }
    // Klik na gumb za otvaranje modala s detaljima partnera
    $(document).on('click', '.viewDetailsBtn', function () {
        var dataPartnerId = $(this).data('id');
        
        // Dohvati detalje partnera
        fetch(`/Partner/GetPartnerById/${dataPartnerId}`).then(response => {
            if (!response.ok) {
                throw new Error('Greška pri dohvaćanju detalja partnera');
            }
            return response.json(); // Pretvori odgovor u JSON 
        }).then(partner => {
            // Ispis detalja partnera u modal 
            document.getElementById('modalFirstName').textContent = 'Ime i prezime: ' + partner.fullName;
            document.getElementById('modalAddress').textContent = 'Adresa: ' + partner.address;
            document.getElementById('modalPartnerNumber').textContent = 'Partner Number: ' + partner.partnerNumber;
            document.getElementById('croatianPIN').textContent = 'OIB: ' + partner.croatianPIN;
            document.getElementById('modalPartnerType').textContent = 'Tip partnera: ' + getPartnerTypeText(partner.partnerTypeId, partner.partnerTypeOptions);
            document.getElementById('modalcreatedAtUtc').textContent = 'Datum kreiranja: ' + partner.createdAtUtc;
            document.getElementById('modalcreatedByUser').textContent = 'Kreirao: ' + partner.createdByUser;
            document.getElementById('modalisForeign').textContent = 'Stranac: ' + (partner.isForeign ? "DA": "NE");
            document.getElementById('modalexternalCode').textContent = 'Vanjski kod: ' + partner.externalCode;
            document.getElementById('modalgender').textContent = 'Spol: ' + partner.gender;
            document.getElementById('modalpolicys').innerHTML = 'Police: ' + getPolicysForModalDetails(partner.policys);
            $('#partnerModal').modal('show');
        }).catch(error => {
            console.error('Greška:', error);
            showToast('Greška pri dohvaćanju detalja partnera!');
            //alert('Greška pri dohvaćanju detalja partnera!');
        });
    });

    function getPolicysForModalDetails(policys) {
        // Provjeri je li partner.policys niz i nije prazan
        if (policys && policys.length > 0) {

            let policiesText = '<br>';

            let totalAmount = 0;
            
            policys.forEach(policy => {
                
                policiesText += `Broj police: ${policy.policyNumber}, Iznos: ${policy.amount} <br>`;
                // Zbroji amount za ukupno
                totalAmount += policy.amount;
            });
            // Na kraju dodaj Ukupno iznos
            policiesText += `<br><strong>Ukupno iznos: ${totalAmount} EUR</strong> `;
            
            return policiesText;
        } else {
            // Ako policys nije dostupan ili je prazan
            return policiesText = 'Nema polica.';
        }
    }

    function getPartnerTypeText(partnerTypeId, partnertypes) {
        // Pronađi odgovarajući tip partnera iz partnerTypeOptions
        let partnerType = partnertypes.find(option => option.value == partnerTypeId);

        if (partnerType) {
            return partnerType.text;
        } else {
            return 'nije odabran';
        }
    }
});