using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DietBowl.EF;
using DietBowl.Models;
using DietBowl.Services;
using DietBowl.Services.Interfaces;
using DietBowl.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
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
                //return Json(new { redirectToUrl = Url.Action("Index") });
                return RedirectToAction("Login", "User");
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

        public IActionResult AddBodyParameters()
        {
            ViewData["Title"] = "Dodaj parametry ciała";
            return View();
        }

        public IActionResult AddPreference()
        {
            ViewData["Title"] = "Dodaj preferencje";
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

            if (principal.Result != null)
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

        [Authorize(Roles = "0,1,2")]
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

        [HttpPost]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> AddBodyParameter(User user, BodyParameter bodyParameter)
        {
            var emailUser = User.FindFirstValue(ClaimTypes.Name);
            var userId = await _userService.GetUserIdByEmail(emailUser);

            if (userId != null)
            {
                var today = DateTime.Now.Date;
                var existingParameter = await _dietBowlDbContext.BodyParameters
                    .Where(bp => bp.UserId == userId && bp.Date == today)
                    .FirstOrDefaultAsync();

                if (existingParameter != null)
                {
                    //Trzeba dodac jakas obsluge tego jak uzytkownik dodal juz parametry dzisiejszego dnia
                    //poki co przekierowuje go po prostu na liste parametrow
                    return RedirectToAction("BodyParameters");
                }

                // Ustawiamy ID użytkownika w obiekcie bodyParameter
                bodyParameter.UserId = userId.Value;

                // Ustawiamy datę na aktualną
                bodyParameter.Date = DateTime.Now.Date;

                // Obliczamy BMI i zaokrąglamy do trzeciego miejsca po przecinku
                bodyParameter.BMI = Math.Round(bodyParameter.Weight / Math.Pow(bodyParameter.Height / 100, 2), 3);

                await _dietBowlDbContext.BodyParameters.AddAsync(bodyParameter);
                await _dietBowlDbContext.SaveChangesAsync();

                // Po pomyślnym dodaniu przekieruj do widoku z wszystkimi parametrami ciała
                return RedirectToAction("BodyParameters");
            }
            else
            {
                return BadRequest("Nie udało się dodać parametrów ciała. Użytkownik nie został rozpoznany lub nie masz uprawnień do wykonania tej operacji.");
            }
        }

        [Authorize(Roles = "2")]
        public async Task<IActionResult> BodyParameters()
        {
            var emailUser = User.FindFirstValue(ClaimTypes.Name);
            var userId = await _userService.GetUserIdByEmail(emailUser);


            var bodyParameters = await _userService.GetBodyParameters((int)userId);
            return View(bodyParameters);
        }



        //Preferencje
        [HttpPost]
        public async Task<IActionResult> AddPreference(PreferenceVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userId = await _userService.GetUserIdByEmail(User.FindFirstValue(ClaimTypes.Name));
                    var preference = new Preference
                    {
                        UserId = userId.Value,
                        Description = model.Description,
                        WeightGoal = model.WeightGoal,
                        ActivityStatus = model.ActivityStatus
                    };

                    foreach (var allergenId in model.SelectedAllergensIds)
                    {
                        var allergen = await _dietBowlDbContext.Allergens.FindAsync(allergenId);
                        if (allergen != null)
                        {
                            preference.Allergens.Add(allergen);
                        }
                    }

                    _dietBowlDbContext.Preferences.Add(preference);
                    await _dietBowlDbContext.SaveChangesAsync();
                    return RedirectToAction("Index", "Home"); // Zaktualizuj według potrzeb
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while adding preferences.");
                _logger.LogError(ex, "An error occurred while adding preferences.");
            }

            model.AvailableAllergens = await LoadAvailableAllergensAsync();
            return View(model);
        }

        private async Task<List<AllergenVM>> LoadAvailableAllergensAsync()
        {
            return await _dietBowlDbContext.Allergens.Select(a => new AllergenVM { Id = a.Id, Name = a.Name }).ToListAsync();
        }
    }
}