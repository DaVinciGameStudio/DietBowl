using DietBowl.EF;
using DietBowl.Models;
using DietBowl.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DietBowl.Services
{
    public class AdministratorService : BaseService, IAdministratorService
    {
        readonly string salt = "$2a$10$3X5I5A7K.YQZW6XolhO6ce";

        public AdministratorService(DietBowlDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<List<User>> GetAllDietitians()
        {
            return await _dietBowlDbContext.Users.Where(u => u.Role == 1 && u.IdDietician == null).ToListAsync();
        }

        public bool RegisterDietitian(User user)
        {
            try
            {
                bool usernameTaken = _dietBowlDbContext.Users.Any(u => u.Email == user.Email);

                if (!usernameTaken)
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, salt);
                    user.Role = 1;
                    _dietBowlDbContext.Users.Add(user);
                    _dietBowlDbContext.SaveChanges();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Wystąpił błąd podczas rejestracji.");
                throw;
            }
        }
    }
}
