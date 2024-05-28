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

        public async Task<List<User>> GetAllUsers()
        {
            return await _dietBowlDbContext.Users
                .Where(u => u.Role == 2)
                .ToListAsync();
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


        public async Task DeleteUserById(int id)
        {
            var user = await _dietBowlDbContext.Users.FindAsync(id);
            if (user != null)
            {
                _dietBowlDbContext.Users.Remove(user);
                await _dietBowlDbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteDietitianById(int id)
        {
            var dietitian = await _dietBowlDbContext.Users.FindAsync(id);
            if (dietitian != null && dietitian.Role == 1)
            {
                _dietBowlDbContext.Users.Remove(dietitian);
                await _dietBowlDbContext.SaveChangesAsync();
            }
        }

        public async Task RemoveDietitianAndReleasePatients(int dietitianId)
        {
            // Znajdź dietetyka
            var dietitian = await _dietBowlDbContext.Users.FindAsync(dietitianId);

            if (dietitian != null && dietitian.Role == 1)
            {
                // Znajdź pacjentów przypisanych do dietetyka
                var patients = await _dietBowlDbContext.Users
                    .Where(u => u.Role == 2 && u.IdDietician == dietitianId)
                    .ToListAsync();

                // Ustaw IdDietician na NULL dla każdego pacjenta
                foreach (var patient in patients)
                {
                    patient.IdDietician = null;
                }

                // Usuń dietetyka
                _dietBowlDbContext.Users.Remove(dietitian);

                // Zapisz zmiany w bazie danych
                await _dietBowlDbContext.SaveChangesAsync();
            }
        }

    }
}
