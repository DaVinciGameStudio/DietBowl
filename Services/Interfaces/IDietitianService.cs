using DietBowl.Models;

namespace DietBowl.Services.Interfaces
{
    public interface IDietitianService
    {
        Task<List<User>> GetAllPatientsAsync();
    }
}
