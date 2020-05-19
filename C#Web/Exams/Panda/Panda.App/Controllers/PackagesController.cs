
using Panda.App.ViewModels.Package;
using Panda.Data.Models;
using Panda.Services;
using SIS.MvcFramework;
using SIS.MvcFramework.Attributes;
using SIS.MvcFramework.Result;
using System.Linq;

namespace Panda.App.Controllers
{
    public class PackagesController : Controller
    {
        private readonly IPackagesService packagesService;
        private readonly IUsersService usersService;

        public PackagesController(IPackagesService packagesService, IUsersService usersService)
        {
            this.packagesService = packagesService;
            this.usersService = usersService;
        }

        public IActionResult Create()
        {
            var list = this.usersService.GetUsernames();
            return this.View(list);
        }

        [HttpPost]
        public IActionResult Create(CreatePackageInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Redirect("/Packages/Create");
            }

            this.packagesService.Create(input.Description, input.Weight, input.ShippingAddress, input.RecipientName);

            return this.Redirect("/Packages/Pending");
        }

        public IActionResult Delivered()
        {
            var packages = this.packagesService.AllPaclageByStatus(PackageStatus.Delivered)
                .Select(s => new PackageViewModel
                {
                    Id = s.Id,
                    Description = s.Description,
                    Weight = s.Weight,
                    ShippingAddress = s.ShippingAddress,
                    RecipientName = s.Recipient.Username,
                })
                .ToList();
            return this.View(new PackagesListViewModel { Packages = packages });
        }

        public IActionResult Pending()
        {
            var packages = this.packagesService.AllPaclageByStatus(PackageStatus.Pending)
                .Select(s => new PackageViewModel
                {
                    Id = s.Id,
                    Description = s.Description,
                    Weight = s.Weight,
                    ShippingAddress = s.ShippingAddress,
                    RecipientName = s.Recipient.Username,
                })
                .ToList();
            return this.View(new PackagesListViewModel {Packages = packages });
        }

        public IActionResult Deliver(string id)
        {
            this.packagesService.Deliver(id);
            return this.Redirect("/Packages/Delivered");
        }
    }
}
