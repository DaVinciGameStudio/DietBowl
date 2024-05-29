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


        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize(Roles = "1")]
        public IActionResult Add([Bind("Title,Ingedients,Instructions,Protein,Fat,Carbohydrate,Calories")] Recipe recipe, int[] selectedAllergens)
        {
            if (ModelState.IsValid)
            {
                // Calculate calories based on the provided values
                recipe.Calories = recipe.CalculateCalories();

                // Add the recipe to the database
                _dietBowlDbContext.Recipes.Add(recipe);

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

                _dietBowlDbContext.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            return View(recipe);
        }

        [HttpGet]
        [Authorize(Roles = "1")]
        public IActionResult Add()
        {
            // Create an empty Recipe object
            var newRecipe = new Recipe();
            List<Allergen> allergens = _dietBowlDbContext.Allergens.ToList();
            newRecipe.Allergens = allergens;
            // Pass the empty Recipe object to the view
            return View(newRecipe);
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




        // GET: Recipes/Edit/{id}
        [Authorize(Roles = "1")]
        public IActionResult Edit(int id)
        {
            var recipe = _dietBowlDbContext.Recipes
                .Include(r => r.Allergens) // Dodaj Include dla alergenów
                .FirstOrDefault(r => r.Id == id);
            ViewBag.Allergens = _dietBowlDbContext.Allergens.ToList();
            if (recipe == null)
            {
                return NotFound();
            }
            return View(recipe);
        }


        /*
        // POST: Recipes/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "1")]
        public IActionResult Edit(int id, [Bind("Id,Title,Ingedients,Instructions,Protein,Fat,Carbohydrate,Calories")] Recipe recipe)
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
                    _dietBowlDbContext.Update(recipe);
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
        */

        // POST: Recipes/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "1")]
        public IActionResult Edit(int id, [Bind("Id,Title,Ingedients,Instructions,Protein,Fat,Carbohydrate,Calories")] Recipe recipe, int[] selectedAllergens)
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

                    // Update the recipe
                    _dietBowlDbContext.Update(recipe);

                    // Clear existing allergens associated with the recipe
                    var existingRecipe = _dietBowlDbContext.Recipes.Include(r => r.Allergens).FirstOrDefault(r => r.Id == id);
                    if (existingRecipe != null)
                    {
                        existingRecipe.Allergens.Clear();
                    }

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