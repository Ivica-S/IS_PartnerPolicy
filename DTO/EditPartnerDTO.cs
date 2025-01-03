using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace IS_PartnerPolicy.DTO
{
    public class EditPartnerDTO
    {
        [JsonIgnore]
        public int PartnerId { get; set; }

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
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "ExternalCode može sadržavati samo alfanumeričke znakove.")]
        public string ExternalCode { get; set; }
        [Required]
        public string CreatedByUser { get; set; }       // Email korisnika koji je unio partnera
        public char Gender { get; set; }
    }
}
