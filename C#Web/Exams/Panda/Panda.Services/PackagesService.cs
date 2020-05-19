using Panda.Data;
using Panda.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Panda.Services
{
    public class PackagesService : IPackagesService
    {
        private readonly PandaDbContex db;
        private readonly IReceiptService receiptService;

        public PackagesService(PandaDbContex db, IReceiptService receiptService)
        {
            this.db = db;
            this.receiptService = receiptService;
        }

    

        public void Create(string description, decimal weight, string address, string username)
        {
            string recipient = this.db.Users.Where(x => x.Username == username).Select(s=>s.Id).FirstOrDefault();

            if (recipient == null)
            {
                return;
            }

            var package = new Package()
            {
                Description = description,
                Weight = weight,
                ShippingAddress = address,
                Status = PackageStatus.Pending,
                RecipientId = recipient
            };

            this.db.Packages.Add(package);
            this.db.SaveChanges();
        }

        public IQueryable<Package> AllPaclageByStatus(PackageStatus statusCode)
        {
            var packages = this.db.Packages.Where(s => s.Status == statusCode);
            return packages;
        }

        public void Deliver(string id)
        {
            var packages = this.db.Packages.FirstOrDefault(s => s.Id == id);
            if (packages == null)
            {
                return;
            }
         
            packages.Status = PackageStatus.Delivered;
            this.db.SaveChanges();

            this.receiptService.CreateFromPackage(packages.Weight, packages.Id, packages.RecipientId);
        }
    }
}
