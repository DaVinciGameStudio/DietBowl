using DietBowl.EF;
using DietBowl.Models;
using DietBowl.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DietBowl.Services
{
    public class DietitianService: BaseService, IDietitianService
    {
        public DietitianService(DietBowlDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<List<User>> GetAllPatientsAsync()
        {
            return await _dietBowlDbContext.Users.Where(u => u.Role == 2).ToListAsync();
        }
    }
}
