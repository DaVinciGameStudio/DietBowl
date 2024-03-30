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

        [Authorize(Roles = "1")]    // Tylko dla dietetyków
        public async Task<IActionResult> Patients()
        {
            List<Models.User> freePatients = await _dietitianService.GetAllFreePatients();
            return View(freePatients); // Przekazanie listy pacjentów do widoku
        }

        [Authorize(Roles = "1")]
        public async Task<IActionResult> AssignedPatients()
        {
            var emailDietitian = User.FindFirstValue(ClaimTypes.Name); // Pobierz adres e-mail dietetyka

            // Znajdź ID dietetyka na podstawie adresu e-mail
            var dietitianId = await _dietitianService.GetDietitianIdByEmail(emailDietitian);

            var patients = await _dietitianService.GetAssignedPatients((int)dietitianId);
            return View(patients);
        }

        [HttpPost]
        [Authorize(Roles = "1")]    // Tylko dla dietetyków
        public async Task<IActionResult> AddPatient(int userId)
        {
            var emailDietitian = User.FindFirstValue(ClaimTypes.Name); // Pobierz adres e-mail dietetyka

            // Znajdź ID dietetyka na podstawie adresu e-mail
            var dietitianId = await _dietitianService.GetDietitianIdByEmail(emailDietitian);

            // Jeśli udało się znaleźć ID dietetyka
            if (dietitianId != null)
            {
                // Dodaj pacjenta przy użyciu znalezionego ID dietetyka
                bool result = await _dietitianService.AddPatient(dietitianId.Value, userId);

                if (result)
                {
                    return RedirectToAction("Patients"); // Przekierowanie do akcji "Patients" jeśli dodanie się powiodło
                }
            }

            // Obsługa błędu
            //ModelState.AddModelError("", "Nie udało się dodać pacjenta. Spróbuj ponownie.");
            //return View("Error"); // Możesz utworzyć widok "Error" do wyświetlania komunikatu o błędzie
            return RedirectToAction("Index", "Home"); // Domyślna strona po zalogowaniu
        }
    }
}
