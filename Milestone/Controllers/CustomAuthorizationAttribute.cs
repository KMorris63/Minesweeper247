using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Mvc;

namespace Milestone.Controllers
{
    internal class CustomAuthorizationAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string userID = context.HttpContext.Session.GetString("userID");

            // if it hasn't been set or is -1
            if (userID == null || userID == "-1")
            {
                // session variable is not set, deny access
                context.Result = new RedirectResult("/login");
            }
        }
    }
}