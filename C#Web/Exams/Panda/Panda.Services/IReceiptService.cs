using Panda.Data.Models;
using System.Linq;

namespace Panda.Services
{
    public interface IReceiptService
    {
        void CreateFromPackage(decimal weight, string packageId, string userId);

        IQueryable<Receipt> GetAll();
    }
}
