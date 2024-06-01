using DietBowl.Models;
using DietBowl.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace DietBowl.Services.Interfaces
{
    public interface IDietitianService
    {
        Task<List<User>> GetAllFreePatients();
        Task<List<User>> GetAssignedPatients(int dietitianId);
        Task<int?> GetUserIdByEmail(string email);
        Task<int?> GetDietitianIdByEmail(string email);
        Task<bool> AddPatient(int dietitianId, int userId);
        Task<bool> RemovePatient(int dietitianId, int userId);
        Task<User> GetUserById(int userId);
        Task UpdateNutritionalRequirements(NutritionalRequirementsVM model);
        Task<List<Recipe>> GetRecipes();
        Task<bool> AddRecipeAtDay(int userId, DateTime day, List<int> idRecipes);
        void SetUserMacronutrients(int userId, double protein, double fat, double carbohydrate);
        User GetUserWithPreference(int userId);
        User GetUserWithUserNutritionalRequirements(int userId);
        User GetUserWithPreferenceAndUserNutritionalRequirements(int userId);
        UserMacronutrientsVM GetUserMacronutrients(int userId);
        void UpdateUserMacronutrients(UserMacronutrientsVM model);
        Task<List<BodyParameter>> GetBodyParameters(int userId);
    }
}
