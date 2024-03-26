using DietBowl.Services;
using DietBowl.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            List<Models.User> patients = await _dietitianService.GetAllPatientsAsync();
            return View(patients); // Przekazanie listy pacjentów do widoku
        }
    }
}
