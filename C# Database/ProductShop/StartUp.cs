namespace ProductShop
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using ProductShop.Dtos;
    using ProductShop.Dtos.Export;
    using ProductShop.Dtos.Import;
    using ProductShop.Models;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg => cfg.AddProfile<ProductShopProfile>());

            using (var db = new ProductShopContext())
            {
                //string inputXml = File.ReadAllText("./../../../categories-products.xml");

                var result = GetUsersWithProducts(db);
                Console.WriteLine(result);
            }
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportUsersDTO[]), new XmlRootAttribute("Users"));

            ImportUsersDTO[] importUsersDTO;

            using (var reader = new StringReader(inputXml))
            {
                importUsersDTO = (ImportUsersDTO[])xmlSerializer.Deserialize(reader);
            }

            var users = Mapper.Map<User[]>(importUsersDTO);

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Length}";
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var xmlSeriallizer = new XmlSerializer(typeof(ImportProductsDTO[]), new XmlRootAttribute("Products"));

            ImportProductsDTO[] importProductsDTOs;

            using (var reader = new StringReader(inputXml))
            {
                importProductsDTOs = (ImportProductsDTO[])xmlSeriallizer.Deserialize(reader);
            }

            var products = Mapper.Map<Product[]>(importProductsDTOs);

            context.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Length}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var xmlSerializar = new XmlSerializer(typeof(ImportCategoriesDTO[]), new XmlRootAttribute("Categories"));

            ImportCategoriesDTO[] importCategoriesDTOs;

            using (var reader = new StringReader(inputXml))
            {
                importCategoriesDTOs = (ImportCategoriesDTO[])xmlSerializar.Deserialize(reader);
            }

            var categories = Mapper.Map<Category[]>(importCategoriesDTOs);

            var end = new List<Category>();

            foreach (var category in categories)
            {
                if (string.IsNullOrEmpty(category.Name))
                {
                    continue;
                }
                end.Add(category);
            }

            context.AddRange(end);
            context.SaveChanges();

            return $"Successfully imported {end.Count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var xmlSerializar = new XmlSerializer(typeof(ImportCategories_ProductsDTO[]), new XmlRootAttribute("CategoryProducts"));

            ImportCategories_ProductsDTO[] importCategories_ProductsDTOs;

            using (var reader = new StringReader(inputXml))
            {
                importCategories_ProductsDTOs = (ImportCategories_ProductsDTO[])xmlSerializar.Deserialize(reader);
            }

            var cp = Mapper.Map<CategoryProduct[]>(importCategories_ProductsDTOs);

            int i = 0;

            foreach (var item in cp)
            {
                var category = context.Categories.First(s => s.Id == item.CategoryId);
                var product = context.Products.First(s => s.Id == item.ProductId);

                if (category == null || product == null)
                {
                    continue;
                }

                var entity = new CategoryProduct
                {
                    Category = category,
                    Product = product
                };

                context.CategoryProducts.Add(entity);
                i++;
            }

            context.SaveChanges();

            return $"Successfully imported {i}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var sb = new StringBuilder();

            var products = context.Products
                .Where(s => s.Price >= 500 && s.Price <= 1000)
                .OrderBy(o => o.Price)
                .Take(10)
                .ProjectTo<ExportProductsInRangeDTO>()
                .ToArray();

            var xmlSerializar = new XmlSerializer(typeof(ExportProductsInRangeDTO[]), new XmlRootAttribute("Products"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using (var writer = new StringWriter(sb))
            {
                xmlSerializar.Serialize(writer, products, namespaces);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var sb = new StringBuilder();

            var mathces = context.Users
                .Where(s => s.ProductsSold.Count >=1)
                .OrderBy(o => o.LastName)
                .ThenBy(i => i.FirstName)
                .Take(5)
                .ProjectTo<ExportUserSoldProductsDTO>()
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportUserSoldProductsDTO[]), new XmlRootAttribute("Users"));

            var namesps = new XmlSerializerNamespaces();
            namesps.Add(string.Empty, string.Empty);

            using (var writer = new StringWriter(sb))
            {
                xmlSerializer.Serialize(writer, mathces, namesps);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var sb = new StringBuilder();

            var categories = context.Categories
                  .ProjectTo<ExportCategoriesByProductsCountDTO>()
                  .OrderByDescending(c => c.Count)
                  .ThenBy(c => c.TotalRevenue)
                  .ToArray();


            var xml = new XmlSerializer(typeof(ExportCategoriesByProductsCountDTO[]), new XmlRootAttribute("Categories"));

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using (var writer = new StringWriter(sb))
            {
                xml.Serialize(writer, categories, namespaces);
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var xml = new XmlSerializer(typeof(ExportUserDto[]), new XmlRootAttribute("Users"));
            var sb = new StringBuilder();

            var users = context.Users
                .Include(x => x.ProductsSold)
                .Where(u => u.ProductsSold.Count > 0)
                .OrderByDescending(u => u.ProductsSold.Count)
                .ProjectTo<ExportUserDto>()
                .ToList();


            var np = new XmlSerializerNamespaces();
            np.Add(string.Empty, string.Empty);
            using (var writer = new StringWriter(sb))
            {
                xml.Serialize(writer, users, np);
            }
            return sb.ToString().TrimEnd();
        }
    }
}