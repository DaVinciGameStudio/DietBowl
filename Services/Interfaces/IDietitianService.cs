using DietBowl.Models;

namespace DietBowl.Services.Interfaces
{
    public interface IDietitianService
    {
        Task<List<User>> GetAllFreePatientsAsync();

        public Task<int?> GetDietitianIdByEmail(string email);
        public Task<bool> AddPatient(int dietitianId, int userId);
    }
}
