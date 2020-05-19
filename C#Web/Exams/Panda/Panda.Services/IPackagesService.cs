using Panda.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace Panda.Services
{
    public interface IPackagesService
    {
        void Create(string description, decimal weight, string address, string username);

        IQueryable<Package> AllPaclageByStatus(PackageStatus statusCode);
        void Deliver(string id);
    }
}
