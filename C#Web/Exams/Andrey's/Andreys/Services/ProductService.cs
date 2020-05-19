using System.Collections.Generic;
using System.Linq;
using Andreys.Data;
using Andreys.Models;
using Andreys.ViewModels.Products;

namespace Andreys.Services
{
    public class ProductService : IProductService
    {
        private readonly AndreysDbContext db;

        public ProductService(AndreysDbContext db)
        {
            this.db = db;
        }

        public int Add(string name, string description, string iumageUrl, decimal price, Category category, Gender gender)
        {
            var product = new Product
            {
                Name = name,
                Description = description,
                ImageUrl = iumageUrl,
                Price = price,
                Category = category,
                Gender = gender
            };

            this.db.Products.Add(product);
            this.db.SaveChanges();

            return product.Id;
        }

        public IEnumerable<Product> All()
        {
           var products =  this.db.Products.ToList();

            return products;
        }
    }
}
