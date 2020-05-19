using Andreys.Models;
using Andreys.Services;
using Andreys.ViewModels.Products;
using SIS.HTTP;
using SIS.MvcFramework;
using System;

namespace Andreys.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService productService;

        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpGet("/Products/Add")]
        public HttpResponse Add()
        {
            if (!IsUserLoggedIn())
            {
                return this.Redirect("/User/Login");
            }
            return this.View("/Add");
        }

        [HttpPost("/Products/Add")]
        public HttpResponse Add(string name, string description, string iumageUrl, decimal price, string category, string gender)
        {
            var currCategory = Enum.Parse<Category>(category);
            var currGender = Enum.Parse<Gender>(gender);

            if (!IsUserLoggedIn())
            {
                return this.Redirect("/User/Login");
            }

            if (category == null || gender == null)
            {
                return this.Redirect("/Products/Add");
            }
            if (name.Length < 4 || name.Length > 20)
            {
                return this.Redirect("/Products/Add");
            }
            if (description.Length < 10)
            {
                return this.Redirect("/Products/Add");
            }
            if (price >= 0)
            {
                return this.Redirect("/Products/Add");
            }

           var productId  = this.productService.Add(name, description, iumageUrl, price, currCategory, currGender);
            return this.View("/Home"); 
        }
    }
}
