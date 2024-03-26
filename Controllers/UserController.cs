using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DietBowl.EF;
using DietBowl.Models;
using DietBowl.Services;
using DietBowl.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DietBowl.Controllers
{
    public class UserController : Controller
    {

        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _configuration;
        private readonly DietBowlDbContext _dietBowlDbContext;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IConfiguration configuration, DietBowlDbContext dietBowlDbContext, IUserService userService)
        {
            _logger = logger;
            _configuration = configuration;
            _dietBowlDbContext = dietBowlDbContext;
            _userService = userService;
        }

        [HttpPost]
        public IActionResult Register([FromForm] User user)
        {
            ViewData["Title"] = "Rejestracja";
            var isUserAdded = _userService.Register(user);

            if (isUserAdded)
            {
                ViewBag.UserAdded = true;
                return Json(new { redirectToUrl = Url.Action("Index") });
            }

            return Json(new { message = "Email is already taken" });
        }
         public IActionResult Register()
        {
            ViewData["Title"] = "Rejestracja";
            return View();
        }

        public IActionResult Login()
        {
            ViewData["Title"] = "Login";
            return View();
        }
        public IActionResult Preferences()
        {
            ViewData["Title"] = "Preferencje";
            return View();
        }
        public IActionResult Body_Parameters()
        {
            ViewData["Title"] = "Parametry ciała";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

        [HttpPost]
        public async Task<IActionResult> LoginUser([FromForm] UserVM user)
        {
            ViewData["Title"] = "Logowanie";
            Task<ClaimsPrincipal>? principal = _userService.Login(user);

            if(principal.Result != null)
            {
                await HttpContext.SignInAsync(principal.Result);

                // Sprawdzenie roli użytkownika - dodane na lekcji
                var userClaims = principal.Result.Claims;
                var userRole = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                //return RedirectToAction("Index", "Home"); stary kod

                // Na podstawie roli przekieruj do odpowiedniego widoku - dodane na lekcji
                switch (userRole)
                {
                    case "1": // Dietetyk
                        return RedirectToAction("Dietician", "Home");
                    case "2": // Pacjent
                        return RedirectToAction("User", "Home");
                    default:
                        return RedirectToAction("Index", "Home"); // Domyślna strona po zalogowaniu
                }
            }

            return Json(new { message = "Nazwa użytkownika lub hasło nieprawidłowe" });
        }

        [Authorize(Roles = "1,2")]
        public IActionResult Ograniczenia()
        {
            ViewData["Title"] = "Login";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}