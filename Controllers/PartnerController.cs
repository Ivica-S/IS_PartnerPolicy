﻿using IS_PartnerPolicy.DTO;
using IS_PartnerPolicy.Models;
using IS_PartnerPolicy.Repository;
using Microsoft.AspNetCore.Mvc;

namespace IS_PartnerPolicy.Controllers
{
    public class PartnerController : Controller
    {
        private readonly ILogger<PartnerController> _logger;
        private readonly PartnerRepository _repository;

        public PartnerController(ILogger<PartnerController> logger, PartnerRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetPartners()
        {
            try
            {
                var partners = _repository.GetAllPartners();
                return Ok(partners);
            }
            catch (Exception ex)
            {
                // Logiranje greške
                _logger.LogError(ex, "Došlo je do pogreške prilikom dohvaćanja partnera.");
                //return StatusCode(500, "Došlo je do greške prilikom obrade vašeg zahtjeva.");
                return Json(new { success = false, message = "Došlo je do greške: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetPartnerById(int id)
        {
            var partner = _repository.GetPartnerById(id);
            return Ok(partner);
        }
        // Akcija za prikazivanje forme za uređivanje partnera
        public IActionResult EditPartner(int id)
        {
            try
            {
                var partner = _repository.GetPartnerById(id);
                if (partner == null)
                    return NotFound(); // Ako partner nije pronađen

                var model = new EditPartnerModel
                {
                    PartnerId = partner.PartnerId,
                    FirstName = partner.FirstName,
                    LastName = partner.LastName,
                    Address = partner.Address,
                    PartnerNumber = partner.PartnerNumber,
                    CroatianPIN = partner.CroatianPIN,
                    PartnerTypeId = partner.PartnerTypeId,
                    CreatedAtUtc = partner.CreatedAtUtc,
                    CreatedByUser = partner.CreatedByUser,
                    IsForeign = partner.IsForeign,
                    ExternalCode = partner.ExternalCode,
                    Gender = partner.Gender,

                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Došlo je do pogreške prilikom dohvata podataka za edit partnera.");
                return Json(new { success = false, message = "Došlo je do greške: " + ex.Message });
            }
        }
        // Spremanje izmjena partnera
        [HttpPost]
        public IActionResult EditPartner([FromRoute] int id, [FromBody] EditPartnerDTO modeldto)
        {
            if (modeldto == null)
            {
                return BadRequest("Podaci nisu ispravni.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model za partnera s ID: {PartnerId} nije validan.", id);
                // Vratiti greške u JSON formatu sa standardnim odgovarajućim statusom greške
                return BadRequest(ModelState);
            }

            try
            {
                // Ažuriraj partnera
                modeldto.PartnerId = id;
                var rowsAffected = _repository.EditPartner(modeldto);

                // Ako nije bilo promjena, vrati BadRequest
                if (rowsAffected == 0)
                {
                    return BadRequest("Nema promjena za ažuriranje.");
                }

                // Ako je uspješno ažurirano
                _logger.LogInformation("Partner s ID: {id} uspješno ažuriran.", id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Došlo je do pogreške pri ažuriranju partnera s ID: {id}.", id);
                return Json(new { success = false, message = "Došlo je do greške: " + ex .Message});
                //return StatusCode(500, "Greška pri ažuriranju podataka. Pokušajte ponovno.");
            }
        }

        // Ova akcija prikazuje stranicu za dodavanje novog partnera
        public IActionResult NewPartner()
        { 
            var partner = new Partner();
            return View(partner); 
        }

        [HttpPost]
        public IActionResult AddNewPartner([FromBody] PartnerCreateDto partner)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model za novog partnera nije validan.");
                // Vratiti greške u JSON formatu sa standardnim odgovarajućim statusom greške
                return BadRequest(ModelState);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (_repository.IsExternalCodeUnique(partner.ExternalCode))
                    {
                        ModelState.AddModelError("ExternalCode", "ExternalCode mora biti jedinstven.");
                        return Json(new { success = false, message = "Došlo je do greške: Vanjski kod već postoji u bazi"});
                    }
                    var newPartnerId = _repository.AddPartner(partner);
                    return Json(new { success = true, partnerId = newPartnerId });                            
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Došlo je do pogreške prilikom spremanja novog partnera.");
                    return Json(new { success = false, message = "Došlo je do greške: " + ex.Message });
                }
            }
            return BadRequest(ModelState);
        }     

        [HttpPost("policy")]
        public IActionResult AddPolicy([FromBody] PartnerPolica policy)
        {
            if (ModelState.IsValid)
            {
                //_repository.AddPolicy(policy);
                return Ok(policy);
            }
            return BadRequest(ModelState);
        }
    }
}
