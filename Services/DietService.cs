using DietBowl.EF;
using DietBowl.Models;
using DietBowl.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DietBowl.Services
{
    public class DietService : BaseService, IDietService
    {
        public DietService(DietBowlDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Diet> GetDietForDate(int userId, DateTime date)
        {
            return await _dietBowlDbContext.Diets
                .Include(u => u.User)
                .Include(d => d.DietRecipes)
                .ThenInclude(dr => dr.Recipe)
                .FirstOrDefaultAsync(d => d.UserId == userId && d.Date.Date == date.Date);
        }

        public async Task<List<Recipe>> GetRecipes()
        {
            return await _dietBowlDbContext.Recipes
                .Include(a => a.Allergens)
                .ToListAsync();
        }

        public async Task<Diet> GetDiet(int userId, DateTime date, int dietId)
        {
            // Implementacja pobierania diety z bazy danych
            var diet = await _dietBowlDbContext.Diets.FindAsync(dietId);
            return diet;
        }

        public async Task<List<Recipe>> GetRecipesInDiet(int dietId)
        {
            var diet = await _dietBowlDbContext.Diets
                .Include(d => d.DietRecipes)
                .ThenInclude(dr => dr.Recipe)
                .FirstOrDefaultAsync(d => d.Id == dietId);

            if (diet != null)
            {
                return diet.DietRecipes.Select(dr => dr.Recipe).ToList();
            }
            else
            {
                return new List<Recipe>(); // Możesz zwrócić pustą listę lub null, w zależności od potrzeb
            }
        }

        public async Task<bool> AddRecipeAtDay(int userId, DateTime date, List<int> idRecipes)
        {
            if (userId > 0)
            {
                List<Recipe> recipes = _dietBowlDbContext.Recipes
                    .Where(r => idRecipes.Contains(r.Id))
                    .ToList();
                User user = _dietBowlDbContext.Users
                    .Where(u => u.Id == userId)
                    .FirstOrDefault()!;

                Diet diet = new Diet
                {
                    Date = date,
                    UserId = userId,
                    User = user,
                    DietRecipes = recipes.Select(recipe => new DietRecipe
                    {
                        RecipeId = recipe.Id,
                        Recipe = recipe,
                        IsConsumed = false
                    }).ToList()
                };

                diet.Protein = diet.DietRecipes.Sum(dr => dr.Recipe.Protein);
                diet.Fat = diet.DietRecipes.Sum(dr => dr.Recipe.Fat);
                diet.Carbohydrate = diet.DietRecipes.Sum(dr => dr.Recipe.Carbohydrate);
                diet.Calories = diet.DietRecipes.Sum(dr => dr.Recipe.CalculateCalories());

                await _dietBowlDbContext.Diets.AddAsync(diet);
                await _dietBowlDbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }


        public async Task<bool> EditDiet(Diet model)
        {
            try
            {
                var existingDiet = await _dietBowlDbContext.Diets.FindAsync(model.Id);

                if (existingDiet != null)
                {
                    existingDiet.Date = model.Date;
                    existingDiet.Protein = model.Protein;
                    existingDiet.Fat = model.Fat;
                    existingDiet.Carbohydrate = model.Carbohydrate;
                    existingDiet.Calories = model.Calories;

                    await _dietBowlDbContext.SaveChangesAsync();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error editing diet: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Diet>> GetDietsForDietitian(int userId)
        {
            return await _dietBowlDbContext.Diets
                .Where(d => d.UserId == userId)
                .ToListAsync();
        }
    }
}
