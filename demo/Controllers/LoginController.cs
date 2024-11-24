using demo.DTOs;
using demo.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace demo.Controllers
{
    public class LoginController : Controller
    {
        private demoEntities db = new demoEntities();

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginDTO log)
        {
            if (!ModelState.IsValid) // Check if the model is valid (i.e., all required fields are filled)
            {
                return View(log); // Return the view with validation messages
            }

            // Check if the user exists and validate login credentials
            var user = (from u in db.Roles
                        where u.UserName.Equals(log.UserName) &&
                              u.Password.Equals(log.Password)
                        select u).SingleOrDefault();

            if (user != null)
            {
                var userDto = new LoginDTO
                {
                    RoleId = user.RoleId,
                    UserName = user.UserName,
                    Password = user.Password,
                    Role1 = user.Role1
                };
                // Store user info in the session
                Session["user"] = userDto;

                // Redirect based on role
                /*  if (user.Role1 == "User")
                  {
                      return RedirectToAction("List", "Channel");  // User can only view the list
                  }
                  else if (user.Role1 == "Editor")
                  {
                      return RedirectToAction("List", "Channel");  // Editor can also view/edit but not delete
                  }
                  else if (user.Role1 == "Admin")
                  {
                      return RedirectToAction("List", "Channel");  // Admin can manage everything
                  }*/
                return RedirectToAction("List", "Channel");
            }
            TempData["Msg"] = "User not found";
            return View();
        }

    }
}
