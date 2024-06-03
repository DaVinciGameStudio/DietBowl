using DietBowl.EF;
using DietBowl.Models;
using DietBowl.Services;
using DietBowl.Services.Interfaces;
using DietBowl.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Collections.Generic;
using System.Security.Claims;

namespace DietBowl.Controllers
{
    public class DietController : Controller
    {

        private readonly IDietService _dietService;

        private readonly DietBowlDbContext _dietBowlDbContext;

        public DietController(IDietService dietService, DietBowlDbContext dietBowlDbContext)
        {
            _dietService = dietService;
            _dietBowlDbContext = dietBowlDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> DietsCallendar()
        {

            var emailUser = User.FindFirstValue(ClaimTypes.Name);
            var user = await _dietBowlDbContext.Users
                        .Include(b => b.BodyParameters)
                        .FirstOrDefaultAsync(u => u.Email == emailUser && u.Role == 2);
            var userId = user.Id;

            // Pobierz wszystkie daty z bazy danych dla danego użytkownika
            var diets = await _dietBowlDbContext.Diets
                            .Where(d => d.UserId == userId)
                            .Select(d => d.Date)
                            .ToListAsync();

            // Utwórz słownik, w którym kluczem będzie data, a wartością będzie lista obiektów Diet dla tej daty
            var dietDictionary = diets.GroupBy(d => d.Date.Date)
                                      .ToDictionary(g => g.Key, g => g.ToList());

            // Przekazanie danych do widoku
            ViewData["DietDictionary"] = dietDictionary;
            ViewBag.userId = userId;
            ViewBag.userBody = user.BodyParameters.Any(x => x.Date.Date == DateTime.Today);
            return View(user);
        }


        public async Task<IActionResult> DietsCallendarShow(DateTime date)
        {
            var emailUser = User.FindFirstValue(ClaimTypes.Name);
            var user = await _dietBowlDbContext.Users
                        .FirstOrDefaultAsync(u => u.Email == emailUser && u.Role == 2);
            var userId = user.Id;

            // Pobieramy dietę na podstawie wybranej daty i ID użytkownika
            var diet = await _dietBowlDbContext.Diets
                .Include(d => d.DietRecipes)
                .ThenInclude(r => r.Recipe)
                .Where(d => d.Date.Date == date.Date && d.UserId == userId) // Uwzględniamy datę i ID użytkownika
                .FirstOrDefaultAsync();

            if (diet == null)
            {
                // Przekazanie daty do akcji "NoDietForDay"
                return RedirectToAction("NoDietForDay", new { date = date.ToString("yyyy-MM-dd") });
            }

            var totalCalories = diet.DietRecipes.Sum(dr => dr.Recipe.Calories);
            var totalCarbohydrate = diet.DietRecipes.Sum(dr => dr.Recipe.Carbohydrate);
            var totalFat = diet.DietRecipes.Sum(dr => dr.Recipe.Fat);
            var totalProtein = diet.DietRecipes.Sum(dr => dr.Recipe.Protein);

            var consumedDietRecipes = diet.DietRecipes.Where(dr => dr.IsConsumed);

            var totalCaloriesUse = consumedDietRecipes.Sum(dr => dr.Recipe.Calories);
            var totalCarbohydrateUse = consumedDietRecipes.Sum(dr => dr.Recipe.Carbohydrate);
            var totalFatUse = consumedDietRecipes.Sum(dr => dr.Recipe.Fat);
            var totalProteinUse = consumedDietRecipes.Sum(dr => dr.Recipe.Protein);

            ViewBag.TotalCalories = totalCalories;
            ViewBag.TotalCarbohydrate = totalCarbohydrate;
            ViewBag.TotalFat = totalFat;
            ViewBag.TotalProtein = totalProtein;

            ViewBag.TotalCaloriesUse = totalCaloriesUse;
            ViewBag.TotalCarbohydrateUse = totalCarbohydrateUse;
            ViewBag.TotalFatUse = totalFatUse;
            ViewBag.TotalProteinUse = totalProteinUse;

            return View(diet);
        }


        [HttpPost]
        public async Task<IActionResult> MarkAsConsumed(int dietId, int recipeId, int userId)
        {
            var dietRecipe = await _dietBowlDbContext.DietRecipes
                .Include(d => d.Diet)
                .FirstOrDefaultAsync(dr => dr.DietId == dietId && dr.RecipeId == recipeId);

            if (dietRecipe == null)
            {
                return NotFound();
            }

            dietRecipe.IsConsumed = true;
            _dietBowlDbContext.Update(dietRecipe);
            await _dietBowlDbContext.SaveChangesAsync();

            return RedirectToAction("DietsCallendarShow", new { date = dietRecipe.Diet.Date.ToString("yyyy-MM-dd"), userId = userId });
        }


        // Nowa akcja dla wyświetlania widoku "NoDietForDay"
        public IActionResult NoDietForDay(string date)
        {
            ViewBag.Date = date;
            return View();
        }



        //Widok kalendarza dla dietetyka
        public async Task<IActionResult> DietsCallendarForDietitian(int userId)
        {
            // Pobierz wszystkie daty z bazy danych dla danego użytkownika
            var diets = await _dietBowlDbContext.Diets
                            .Where(d => d.UserId == userId)
                            .ToListAsync();

            // Utwórz słownik, w którym kluczem będzie data, a wartością będzie lista obiektów Diet dla tej daty
            var dietDictionary = diets.GroupBy(d => d.Date.Date)
                                      .ToDictionary(g => g.Key, g => g.ToList());

            // Przekazanie danych do widoku
            ViewData["DietDictionary"] = dietDictionary;
            ViewBag.userId = userId;
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> ViewDiet(int userId, DateTime date)
        {
            var diet = await _dietService.GetDietForDate(userId, date);
            if (diet == null)
            {
                return NotFound("Diet not found for the specified date.");
            }

            return View(diet);
        }


        //[HttpGet]
        //public async Task<IActionResult> EditDiet(int userId, DateTime date, int dietId)
        //{
        //    var diet = await _dietService.GetRecipesInDiet(dietId);
        //    var recipes = await _dietService.GetRecipes();

        //    // Przekazujemy listę przepisów i dane diety do edycji do widoku
        //    return View(diet);
        //}


        [HttpGet]
        public async Task<IActionResult> EditDiet(int userId, DateTime date, int dietId)
        {
            List<Recipe> dietRecipes = await _dietService.GetRecipesInDiet(dietId);
            List<Recipe> allRecipes = await _dietService.GetRecipes();
            var allJsRecipes = await _dietBowlDbContext.Recipes
                .Select(r => new
                {
                    r.Id,
                    r.Title,
                    r.Fat,
                    r.Carbohydrate,
                    r.Protein,
                    r.Calories
                })
                .ToListAsync();
            List<int> listIds = new List<int>();
            foreach (var recipe in dietRecipes)
            {
                listIds.Add(recipe.Id);
            }

            // Przekazujemy listę przepisów w diecie i wszystkie przepisy do widoku

            ViewData["List"] = allRecipes.Select(x => x.Title).ToJson();
            ViewBag.dietId = dietId;
            ViewBag.userId = userId;
            ViewBag.DietRecipes = listIds;
            ViewData["AllRecipes"] = allRecipes;
            ViewBag.allJsRecipes = allJsRecipes;

            UserNutritionalRequirement? userNutritionalRequirement = await _dietBowlDbContext.UserNutritionalRequirements
                                .FirstOrDefaultAsync(ur => ur.UserId == userId);

            if (userNutritionalRequirement != null)
            {
                ViewBag.Calories = userNutritionalRequirement.Calories;
                ViewBag.Protein = userNutritionalRequirement.Protein;
                ViewBag.Fat = userNutritionalRequirement.Fat;
                ViewBag.Carbohydrate = userNutritionalRequirement.Carbohydrate;
            }

            return View(dietRecipes);
        }

        [HttpPost]
        public async Task<IActionResult> EditDiet(int dietId, string recipeList)
        {
            dynamic jsonData = JsonConvert.DeserializeObject(recipeList)!;
            List<int> idRecipes = new();
            foreach (int idRepice in jsonData)
            {
                idRecipes.Add(idRepice);
            }

            await _dietService.EditDiet(dietId, idRecipes);
            //await _dietitianService.AddRecipeAtDay(userId, date, idRecipes);
            return RedirectToAction("AssignedPatients", "Dietitian");
        }


        //Usuwanie diety
        [HttpGet]
        public async Task<IActionResult> DeleteDiet(int dietId, int userId, string date)
        {
            bool success = await _dietService.DeleteDiet(dietId, userId);

            if (!success)
            {
                return NotFound("Diet not found.");
            }

            return RedirectToAction("DietsCallendarForDietitian", new { userId = userId });
        }
    }
}
