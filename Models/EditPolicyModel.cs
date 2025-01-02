using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace IS_PartnerPolicy.Models
{
    public class EditPolicyModel
    {
        [JsonIgnore]
        public int PolicaId { get; set; }
        public int PartnerId { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 10)]
        public string PolicyNumber { get; set; } // Broj police
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Količina mora biti veća od 0.")]
        public decimal Amount { get; set;}  //IznosPolice

        // Partner kojem polica pripada
        [JsonIgnore]
        public Partner Partner { get; set; }

    }
}
