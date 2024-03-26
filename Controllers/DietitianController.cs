using DietBowl.Services;
using DietBowl.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DietBowl.Controllers
{
    public class DietitianController : Controller
    {
        private readonly IDietitianService _dietitianService;

        public DietitianController(IDietitianService dietitianService)
        {
            _dietitianService = dietitianService;
        }

        // Tylko dla dietetyków
        [Authorize(Roles = "1")]
        public async Task<IActionResult> Patients()
        {
            List<Models.User> patients = await _dietitianService.GetAllFreePatientsAsync();
            return View(patients); // Przekazanie listy pacjentów do widoku
        }

        [HttpPost]
        public async Task<IActionResult> AddPatient(int idToadd)
        {
            var emailDietitian = await User.FindFirstValue(ClaimTypes.Email);

            await _dietitianService.AddPatient(emailDietitian, idToadd);

            return RedirectToAction("Patients", "Dietitian");
        }
    }
}
