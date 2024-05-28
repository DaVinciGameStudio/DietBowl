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

        // GET: Recipes /Add
        public IActionResult Add()
        {
            return View();
        }

        // POST: Recipes /Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "1")]
        public IActionResult Add([Bind("Title,Ingedients,Instructions,Protein,Fat,Carbohydrate,Calories")] Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                // Obliczanie kalorii na podstawie podanych wartości
                recipe.Calories = recipe.CalculateCalories();

                // Dodanie przepisu do bazy danych
                _dietBowlDbContext.Recipes.Add(recipe);
                _dietBowlDbContext.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            return View(recipe);
        }

        public IActionResult Index()
        {
            var recipes = _dietBowlDbContext.Recipes.ToList(); // Pobranie wszystkich przepissow z bazy danych
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




        //Ostatnie szlify
        // GET: Recipes/Edit/{id}
        [Authorize(Roles = "1")]
        public IActionResult Edit(int id)
        {
            var recipe = _dietBowlDbContext.Recipes.Find(id);
            if (recipe == null)
            {
                return NotFound();
            }
            return View(recipe);
        }

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
    }
}