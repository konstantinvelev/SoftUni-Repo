using Andreys.Models;
using Andreys.ViewModels.Products;
using System.Collections;
using System.Collections.Generic;

namespace Andreys.Services
{
    public interface IProductService
    {
        int Add(string name, string description, string iumageUrl, decimal price, Category category, Gender gender);

        IEnumerable<Product> All();
    }
}
