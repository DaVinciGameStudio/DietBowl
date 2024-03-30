using DietBowl.Models;
using DietBowl.Services;
using DietBowl.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DietBowl.Controllers
{
    public class AdministratorController : Controller
    {
        private readonly IAdministratorService _administratorService;

        public AdministratorController(IAdministratorService administratorService)
        {
            _administratorService = administratorService;
        }

        [Authorize(Roles = "0")]
        public async Task<IActionResult> Dietitians()
        {
            List<Models.User> dietitians = await _administratorService.GetAllDietitians();
            return View(dietitians);
        }

        [HttpPost]
        [Authorize(Roles = "0")]
        public IActionResult AddDietitian([FromForm] User user)
        {
            ViewData["Title"] = "Rejestracja dietetyka";
            var isUserAdded = _administratorService.RegisterDietitian(user);

            if (isUserAdded)
            {
                ViewBag.UserAdded = true;
                return Json(new { redirectToUrl = Url.Action("Index") });
            }

            return Json(new { message = "Email is already taken" });
        }
    }
}
