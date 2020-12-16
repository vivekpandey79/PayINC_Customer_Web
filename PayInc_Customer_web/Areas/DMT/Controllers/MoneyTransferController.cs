using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PayInc_Customer_web.Areas.DMT.Models;
using PayInc_Customer_web.Models;

namespace PayInc_Customer_web.Areas.DMT.Controllers
{
    [Area("DMT")]
    [Authentication]
    public class MoneyTransferController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckMobileNumber(MoneyTransferModel input)
        {
            if (input.MobileNumber=="8898862556")
            {
                return View("BeneficiaryList");
            }
            else
            {
                return View("RegisterUser");
            }
        }

        public IActionResult TransactionConfirm()
        {
            return View();
        }
    }
}
