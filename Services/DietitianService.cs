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

        public async Task<List<User>> GetAllFreePatients()
        {
            return await _dietBowlDbContext.Users.Where(u => u.Role == 2 && u.IdDietician == null).ToListAsync();
        }

        public async Task<List<User>> GetAssignedPatients(int dietitianId)
        {
            return await _dietBowlDbContext.Users
                        .Where(u => u.Role == 2 && u.IdDietician == dietitianId)
                        .ToListAsync();
        }

        public async Task<int?> GetDietitianIdByEmail(string email)
        {
            var dietitian = await _dietBowlDbContext.Users
                                .FirstOrDefaultAsync(u => u.Email == email && u.Role == 1);

            return dietitian?.Id; // Zwróć ID dietetyka lub null, jeśli nie znaleziono
        }

        public async Task<bool> AddPatient(int dietitianId, int userId)
        {
            User? dietitianUser = await _dietBowlDbContext.Users.FirstOrDefaultAsync(u => u.Id == dietitianId && u.Role == 1); // dietetyk
            if (dietitianUser == null)
            {
                // dietetyk nie został znaleziony
                await Console.Out.WriteLineAsync("dietetyk nie został znaleziony");
                return false;
            }

            var userToAdd = await _dietBowlDbContext.Users.FirstOrDefaultAsync(u => u.Id == userId && u.Role == 2); // pacjent
            if (userToAdd == null)
            {
                // pacjent nie został znaleziony
                await Console.Out.WriteLineAsync("pacjent nie został znaleziony");
                return false;
            }

            userToAdd.IdDietician = dietitianUser.Id; // przypisanie id dietetyka do id pacjenta

            _dietBowlDbContext.Users.Update(userToAdd);
            await _dietBowlDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemovePatient(int dietitianId, int userId)
        {
            try
            {
                var user = await _dietBowlDbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user != null)
                {
                    // Sprawdź, czy użytkownik jest aktualnie przypisany do tego dietetyka
                    if (user.IdDietician == dietitianId)
                    {
                        user.IdDietician = null;
                        await _dietBowlDbContext.SaveChangesAsync();

                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                // Obsługa błędu
                Console.WriteLine($"Wystąpił błąd podczas usuwania pacjenta: {ex.Message}");
                return false;
            }
        }
    }
}
