using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DietBowl.EF;
using DietBowl.Models;
using DietBowl.Services.Interfaces;
using DietBowl.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;

namespace DietBowl.Services
{
    public class UserService : BaseService, IUserService
    {

        readonly string salt = "$2a$10$3X5I5A7K.YQZW6XolhO6ce";
        public UserService(DietBowlDbContext context, ILogger<UserService> logger, IOptions<Strings> settings) : base(context)
        {
            // _logger = logger;
            // _salt = settings.Value.Salt;
        }

        public bool Register(User user)
        {
            try
            {
                bool usernameTaken = _dietBowlDbContext.Users.Any(u => u.Email == user.Email);

                if (!usernameTaken)
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, salt);
                    user.Role = 2;
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

        public async Task<ClaimsPrincipal?> Login(UserVM user)
        {
            try
            {
                User? foundUser = null;
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                if (regex.Match(user.Email).Success)
                    foundUser = await _dietBowlDbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);


                Console.WriteLine(foundUser);
                if (foundUser != null)
                {
                    if (BCrypt.Net.BCrypt.Verify(user.Password, foundUser.Password))
                    {
                        List<Claim> claims = new()
                        {
                            new Claim(ClaimTypes.Role, foundUser.Role.ToString()),
                            new Claim(ClaimTypes.Name, foundUser.Email),
                            //new Claim(ClaimTypes.Email, foundUser.Email), //dodane
                        };
                        ClaimsIdentity userClaims = new(claims, "login");

                        return new ClaimsPrincipal(userClaims);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                return null;
            }
        }

        public async Task<int?> GetUserIdByEmail(string email)
        {
            var user = await _dietBowlDbContext.Users
                                .FirstOrDefaultAsync(u => u.Email == email && u.Role == 2);

            return user?.Id; // Zwróć ID user lub null, jeśli nie znaleziono
        }

        public async Task<List<BodyParameter>> GetBodyParameters(int userId)
        {
            return await _dietBowlDbContext.BodyParameters.Where(bp => bp.UserId == userId).ToListAsync();
        }


        //Preferencje
        public async Task<Preference> GetUserPreferences(int userId)
        {
            // Pobierz preferencje użytkownika na podstawie podanego userId
            var userPreferences = await _dietBowlDbContext.Preferences
                .Include(a=>a.Allergens)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            return userPreferences;
        }

        public async Task AddUserPreference(int userId, Preference preference)
        {
            var user = await _dietBowlDbContext.Users.FindAsync(userId);

            if (user == null)
            {
                //throw new NotFoundException("User not found");
                await Console.Out.WriteLineAsync("Nie ma uzytkownika");
            }

            // Ustaw userId dla preferencji, jeśli nie został już ustawiony
            preference.UserId = userId;

            // Dodaj preferencję do kontekstu bazy danych
            await _dietBowlDbContext.Preferences.AddAsync(preference);
            await _dietBowlDbContext.SaveChangesAsync();
        }

        public async Task<User> GetUserById(int userId)
        {
            var user = await _dietBowlDbContext.Users
                .AsNoTracking() // Opcjonalnie, zwiększa wydajność zapytania, jeśli nie planujesz edytować tego obiektu
                .Include(u => u.Preference) // Dołącz informacje o preferencjach użytkownika, jeśli są potrzebne
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user;
        }

        public async Task<BodyParameter> GetBodyParametersById(int id)
        {
            return await _dietBowlDbContext.BodyParameters.FirstAsync(bp => bp.Id == id);
        }
    }

}