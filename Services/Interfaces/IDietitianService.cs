using DietBowl.Models;

namespace DietBowl.Services.Interfaces
{
    public interface IDietitianService
    {
        Task<List<User>> GetAllFreePatientsAsync();
        public Task<bool> AddPatient(string dietitianEmail, int userId);
    }
}
