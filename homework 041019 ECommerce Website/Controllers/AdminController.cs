using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ECommerce.Data;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace homework_041019_ECommerce_Website.Controllers
{
    public class AdminController : Controller
    {
        private IHostingEnvironment _environment;
        private string _connectionString;

        public AdminController(IHostingEnvironment environment,
            IConfiguration configuration)
        {
            _environment = environment;
            _connectionString = configuration.GetConnectionString("ConStr");
        }
        [Authorize]
        public IActionResult Index()
        {
            Db db = new Db(_connectionString);
            var categories = db.GetCategories();
            return View(categories);
        }

        [HttpPost]
        public IActionResult AddCategory(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                Db db = new Db(_connectionString);
                db.AddCategory(name);
            }
            return Redirect("/admin/index");
        }

        [HttpPost]
        public IActionResult AddProduct(Product product, IFormFile image)
        { 
            if(product.CategoryId != 0 && product.Name != null && product.Description != null && product.Price != 0)
            {
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                string fullPath = Path.Combine(_environment.WebRootPath, "images/uploads", fileName);
                using (FileStream stream = new FileStream(fullPath, FileMode.CreateNew))
                {
                    image.CopyTo(stream);
                }
                product.Image = fileName;

                Db db = new Db(_connectionString);

                db.AddProduct(product);
            }
            return Redirect("/admin/index");
        }

        
    }
}