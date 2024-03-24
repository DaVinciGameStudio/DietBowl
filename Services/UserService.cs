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
                bool usernameTaken = _dietBowlDbContext.Users.Any(u => u.FirstName == user.FirstName);

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

        public async Task<ClaimsPrincipal> Login(UserVM user)
        {
            User ?foundUser = null;
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (regex.Match(user.Username).Success)
            {
                foundUser = await _dietBowlDbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Username);
            }

            bool checkUserCredentials = BCrypt.Net.BCrypt.Verify(user.Password, foundUser!.Password);
            if (checkUserCredentials)
            {
                var userRole = _dietBowlDbContext.Users.FirstOrDefault(u => u.Role == foundUser.Role)!;
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Role, foundUser.Role.ToString()),
                    new Claim(ClaimTypes.Email, foundUser.Email),
                    new Claim(ClaimTypes.NameIdentifier, foundUser.Id.ToString())
                };


                var userIdentity = new ClaimsIdentity(claims, "login");

                ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);

                return principal;
            }
            else
                return null;
        }

    }
}