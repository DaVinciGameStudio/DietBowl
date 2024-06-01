using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DietBowl.EF;
using DietBowl.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DietBowl.Controllers
{
    public class RecipesController : Controller
    {
        private readonly ILogger<RecipesController> _logger;
        private readonly DietBowlDbContext _dietBowlDbContext;
        public RecipesController(ILogger<RecipesController> logger, DietBowlDbContext dietBowlDbContext)
        {
            _logger = logger;
            _dietBowlDbContext = dietBowlDbContext;
        }

        [HttpGet]
        [Authorize(Roles = "1")]
        public IActionResult Add()
        {
            var newRecipe = new Recipe();
            List<Allergen> allergens = _dietBowlDbContext.Allergens.ToList();
            newRecipe.Allergens = allergens;

            ViewBag.Categories = new List<string> { "Śniadanie", "Obiad", "Podwieczorek", "Kolacja" };
            return View(newRecipe);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize(Roles = "1")]
        public IActionResult Add([Bind("Title,Category,Ingedients,Instructions,Protein,Fat,Carbohydrate,Calories")] Recipe recipe, int[] selectedAllergens)
        {
            if (ModelState.IsValid)
            {
                // Calculate calories based on the provided values
                recipe.Calories = recipe.CalculateCalories();

                // Associate selected allergens with the recipe
                if (selectedAllergens != null)
                {
                    foreach (var allergenId in selectedAllergens)
                    {
                        var allergen = _dietBowlDbContext.Allergens.Find(allergenId);
                        if (allergen != null)
                        {
                            recipe.Allergens.Add(allergen);
                        }
                    }
                }

                // Add the recipe to the database
                _dietBowlDbContext.Recipes.Add(recipe);
                _dietBowlDbContext.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            // Repopulate the ViewBag and allergens list if ModelState is not valid
            ViewBag.Categories = new List<string> { "Śniadanie", "Obiad", "Przekąska", "Kolacja" };
            recipe.Allergens = _dietBowlDbContext.Allergens.ToList();
            return View(recipe);
        }





        public IActionResult Index()
        {
            var recipes = _dietBowlDbContext.Recipes
                .Include(a => a.Allergens)
                .ToList(); // Pobranie wszystkich przepissow z bazy danych
            return View(recipes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "1")]
        public IActionResult Delete(int id)
        {
            var recipe = _dietBowlDbContext.Recipes.Find(id);
            if (recipe == null)
            {
                return NotFound();
            }

            _dietBowlDbContext.Recipes.Remove(recipe);
            _dietBowlDbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }







        //// GET: Recipes/Edit/{id}
        [Authorize(Roles = "1")]
        public IActionResult Edit(int id)
        {
            var recipe = _dietBowlDbContext.Recipes
                .Include(r => r.Allergens)
                .FirstOrDefault(r => r.Id == id);
            if (recipe == null)
            {
                return NotFound();
            }

            ViewBag.Allergens = _dietBowlDbContext.Allergens.ToList();
            ViewBag.Categories = new List<string> { "Śniadanie", "Obiad", "Przekąska", "Kolacja" };

            return View(recipe);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize(Roles = "1")]
        public IActionResult Edit(int id, [Bind("Id,Title,Category,Ingedients,Instructions,Protein,Fat,Carbohydrate,Calories")] Recipe recipe, int[] selectedAllergens)
        {
            if (id != recipe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    recipe.Calories = recipe.CalculateCalories(); // Recalculate calories

                    // Fetch the existing recipe with its allergens
                    var existingRecipe = _dietBowlDbContext.Recipes.Include(r => r.Allergens).FirstOrDefault(r => r.Id == id);
                    if (existingRecipe == null)
                    {
                        return NotFound();
                    }

                    // Clear existing allergens associated with the recipe
                    existingRecipe.Allergens.Clear();

                    // Associate selected allergens with the recipe
                    if (selectedAllergens != null)
                    {
                        foreach (var allergenId in selectedAllergens)
                        {
                            var allergen = _dietBowlDbContext.Allergens.Find(allergenId);
                            if (allergen != null)
                            {
                                existingRecipe.Allergens.Add(allergen);
                            }
                        }
                    }

                    // Update the existing recipe with new values
                    existingRecipe.Title = recipe.Title;
                    existingRecipe.Category = recipe.Category;
                    existingRecipe.Ingedients = recipe.Ingedients;
                    existingRecipe.Instructions = recipe.Instructions;
                    existingRecipe.Protein = recipe.Protein;
                    existingRecipe.Fat = recipe.Fat;
                    existingRecipe.Carbohydrate = recipe.Carbohydrate;
                    existingRecipe.Calories = recipe.Calories;

                    _dietBowlDbContext.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_dietBowlDbContext.Recipes.Any(e => e.Id == recipe.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(recipe);
        }
    }
}