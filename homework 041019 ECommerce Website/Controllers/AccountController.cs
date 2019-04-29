using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace homework_041019_ECommerce_Website.Controllers
{
    public class AccountController : Controller
    {
        private IHostingEnvironment _environment;
        private string _connectionString;

        public AccountController(IHostingEnvironment environment,
            IConfiguration configuration)
        {
            _environment = environment;
            _connectionString = configuration.GetConnectionString("ConStr");
        }
        
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(Customer c, string password)
        {
            ClientDb db = new ClientDb(_connectionString);
            db.AddCustomer(c, password);
            return Redirect("/home/index");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var db = new ClientDb(_connectionString);
            var user = db.Login(email, password);
            if (user == null)
            {
                
                TempData["message"] = "Invalid login attempt";
                return Redirect("/account/login");
            }

            //this code is conceptually the same as FormsAuthentication.SetAuthCookie()
            var claims = new List<Claim>
                {
                    new Claim("user", email)
                };
            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();

            return Redirect("/home/index");
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/");
        }
    }
}