using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DietBowl.Models;

namespace DietBowl.Services
{
    public interface IUserService
    {
        bool Register(User user);
    }
}