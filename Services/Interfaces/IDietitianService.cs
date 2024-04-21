using DietBowl.Models;
using DietBowl.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace DietBowl.Services.Interfaces
{
    public interface IDietitianService
    {
        Task<List<User>> GetAllFreePatients();
        Task<List<User>> GetAssignedPatients(int dietitianId);
        Task<int?> GetDietitianIdByEmail(string email);
        Task<bool> AddPatient(int dietitianId, int userId);
        Task<bool> RemovePatient(int dietitianId, int userId);
        Task<User> GetUserById(int userId);
        Task UpdateNutritionalRequirements(NutritionalRequirementsVM model);
        Task<List<Recipe>> GetRecipes();
        Task<bool> AddRecipeAtDay(int userId, DateTime day, List<int> idRecipes);

    }
}
