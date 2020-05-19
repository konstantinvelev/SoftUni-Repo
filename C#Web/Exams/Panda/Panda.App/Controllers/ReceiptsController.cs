using Panda.App.ViewModels.Receipts;
using Panda.Services;
using SIS.MvcFramework;
using SIS.MvcFramework.Result;
using System.Linq;

namespace Panda.App.Controllers
{
    public class ReceiptsController : Controller
    {
        private readonly IReceiptService receiptService;

        public ReceiptsController(IReceiptService receiptService)
        {
            this.receiptService = receiptService;
        }
        public IActionResult Index()
        {
            var viewModel = this.receiptService.GetAll()
                 .Select(s => new ReceiptViewModel
                 {
                     Id = s.Id,
                     Fee = s.Fee,
                     IssuedOn = s.IssuedOn,
                     RecipientName = s.Recipient.Username
                 }).ToList();

            return this.View(viewModel);
        }

    }
}
