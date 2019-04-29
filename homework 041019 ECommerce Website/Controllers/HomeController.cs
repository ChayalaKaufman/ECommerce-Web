using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using homework_041019_ECommerce_Website.Models;
using ECommerce.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace homework_041019_ECommerce_Website.Controllers
{
    public class HomeController : Controller
    {
        private IHostingEnvironment _environment;
        private string _connectionString;

        public HomeController(IHostingEnvironment environment,
            IConfiguration configuration)
        {
            _environment = environment;
            _connectionString = configuration.GetConnectionString("ConStr");
        }
        public IActionResult Index(int id)
        {
            ClientDb clientDb = new ClientDb(_connectionString);

            HomePageViewModel vm = new HomePageViewModel();
            if (id != 0)
            {
                vm.Products = clientDb.GetProductsForCategory(id);
            }
            else
            {
                vm.Products = clientDb.GetProductsForCategory(0);
            }

            vm.Categories = clientDb.GetCategories();
            return View(vm);
        }

        public IActionResult ViewProduct(int id)
        {
            ClientDb db = new ClientDb(_connectionString);
            ProductViewModel vm = new ProductViewModel();
            vm.Product = db.GetProduct(id);
            vm.Categories = db.GetCategories();
            return View(vm);
        }
        
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            ClientDb db = new ClientDb(_connectionString);
            int? cart = HttpContext.Session.GetInt32("Cart");
            if (cart == null)
            {
                cart = db.CreateCart();
                HttpContext.Session.SetInt32("Cart", cart.Value);
            }
            db.AddToCart(productId, cart.Value, quantity);
            Product p = db.GetProduct(productId);
            return Json(p);
        }

        [Authorize]
        public IActionResult ViewCart()
        {
            CartViewModel vm = new CartViewModel();
            ClientDb db = new ClientDb(_connectionString);
            int? cart = HttpContext.Session.GetInt32("Cart");
            if (cart == null)
            {
                vm.Message = "Your cart is empty";
                return View(vm);
            }
            vm.Customer = db.GetByEmail(User.Identity.Name);
            vm.CartProducts = db.GetCartProducts(cart.Value);
            return View(vm);
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            int cartId = HttpContext.Session.GetInt32("Cart").Value;
            ClientDb db = new ClientDb(_connectionString);
            db.RemoveFromCart(cartId, productId);
            return Json(productId);
        }

        [HttpPost]
        public IActionResult EditCart(int quantity, int productId)
        {
            int cartId = HttpContext.Session.GetInt32("Cart").Value;
            ClientDb db = new ClientDb(_connectionString);
            db.EditCart(cartId, productId, quantity);
            return Json(quantity);
        }

        public IActionResult GetCartAjax()
        {
            int cartId = HttpContext.Session.GetInt32("Cart").Value;
            ClientDb db = new ClientDb(_connectionString);
            List<CartProduct> cartProducts = db.GetCartProducts(cartId);
            return Json(cartProducts);
        }
    }

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonConvert.DeserializeObject<T>(value);
        }
    }
}
