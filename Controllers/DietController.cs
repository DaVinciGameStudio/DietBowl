using DietBowl.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DietBowl.Controllers
{
    public class DietController : Controller
    {

        private readonly DietBowlDbContext _dietBowlDbContext;

        public DietController(DietBowlDbContext dietBowlDbContext)
        {
            _dietBowlDbContext = dietBowlDbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        //public async Task<IActionResult> DietsCallendar()
        //{
        //    var emailUser = User.FindFirstValue(ClaimTypes.Name);
        //    var user = await _dietBowlDbContext.Users
        //                .FirstOrDefaultAsync(u => u.Email == emailUser && u.Role == 2);
        //    var userId = user.Id;
        //    // Pobierz wszystkie daty z bazy danych dla danego użytkownika
        //    var diets = await _dietBowlDbContext.Diets
        //                    .Where(d => d.UserId == userId)
        //                    .ToListAsync();

        //    // Utwórz słownik, w którym kluczem będzie data, a wartością będzie lista obiektów Diet dla tej daty
        //    var dietDictionary = diets.GroupBy(d => d.Date.Date)
        //                              .ToDictionary(g => g.Key, g => g.ToList());

        //    // Przekazanie danych do widoku
        //    ViewData["DietDictionary"] = dietDictionary;
        //    ViewBag.userId = userId;
        //    return View();
        //}

        public async Task<IActionResult> DietsCallendar()
        {
            var emailUser = User.FindFirstValue(ClaimTypes.Name);
            var user = await _dietBowlDbContext.Users
                        .FirstOrDefaultAsync(u => u.Email == emailUser && u.Role == 2);
            var userId = user.Id;

            // Pobierz wszystkie daty z bazy danych dla danego użytkownika
            var dietData = await _dietBowlDbContext.Diets
                                .Where(d => d.UserId == userId)
                                .Select(d => new { Date = d.Date.Date.ToString("yyyy-MM-dd"), UserId = d.UserId })
                                .ToListAsync();

            // Przekazanie danych do widoku
            ViewData["DietData"] = dietData;
            ViewBag.userId = userId;
            return View();
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
                // Możesz zdecydować, co zrobić, jeśli nie ma diety na wybrany dzień
                return RedirectToAction("NoDietForDay");
            }

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
        public IActionResult NoDietForDay()
        {
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
    }
}
