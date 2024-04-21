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
            return await _dietBowlDbContext.Users
                        .Where(u => u.Role == 2 && u.IdDietician == dietitianId)
                        .ToListAsync();
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
            .ToListAsync();
        }

        public async Task<bool> AddRecipeAtDay(int userId, DateTime day, List<int> idRecipes)
        {
            if(userId > 0)
            {
                List<Recipe> recipes = _dietBowlDbContext.Recipes
                    .Where(r => idRecipes.Contains(r.Id))
                    .ToList();
                User user = _dietBowlDbContext.Users
                    .Where(u => u.Id == userId)
                    .FirstOrDefault()!;


                Diet diet = new Diet{
                Date = day,
                UserId = userId,
                User = user,
                Recipes = recipes
                };
                await _dietBowlDbContext.Diets.AddAsync(diet);
                await _dietBowlDbContext.SaveChangesAsync();
                return true;
            }
            
           return false;
        }
    }
}
