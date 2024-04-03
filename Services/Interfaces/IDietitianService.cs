using DietBowl.Models;

namespace DietBowl.Services.Interfaces
{
    public interface IDietitianService
    {
        Task<List<User>> GetAllFreePatients();
        public Task<List<User>> GetAssignedPatients(int dietitianId);
        public Task<int?> GetDietitianIdByEmail(string email);
        public Task<bool> AddPatient(int dietitianId, int userId);
        public Task<bool> RemovePatient(int dietitianId, int userId);
    }
}
