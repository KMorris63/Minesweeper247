using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Milestone.Models;
using Milestone.Views.Services;
using Nancy.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*
 * Alex Vergara and Kacey Morris
 * January 31, 2021
 * CST 247
 * Minesweeper Web Application
 * 
 * Login Controller which directs the actions of the login module. 
 * 
 * This is our own work as influenced by class time and examples. 
 */

namespace Milestone.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ProcessLogin(UserModel user)
        {
            SecurityService security = new SecurityService();
            // if -1 was returned, that means the user was not found
            // otherwise, the value should be the userID
            int userID = security.loginUser(user);
            if (userID != -1)
            {
                // set the session variables
                HttpContext.Session.SetInt32("userID", userID);
                return View("LoginSuccessful", user);
            }
            else
            {
                return View("LoginFailure", user);
            }
        }

        public IActionResult SelectDifficulty()
        {
            return View("LoginSuccessful");
        }
    }
}
