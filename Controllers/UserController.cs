using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DietBowl.EF;
using DietBowl.Models;
using DietBowl.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DietBowl.Controllers
{
    public class UserController : Controller
    {

        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _configuration;
        private readonly DietBowlDbContext _dietBowlDbContext;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IConfiguration configuration, DietBowlDbContext dietBowlDbContext, IUserService userService)
        {
            _logger = logger;
            _configuration = configuration;
            _dietBowlDbContext = dietBowlDbContext;
            _userService = userService;
        }

        [HttpPost]
        public IActionResult Register([FromForm] User user)
        {
            ViewData["Title"] = "Rejestracja";
            var isUserAdded = _userService.Register(user);

            if (isUserAdded)
            {
                ViewBag.UserAdded = true;
                return Json(new { redirectToUrl = Url.Action("Index") });
            }

            return Json(new { message = "Username is already taken" });
        }
         public IActionResult Register()
        {
            ViewData["Title"] = "Rejestracja";
            return View();
        }

        public IActionResult Login()
        {
            ViewData["Title"] = "Login";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}