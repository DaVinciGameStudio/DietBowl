using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DietBowl.EF;
using DietBowl.Models;
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

        public async Task<List<User>> GetAllPatientsAsync()
        {
            return await _dietBowlDbContext.Users.Where(u => u.Role == 2).ToListAsync();
        }

    }
}