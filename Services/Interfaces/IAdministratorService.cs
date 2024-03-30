using DietBowl.Models;

namespace DietBowl.Services.Interfaces
{
    public interface IAdministratorService
    {
        Task<List<User>> GetAllDietitians();
        bool RegisterDietitian(User user);
    }
}
