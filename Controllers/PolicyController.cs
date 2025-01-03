using IS_PartnerPolicy.Models;
using IS_PartnerPolicy.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace IS_PartnerPolicy.Controllers
{
    public class PolicyController : Controller
    {
        private readonly ILogger<PolicyController> _logger;
        private readonly PartnerRepository _repository;

        public PolicyController(ILogger<PolicyController> logger, PartnerRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult EditPolicy(int id)
        {
            try
            {
                var partner = _repository.GetPartnerWithPolicies(id);
                if (partner == null)
                    return Json(new { success = false, message = "Došlo je do greške: Partner s Id-em nije pronađen"  }); // Ako partner nije pronađen

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
                    Policys = partner.Policys

                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Došlo je do pogreške prilikom dohvata podataka za edit polica.");
                return Json(new { success = false, message = "Došlo je do greške: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddPolicy([FromBody] EditPolicyModel policy)
        {
 
            // Provjeri da su podaci validni
            if (string.IsNullOrEmpty(policy.PolicyNumber) || policy.Amount <= 0)
            {
                return Json(new { success = false, message = " Došlo je do greške: morate unjeti validan broj police i iznos mora biti veći od 0. " });
            }
            try
            {
                //prvo provjeri da li broj police već postoji
                if (_repository.CheckIsPolicyExisting(policy.PolicyNumber))
                {
                    return Json(new { success = false, message = " Taj broj police već postoji. " });
                }

                var updatedPartner = _repository.AddPartnerPolicy(policy.PartnerId, policy.PolicyNumber, policy.Amount);

                if (updatedPartner == null)
                {
                    return NotFound(); // Ako partner nije pronađen, vratimo 404
                }
                var model = new EditPartnerModel
                {
                    LastName = updatedPartner.LastName,
                    FirstName = updatedPartner.FirstName,
                    PartnerId = updatedPartner.PartnerId,
                    Policys = updatedPartner.Policys,
                };

                // Vraćamo uspjeh i ažurirani partner s policama
                return Json(new { success = true, model = model });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Došlo je do pogreške prilikom spremanja police.");
                return Json(new { success = false, message = "Došlo je do greške: " + ex.Message });
            }
        }

    }
}
