using IS_PartnerPolicy.Helpers.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace IS_PartnerPolicy.DTO
{
    public class PartnerCreateDto
    {
        //public int PartnerId { get; set; }
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string Address { get; set; }// Adresa partnera
        public string CroatianPIN { get; set; }         // OIB partnera (neobavezno)

        [Required]
        [RegularExpression(@"^\d{20}$", ErrorMessage = "Partner broj mora sadržavati točno 20 brojeva.")]
        public string PartnerNumber { get; set; }
        [Required]
        public int PartnerTypeId { get; set; }

        public bool IsForeign { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "ExternalCode mora biti između 10 i 20 znakova.")]
        public string ExternalCode { get; set; }
        [Required]
        public string CreatedByUser { get; set; }       // Email korisnika koji je unio partnera
        public char Gender { get; set; }
        //public string PartnerTypeOptions { get; set; }
        //public string GenderOptions { get; set; }
    }
}
