using AutoMapper;
using ProductShop.Dtos;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System.Linq;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<ImportUsersDTO, User>();

            this.CreateMap<ImportProductsDTO, Product>();

            this.CreateMap<ImportCategoriesDTO, Category>();

            this.CreateMap<ImportCategories_ProductsDTO, CategoryProduct>();


            this.CreateMap<Product, ExportProductsInRangeDTO>()
                .ForMember(s => s.BuyerName, y => y.MapFrom(x => x.Buyer.FirstName + " " + x.Buyer.LastName));

            this.CreateMap<Product, ExportProductSoldProductsDTO>();
            this.CreateMap<User, ExportUserSoldProductsDTO>()
                .ForMember(s => s.SoldProducts, y => y.MapFrom(s => s.ProductsSold.Select(e => new { e.Name, e.Price })));


           // this.CreateMap<User, ExportUsersWithProductDto>();
            this.CreateMap<User, ExportUserDto>();
            this.CreateMap<Product, ExportProductsDto>();
            this.CreateMap<Product, ExportSoldProductsDto>();

            this.CreateMap<Category, ExportCategoriesByProductsCountDTO>()
                 .ForMember(x => x.Count, y => y.MapFrom(obj => obj.CategoryProducts.Count))
                 .ForMember(x => x.TotalRevenue, y => y.MapFrom(obj => obj.CategoryProducts.Sum(z => z.Product.Price)))
                 .ForMember(x => x.AveragePrice, y => y.MapFrom(obj => obj.CategoryProducts.Average(z => z.Product.Price)));
        }
    }
}
