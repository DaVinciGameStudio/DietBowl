using DietBowl.Services;
using DietBowl.Services.Interfaces;
using DietBowl.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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


        //Makroskladniki
        // Wyświetla formularz do ustawiania zapotrzebowania
        public async Task<IActionResult> SetNutritionalRequirements(int userId)
        {
            var user = await _dietitianService.GetUserById(userId);
            if (user == null)
            {
                return NotFound();
            }

            var model = new NutritionalRequirementsVM
            {
                UserId = user.Id,
                Calories = user.UserNutritionalRequirement?.Calories ?? 0,
                Protein = user.UserNutritionalRequirement?.Protein ?? 0,
                Fat = user.UserNutritionalRequirement?.Fat ?? 0,
                Carbohydrate = user.UserNutritionalRequirement?.Carbohydrate ?? 0,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SetNutritionalRequirements(NutritionalRequirementsVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _dietitianService.UpdateNutritionalRequirements(model);
            return RedirectToAction("AssignedPatients", "Dietitian"); // Domyślna strona po zalogowaniu
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
    }

}

