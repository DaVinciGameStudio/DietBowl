using DietBowl.EF;
using DietBowl.Models;
using DietBowl.Services;
using DietBowl.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DietBowl.Controllers
{
    public class AdministratorController : Controller
    {
        private readonly ILogger<AdministratorController> _logger;
        private readonly DietBowlDbContext _dietBowlDbContext;
        private readonly IAdministratorService _administratorService;

        public AdministratorController(ILogger<AdministratorController> logger, IAdministratorService administratorService, DietBowlDbContext dietBowlDbContext)
        {
            _logger = logger;
            _administratorService = administratorService;
            _dietBowlDbContext = dietBowlDbContext;
        }

        [Authorize(Roles = "0")]
        public async Task<IActionResult> Dietitians()
        {
            List<Models.User> dietitians = await _administratorService.GetAllDietitians();
            return View(dietitians);
        }

        public async Task<IActionResult> Users()
        {
            List<User> users = await _administratorService.GetAllUsers();
            return View(users);
        }


        [Authorize(Roles = "0")]
        public IActionResult AddDietitian()
        {
            return View();
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
                //return Json(new { redirectToUrl = Url.Action("Index") });
                return RedirectToAction("Dietitians", "Administrator");
            }

            return Json(new { message = "Email is already taken" });
        }

        public async Task<IActionResult> DeleteUser(int id)
        {
            await _administratorService.DeleteUserById(id);
            return RedirectToAction(nameof(Users));
        }

        public async Task<IActionResult> DeleteDietitian(int id)
        {
            await _administratorService.RemoveDietitianAndReleasePatients(id);
            return RedirectToAction(nameof(Dietitians));
        }


    }
}
