using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DietBowl.Models;
using DietBowl.ViewModel;

namespace DietBowl.Services.Interfaces
{
    public interface IUserService
    {
        bool Register(User user);
        Task<ClaimsPrincipal> Login(UserVM user);
        Task<int?> GetUserIdByEmail(string email);
        Task<List<BodyParameter>> GetBodyParameters(int userId);
        Task AddUserPreference(int userId, Preference preference);
        Task<User> GetUserById(int userId);
        Task<Preference> GetUserPreferences(int userId);
        Task<BodyParameter> GetBodyParametersById(int id);
    }
}