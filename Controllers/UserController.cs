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
        //[Authorize(Roles = "2")]
        //[HttpGet]
        //public async Task<IActionResult> ViewPreference()
        //{
        //    var emailUser = User.FindFirstValue(ClaimTypes.Name);
        //    var userId = await _userService.GetUserIdByEmail(emailUser);

        //    // Pobierz preferencje użytkownika o podanym userId
        //    var userPreferences = await _userService.GetUserPreferences((int)userId);

        //    if (userPreferences == null)
        //    {
        //        return NotFound(); // Jeśli preferencje nie zostały znalezione, zwróć 404
        //    }

        //    return View(userPreferences); // Wyświetl widok z preferencjami użytkownika
        //}

        [Authorize(Roles = "2")]
        [HttpGet]
        public async Task<IActionResult> ViewPreference()
        {
            var emailUser = User.FindFirstValue(ClaimTypes.Name);
            var userId = await _userService.GetUserIdByEmail(emailUser);

            var userPreferences = await _userService.GetUserPreferences((int)userId);

            // Przekazanie preferencji do widoku lub null, jeśli preferencje nie zostały jeszcze dodane
            return View(userPreferences);
        }


        [Authorize(Roles = "2")]
        [HttpGet]
        public async Task<IActionResult> AddPreference(int userId)
        {
            var emailUser = User.FindFirstValue(ClaimTypes.Name);
            userId = (int)await _userService.GetUserIdByEmail(emailUser);



            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Pobierz listę alergenów z bazy danych
            var allergens = await _dietBowlDbContext.Allergens.ToListAsync();

            // Przygotuj model dla widoku
            var model = new Preference
            {
                UserId = userId // Upewnij się, że UserId jest przekazywane do modelu, jeśli jest potrzebne
            };

            // Przekaż listę alergenów do ViewBag, aby móc ją wykorzystać w widoku
            ViewBag.Allergens = allergens;

            return View(model);
        }

        [Authorize(Roles = "2")]
        [HttpPost]
        public async Task<IActionResult> AddPreference(Preference preference, int[] selectedAllergens)
        {
            if (!ModelState.IsValid)
            {
                // Logowanie błędów walidacji
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage); // Logowanie błędów
                }

                // Ponowne ładowanie alergenów do ViewBag, aby zachować dane formularza
                ViewBag.Allergens = await _dietBowlDbContext.Allergens.ToListAsync();
                return View(preference); // Ponowne wyświetlenie widoku z błędami walidacji
            }

            var user = await _userService.GetUserById(preference.UserId);
            if (user == null)
            {
                return NotFound(); // Jeśli użytkownik nie istnieje, zwróć NotFound
            }

            // Dodawanie wybranych alergenów do preferencji
            foreach (var allergenId in selectedAllergens)
            {
                var allergen = await _dietBowlDbContext.Allergens.FindAsync(allergenId);
                if (allergen != null)
                {
                    preference.Allergens.Add(allergen);
                }
            }

            // Tutaj możesz wykonać dodatkową logikę, jeśli to konieczne, np. zapis do bazy danych
            await _userService.AddUserPreference(preference.UserId, preference);

            return RedirectToAction("Index", "Home"); // Przekierowanie po pomyślnym dodaniu preferencji
        }

    }
}