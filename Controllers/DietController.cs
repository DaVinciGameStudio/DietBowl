using DietBowl.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public IActionResult DietsCalendar()
        {
            return View();
        }
        public async Task<IActionResult> DietsCalendarShow(DateTime date)
        {
            // Pobieramy dietę na podstawie wybranej daty
            var diet = await _dietBowlDbContext.Diets
                .Include(d => d.DietRecipes)
                .ThenInclude(r => r.Recipe)
                .Where(d => d.Date.Date == date.Date) // Uwzględniamy tylko datę, bez czasu
                .FirstOrDefaultAsync();

            if (diet == null)
            {
                // Możesz zdecydować, co zrobić, jeśli nie ma diety na wybrany dzień
                return RedirectToAction("NoDietForDay");
            }

            return View(diet);
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsConsumed(int dietId, int recipeId)
        {
            var dietRecipe = await _dietBowlDbContext.DietRecipes
                .Include(d=>d.Diet)
                .FirstOrDefaultAsync(dr => dr.DietId == dietId && dr.RecipeId == recipeId);

            if (dietRecipe == null)
            {
                return NotFound();
            }

            dietRecipe.IsConsumed = true;
            _dietBowlDbContext.Update(dietRecipe);
            await _dietBowlDbContext.SaveChangesAsync();

            return RedirectToAction("DietsCalendarShow", new { date = dietRecipe.Diet.Date.ToString("yyyy-MM-dd") });
        }


        // Nowa akcja dla wyświetlania widoku "NoDietForDay"
        public IActionResult NoDietForDay()
        {
            return View();
        }
    }
}
