using DietBowl.Models;
using DietBowl.Services;
using DietBowl.Services.Interfaces;
using DietBowl.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Protocol;
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

        [Authorize(Roles = "1")]
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
        [Authorize(Roles = "1")]
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
            //return View("Error"); // Łukasz możesz utworzyć widok "Error" do wyświetlania komunikatu o błędzie
            return RedirectToAction("Index", "Home"); // Domyślna strona po zalogowaniu
        }

        [HttpPost]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> RemovePatient(int userId)
        {
            var emailDietitian = User.FindFirstValue(ClaimTypes.Name);

            var dietitianId = await _dietitianService.GetDietitianIdByEmail(emailDietitian);

            // Jeśli udało się znaleźć ID dietetyka
            if (dietitianId != null)
            {
                // Usuń przypisanego pacjenta ustawiając jego dietetyka na null
                bool result = await _dietitianService.RemovePatient(dietitianId.Value, userId);

                if (result)
                {
                    return RedirectToAction("AssignedPatients"); // Przekierowanie do akcji "AssignedPatients" jeśli usunięcie się powiodło
                }
            }

            // Obsługa błędu
            //ModelState.AddModelError("", "Nie udało się usunąć pacjenta. Spróbuj ponownie.");
            //return View("Error"); // Łukasz możesz utworzyć widok "Error" do wyświetlania komunikatu o błędzie
            return RedirectToAction("Index", "Home"); // Domyślna strona po zalogowaniu
        }

         public async Task<IActionResult> AddRecipeToDiet(int userId, DateTime data)
         {
            var recipes = await _dietitianService.GetRecipes();
            ViewData["List"] = recipes.Select(x => x.Title).ToJson();
            ViewData["IdUser"] = userId;
            ViewBag.date = data;
            return View(recipes);
         }

        [HttpPost]
         public async Task<IActionResult> AddRecipeAtDay(int userId, DateTime date, string recipeList)
         {
            dynamic jsonData = JsonConvert.DeserializeObject(recipeList)!;
            List<int> idRecipes = new();
            foreach(int idRepice in jsonData)
            {
                idRecipes.Add(idRepice);
            }
            await _dietitianService.AddRecipeAtDay(userId, date, idRecipes);
            return RedirectToAction("AssignedPatients", "Dietitian");
         }




        //spaghetti
        [HttpGet]
        public IActionResult SetUserMacronutrients(int userId)
        {
            var user = _dietitianService.GetUserWithPreference(userId);

            if (user == null)
            {
                return NotFound();
            }

            var model = new UserMacronutrientsVM
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Age = CalculateAge(user.DateOfBirth),
                Sex = user.Sex,
                WeightGoal = user.Preference?.WeightGoal,
                ActivityStatus = user.Preference?.ActivityStatus
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult SetUserMacronutrients(UserMacronutrientsVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _dietitianService.SetUserMacronutrients(model.UserId, model.Protein, model.Fat, model.Carbohydrate);

            return RedirectToAction("Index", "Home"); // Przekierowanie do dowolnej akcji po zapisie
        }

        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }

        public IActionResult DisplayUserMacronutrients(int userId)
        {
            var user = _dietitianService.GetUserWithUserNutritionalRequirements(userId);

            if (user == null)
            {
                return NotFound();
            }

            var model = new UserMacronutrientsVM
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Calories = user.UserNutritionalRequirement?.Calories ?? 0,
                Protein = user.UserNutritionalRequirement?.Protein ?? 0,
                Fat = user.UserNutritionalRequirement?.Fat ?? 0,
                Carbohydrate = user.UserNutritionalRequirement?.Carbohydrate ?? 0
            };

            return View(model);
        }


        public IActionResult SetOrDisplayUserMacronutrients(int userId)
        {
            var user = _dietitianService.GetUserWithPreferenceAndUserNutritionalRequirements(userId);

            if (user == null)
            {
                return NotFound();
            }

            if (user.UserNutritionalRequirement == null)
            {
                // Jeśli użytkownik nie ma ustawionych makroskładników, przekieruj do akcji SetUserMacronutrients
                return RedirectToAction("SetUserMacronutrients", new { userId = userId });
            }
            else
            {
                // Jeśli użytkownik ma ustawione makroskładniki, przekieruj do akcji DisplayUserMacronutrients
                return RedirectToAction("DisplayUserMacronutrients", new { userId = userId });
            }
        }

        public IActionResult EditUserMacronutrients(int userId)
        {
            var user = _dietitianService.GetUserMacronutrients(userId);

            if (user == null)
            {
                return NotFound();
            }

            var userMacronutrientsVM = new UserMacronutrientsVM
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Calories = user.Calories,
                Protein = user.Protein,
                Fat = user.Fat,
                Carbohydrate = user.Carbohydrate
            };

            return View(userMacronutrientsVM);
        }

        [HttpPost]
        public IActionResult EditUserMacronutrients(UserMacronutrientsVM model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //} brak ubslugi bleDow

            _dietitianService.UpdateUserMacronutrients(model);

            return RedirectToAction("DisplayUserMacronutrients", new { userId = model.UserId });
        }
    }

}

