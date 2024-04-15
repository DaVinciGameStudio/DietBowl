using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DietBowl.EF;
using DietBowl.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        // GET: Recipes/Add
        public IActionResult Add()
        {
            return View();
        }

        // POST: Recipes/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "1")]
        public IActionResult Add([Bind("Title,Ingredients,Instructions,Protein,Fat,Carbohydrate,Calories")] Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                // Dodanie przepisu do bazy danych
                _dietBowlDbContext.Recipes.Add(recipe);
                _dietBowlDbContext.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            return View(recipe);
        }

        public IActionResult Index()
        {
            var recipes = _dietBowlDbContext.Recipes.ToList(); // Pobranie wszystkich przepisÃ³w z bazy danych
            return View(recipes);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}