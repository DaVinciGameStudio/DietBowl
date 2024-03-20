using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DietBowl.EF;
using DietBowl.Models;
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

    }
}