using IS_PartnerPolicy.Helpers;
using IS_PartnerPolicy.Helpers.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace IS_PartnerPolicy.Models
{
    public class Partner
    {
        public int PartnerId { get; set; }              // Primarni ključ (ID partnera)
        public string FirstName { get; set; }           // Ime partnera
        public string LastName { get; set; }            // Prezime partnera
        public string Address { get; set; }// Adresa partnera
        [Required]
        [RegularExpression(@"^\d{20}$", ErrorMessage = "Partner broj mora sadržavati točno 20 brojeva.")]
        public string PartnerNumber { get; set; }       // Broj partnera (točno 20 znamenki)
        public string CroatianPIN { get; set; }         // OIB partnera (neobavezno)
        public PartnerType PartnerTypeId { get; set; }          // Tip partnera (1 - Personal, 2 - Legal)
        public DateTime CreatedAtUtc { get; set; }      // Datum i vrijeme unosa partnera
        public string CreatedByUser { get; set; }       // Email korisnika koji je unio partnera
        public bool IsForeign { get; set; }             // Da li je partner stranac
        [Required]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "ExternalCode mora biti između 10 i 20 znakova.")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "ExternalCode može sadržavati samo alfanumeričke znakove.")]
        public string ExternalCode { get; set; }        // Jedinstveni kod partnera
        public char Gender { get; set; }                // Spol partnera (M, F, N)

        public string FullName => FirstName + " " + LastName;
        /*POLICA*/
        //public string InsuranceNumber { get; set; } // Broj police
        //public decimal InsuranceAmount { get; set; } // Iznos police

        //public List<PartnerPolica> PartnerPolices { get; set; } // Lista polica koje pripadaju partneru

        public SelectList PartnerTypeOptions { get; set; }
        public SelectList GenderOptions { get; set; }
        // Kolekcija polica koje pripadaju ovom partneru
        public List<PartnerPolica> Policys { get; set; } = new List<PartnerPolica>(); // Osiguranje da je ovo lista
        // Konstruktor za inicijalizaciju SelectList za Gender
        public Partner()
        {
            GenderOptions = SelectListHelper.GetGenderList(Gender.ToString());

            PartnerTypeOptions = SelectListHelper.GetPartnerTypeOptionsList((int)PartnerTypeId);
        }

    }
}
