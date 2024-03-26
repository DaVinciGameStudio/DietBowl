using DietBowl.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DietBowl.Controllers
{
    public class DietitianController : Controller
    {
        private readonly IUserService _userService;

        public DietitianController(IUserService userService)
        {
            _userService = userService;
        }

        // Tylko dla dietetyków
        [Authorize(Roles = "1")]
        public async Task<IActionResult> Patients()
        {
            var patients = await _userService.GetAllPatientsAsync();
            return View(patients); // Przekazanie listy pacjentów do widoku
        }
    }
}
