using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using demo.EF;
using demo.DTOs;

namespace demo.Auth
{
    public class Logged : AuthorizeAttribute
    {
        public string Role1 { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var user = (LoginDTO)httpContext.Session["user"];
            if (user != null)
            {
                // Check if a specific role is required and if the user's role matches
                if (!string.IsNullOrEmpty(Role1) && !Role1.Contains(user.Role1))
                {
                    return false; // Role doesn't match
                }
                return true; // User is logged in and has the required role
            }
            return false; // User is not logged in
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!AuthorizeCore(filterContext.HttpContext))
            {
                var user = (LoginDTO)filterContext.HttpContext.Session["user"];
                if (user != null && !string.IsNullOrEmpty(Role1) && !Role1.Contains(user.Role1))
                {
                    // User is logged in but does not have the required role
                    filterContext.Controller.TempData["Msg"] = "You do not have permission to perform this action.";
                    filterContext.Result = new RedirectToRouteResult(
                        new System.Web.Routing.RouteValueDictionary(new { controller = "Channel", action = "List" }));
                }
                else
                {
                    // User is not logged in
                    filterContext.Controller.TempData["Msg"] = "You must be logged in to access this page.";
                    filterContext.Result = new RedirectResult("~/Login/Index");
                }
            }
        }
    }
}