using DietBowl.Models;

namespace DietBowl.Services.Interfaces
{
    public interface IAdministratorService
    {
        Task<List<User>> GetAllDietitians();
        bool RegisterDietitian(User user);
        Task<List<User>> GetAllUsers();
        Task DeleteUserById(int id);
        Task DeleteDietitianById(int id);
        Task RemoveDietitianAndReleasePatients(int dietitianId);
    }
}
