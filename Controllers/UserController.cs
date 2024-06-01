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

            return RedirectToAction("Login", "User");
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
        




        [HttpGet]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> EditBodyParameter(int id)
        {
            // Pobieramy id aktualnie zalogowanego użytkownika
            var emailUser = User.FindFirstValue(ClaimTypes.Name);
            var userId = await _userService.GetUserIdByEmail(emailUser);

            if (userId != null)
            {
                // Szukamy parametru ciała o podanym id dla danego użytkownika
                var bodyParameter = await _dietBowlDbContext.BodyParameters
                    .Where(bp => bp.UserId == userId && bp.Id == id)
                    .FirstOrDefaultAsync();

                if (bodyParameter != null)
                {
                    // Jeśli parametr ciała został znaleziony, zwracamy widok edycji z przekazanym parametrem
                    return View(bodyParameter);
                }
                else
                {
                    // Jeśli parametr ciała o podanym id nie został znaleziony dla danego użytkownika
                    return NotFound();
                }
            }
            else
            {
                // Jeśli użytkownik nie został zidentyfikowany, zwracamy błąd
                return BadRequest("Użytkownik nie został zidentyfikowany.");
            }
        }

        // POST: BodyParameters/Edit/{id}
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> EditBodyParameter(int id, [Bind("Id,Height,Weight")] BodyParameter bodyParameter)
        {
            // Pobieramy adres e-mail aktualnie zalogowanego użytkownika
            var bodyParameters = await _userService.GetBodyParametersById(id);

            // Sprawdzamy, czy przekazane id zgadza się z id parametru ciała
            if (id != bodyParameter.Id)
            {
                return NotFound();
            }


                try
                {
                    // Aktualizujemy parametr ciała w bazie danych
                    bodyParameters.BMI = Math.Round(bodyParameter.Weight / Math.Pow(bodyParameter.Height / 100, 2), 3);
                    bodyParameters.Weight = bodyParameter.Weight;
                    bodyParameters.Height = bodyParameter.Height;
                    _dietBowlDbContext.Update(bodyParameters);
                    await _dietBowlDbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_dietBowlDbContext.BodyParameters.Any(bp => bp.Id == bodyParameter.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                
            }
            //// Jeśli ModelState nie jest poprawny, zwracamy widok edycji z błędami walidacji
            //return View(bodyParameter);

            // Po pomyślnej edycji przekierowujemy do widoku z listą parametrów ciała
            return RedirectToAction("BodyParameters", "User");
        }


        [Authorize(Roles = "2")]
        [HttpGet]
        public async Task<IActionResult> EditPreference(int id)
        {
            var preference = await _dietBowlDbContext.Preferences.Include(p => p.Allergens).FirstOrDefaultAsync(p => p.Id == id);
            if (preference == null)
            {
                return NotFound();
            }

            ViewBag.Allergens = await _dietBowlDbContext.Allergens.ToListAsync();
            return View(preference);
        }

        [Authorize(Roles = "2")]
        [HttpPost]
        public async Task<IActionResult> EditPreference(Preference preference, int[] selectedAllergens)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage); // Logowanie błędów
                }

                ViewBag.Allergens = await _dietBowlDbContext.Allergens.ToListAsync();
                return View(preference);
            }

            var existingPreference = await _dietBowlDbContext.Preferences.Include(p => p.Allergens).FirstOrDefaultAsync(p => p.Id == preference.Id);
            if (existingPreference == null)
            {
                return NotFound();
            }

            // Aktualizacja danych preferencji
            existingPreference.Description = preference.Description;
            existingPreference.WeightGoal = preference.WeightGoal;
            existingPreference.ActivityStatus = preference.ActivityStatus;

            // Aktualizacja alergenów
            existingPreference.Allergens.Clear();
            foreach (var allergenId in selectedAllergens)
            {
                var allergen = await _dietBowlDbContext.Allergens.FindAsync(allergenId);
                if (allergen != null)
                {
                    existingPreference.Allergens.Add(allergen);
                }
            }

            await _dietBowlDbContext.SaveChangesAsync();
            return RedirectToAction("ViewPreference", "User");
        }
    }
}