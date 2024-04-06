using DietBowl.Models;

namespace DietBowl.Services.Interfaces
{
    public interface IDietitianService
    {
        Task<List<User>> GetAllFreePatients();
        Task<List<User>> GetAssignedPatients(int dietitianId);
        Task<int?> GetDietitianIdByEmail(string email);
        Task<bool> AddPatient(int dietitianId, int userId);
        Task<bool> RemovePatient(int dietitianId, int userId);
    }
}
