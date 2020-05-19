using Panda.Data;
using Panda.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panda.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly PandaDbContex db;

        public ReceiptService(PandaDbContex db)
        {
            this.db = db;
        }
        public void CreateFromPackage(decimal weight, string packageId, string userId)
        {
            var receipt = new Receipt
            {
                PackageId = packageId,
                RecipientId = userId,
                Fee = weight * 2.67M,
                IssuedOn = DateTime.UtcNow,
            };
            this.db.Receipts.Add(receipt);
            this.db.SaveChanges();  
        }

        public IQueryable<Receipt> GetAll()
        {
            return this.db.Receipts;
        }
    }
}
