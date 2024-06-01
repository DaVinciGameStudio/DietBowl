using DietBowl.EF;
using DietBowl.Models;
using DietBowl.Services.Interfaces;
using DietBowl.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DietBowl.Services
{
    public class DietitianService : BaseService, IDietitianService
    {
        public DietitianService(DietBowlDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<List<User>> GetAllFreePatients()
        {
            return await _dietBowlDbContext.Users.Where(u => u.Role == 2 && u.IdDietician == null).ToListAsync();
        }

        public async Task<List<User>> GetAssignedPatients(int dietitianId)
        {
            return await _dietBowlDbContext.Users.Include(u => u.UserNutritionalRequirement)
                        .Where(u => u.Role == 2 && u.IdDietician == dietitianId)
                        .ToListAsync();
        }

        public async Task<int?> GetUserIdByEmail(string email)
        {
            var user = await _dietBowlDbContext.Users
                                .FirstOrDefaultAsync(u => u.Email == email && u.Role == 2);

            return user?.Id; // Zwróć ID user lub null, jeśli nie znaleziono
        }

        public async Task<int?> GetDietitianIdByEmail(string email)
        {
            var dietitian = await _dietBowlDbContext.Users
                                .FirstOrDefaultAsync(u => u.Email == email && u.Role == 1);

            return dietitian?.Id; // Zwróć ID dietetyka lub null, jeśli nie znaleziono
        }

        public async Task<bool> AddPatient(int dietitianId, int userId)
        {
            User? dietitianUser = await _dietBowlDbContext.Users.FirstOrDefaultAsync(u => u.Id == dietitianId && u.Role == 1); // dietetyk
            if (dietitianUser == null)
            {
                // dietetyk nie został znaleziony
                await Console.Out.WriteLineAsync("dietetyk nie został znaleziony");
                return false;
            }

            var userToAdd = await _dietBowlDbContext.Users.FirstOrDefaultAsync(u => u.Id == userId && u.Role == 2); // pacjent
            if (userToAdd == null)
            {
                // pacjent nie został znaleziony
                await Console.Out.WriteLineAsync("pacjent nie został znaleziony");
                return false;
            }

            userToAdd.IdDietician = dietitianUser.Id; // przypisanie id dietetyka do id pacjenta

            _dietBowlDbContext.Users.Update(userToAdd);
            await _dietBowlDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemovePatient(int dietitianId, int userId)
        {
            try
            {
                var user = await _dietBowlDbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user != null)
                {
                    // Sprawdź, czy użytkownik jest aktualnie przypisany do tego dietetyka
                    if (user.IdDietician == dietitianId)
                    {
                        user.IdDietician = null;
                        await _dietBowlDbContext.SaveChangesAsync();

                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                // Obsługa błędu
                Console.WriteLine($"Wystąpił błąd podczas usuwania pacjenta: {ex.Message}");
                return false;
            }
        }


        //Makrosklaniki
        public async Task<User> GetUserById(int userId)
        {
            return await _dietBowlDbContext.Users
                .Include(u => u.UserNutritionalRequirement)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task UpdateNutritionalRequirements(NutritionalRequirementsVM model)
        {
            var user = await _dietBowlDbContext.Users
                .Include(u => u.UserNutritionalRequirement)
                .FirstOrDefaultAsync(u => u.Id == model.UserId);

            if (user != null)
            {
                if (user.UserNutritionalRequirement == null)
                {
                    user.UserNutritionalRequirement = new UserNutritionalRequirement();
                }

                user.UserNutritionalRequirement.Calories = model.Calories;
                user.UserNutritionalRequirement.Protein = model.Protein;
                user.UserNutritionalRequirement.Fat = model.Fat;
                user.UserNutritionalRequirement.Carbohydrate = model.Carbohydrate;

                await _dietBowlDbContext.SaveChangesAsync();
            }
        }

        public async Task<List<Recipe>> GetRecipes()
        {
            return await _dietBowlDbContext.Recipes
                .Include(a=>a.Allergens)
                .ToListAsync();
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

        public void SetUserMacronutrients(int userId, double protein, double fat, double carbohydrate)
        {
            var userNutritionalRequirement = new UserNutritionalRequirement
            {
                UserId = userId,
                Calories = CalculateCalories(protein, fat, carbohydrate),
                Protein = protein,
                Fat = fat,
                Carbohydrate = carbohydrate
            };

            _dietBowlDbContext.UserNutritionalRequirements.Add(userNutritionalRequirement);
            _dietBowlDbContext.SaveChanges();
        }

        private double CalculateCalories(double protein, double fat, double carbohydrate)
        {
            return (protein * 4) + (fat * 9) + (carbohydrate * 4);
        }

        public User GetUserWithPreference(int userId)
        {
            return _dietBowlDbContext.Users.Include(u => u.Preference).FirstOrDefault(u => u.Id == userId);
        }

        public User GetUserWithUserNutritionalRequirements(int userId)
        {
            return _dietBowlDbContext.Users.Include(u => u.UserNutritionalRequirement).FirstOrDefault(u => u.Id == userId);
        }

        public User GetUserWithPreferenceAndUserNutritionalRequirements(int userId)
        {
            return _dietBowlDbContext.Users.Include(u => u.Preference).Include(u => u.UserNutritionalRequirement).FirstOrDefault(u => u.Id == userId);
        }

        public UserMacronutrientsVM GetUserMacronutrients(int userId)
        {
            var user = _dietBowlDbContext.Users
                .Include(u => u.UserNutritionalRequirement)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return null;
            }

            var userMacronutrientsVM = new UserMacronutrientsVM
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Calories = user.UserNutritionalRequirement != null ? user.UserNutritionalRequirement.Calories : 0,
                Protein = user.UserNutritionalRequirement != null ? user.UserNutritionalRequirement.Protein : 0,
                Fat = user.UserNutritionalRequirement != null ? user.UserNutritionalRequirement.Fat : 0,
                Carbohydrate = user.UserNutritionalRequirement != null ? user.UserNutritionalRequirement.Carbohydrate : 0
            };

            return userMacronutrientsVM;
        }

        public void UpdateUserMacronutrients(UserMacronutrientsVM model)
        {
            // Oblicz kalorie na podstawie podanych wartości węglowodanów, białka i tłuszczu
            double calories = CalculateCalories(model.Protein, model.Fat, model.Carbohydrate);

            var user = _dietBowlDbContext.Users
                .Include(u => u.UserNutritionalRequirement)
                .FirstOrDefault(u => u.Id == model.UserId);

            if (user != null)
            {
                if (user.UserNutritionalRequirement == null)
                {
                    // Jeśli użytkownik nie ma jeszcze ustawionych wymagań dotyczących makroskładników, stwórz nowy obiekt
                    user.UserNutritionalRequirement = new UserNutritionalRequirement();
                }

                // Aktualizuj wartości makroskładników na podstawie danych z formularza
                user.UserNutritionalRequirement.Calories = calories; // Ustaw obliczone kalorie
                user.UserNutritionalRequirement.Protein = model.Protein;
                user.UserNutritionalRequirement.Fat = model.Fat;
                user.UserNutritionalRequirement.Carbohydrate = model.Carbohydrate;

                _dietBowlDbContext.SaveChanges();
            }
        }

        public async Task<List<BodyParameter>> GetBodyParameters(int userId)
        {
            return await _dietBowlDbContext.BodyParameters.Where(bp => bp.UserId == userId).ToListAsync();
        }
    }
}
