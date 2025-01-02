using System.ComponentModel.DataAnnotations;

namespace IS_PartnerPolicy.Models
{
    public class PartnerPolica
    {
        public int PolicaId { get; set; }
        public string PartnerId { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 10)]
        public string PolicyNumber { get; set; } // Broj police
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Količina mora biti veća od 0.")]
        public decimal Amount { get; set; }  //IznosPolice

        // Partner kojem polica pripada
        public Partner Partner { get; set; }            
    }

}
