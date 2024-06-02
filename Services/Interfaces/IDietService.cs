using DietBowl.Models;

namespace DietBowl.Services.Interfaces
{
    public interface IDietService
    {
        Task<Diet> GetDietForDate(int userId, DateTime date);
        Task<List<Recipe>> GetRecipes();
        Task<Diet> GetDiet(int userId, DateTime date, int dietId);
        Task<bool> AddRecipeAtDay(int userId, DateTime date, List<int> idRecipes);
        Task<bool> EditDiet(Diet model);
        Task<List<Diet>> GetDietsForDietitian(int userId);
        Task<List<Recipe>> GetRecipesInDiet(int dietId);
        Task<Diet> GetDietById(int dietId, int userId);
        Task<bool> DeleteDiet(int dietId, int userId);
        Task<bool> EditDiet(int dietId, List<int> idRecipes);
    }
}
