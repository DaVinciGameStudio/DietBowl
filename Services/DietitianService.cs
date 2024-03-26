using DietBowl.EF;
using DietBowl.Models;
using DietBowl.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DietBowl.Services
{
    public class DietitianService : BaseService, IDietitianService
    {
        public DietitianService(DietBowlDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<List<User>> GetAllFreePatientsAsync()
        {
            return await _dietBowlDbContext.Users.Where(u => u.Role == 2 && u.IdDietician == null).ToListAsync();
        }

        public async Task<bool> AddPatient(string dietitianEmail, int userId)
        {
            User? myUser = await _dietBowlDbContext.Users.Where(u => u.Email == dietitianEmail).FirstOrDefaultAsync();
            int idDietitian = myUser.Id;

            var userToAdd = await _dietBowlDbContext.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            userToAdd.Id = idDietitian;

            _dietBowlDbContext.Users.Update(userToAdd);
            await _dietBowlDbContext.SaveChangesAsync();
            return true;
        }
    }
}
