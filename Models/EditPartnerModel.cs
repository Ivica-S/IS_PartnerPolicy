﻿using IS_PartnerPolicy.Helpers;
using IS_PartnerPolicy.Helpers.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace IS_PartnerPolicy.Models
{
    public class EditPartnerModel
    {
        public int PartnerId { get; set; }
        public string FullName => FirstName + " " + LastName;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        [Required]
        [RegularExpression(@"^\d{20}$", ErrorMessage = "Partner broj mora sadržavati točno 20 brojeva.")]
        public string PartnerNumber { get; set; }
        public string CroatianPIN { get; set; }
        public PartnerType PartnerTypeId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public string CreatedByUser { get; set; }
        public bool IsForeign { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "ExternalCode mora biti između 10 i 20 znakova.")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "ExternalCode može sadržavati samo alfanumeričke znakove.")]
        public string ExternalCode { get; set; }
        public char Gender { get; set; }

        public SelectList PartnerTypeOptions { get; set; }
        public SelectList GenderOptions { get; set; }
        // Kolekcija polica koje pripadaju ovom partneru
        public List<PartnerPolica> Policys { get; set; } = new List<PartnerPolica>(); // Osiguranje da je ovo lista

        // Konstruktor za inicijalizaciju SelectList za Gender
        public EditPartnerModel()
        {
            GenderOptions = SelectListHelper.GetGenderList(Gender.ToString());

            PartnerTypeOptions = SelectListHelper.GetPartnerTypeOptionsList((int)PartnerTypeId);
        }
    }
}
