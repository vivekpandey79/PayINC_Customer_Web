using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Areas.Reports.Controllers
{
    [Area("Reports")]
    public class AEPSTransactionController : Controller
    {
        public IActionResult Index()
        {
            try
            {
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<string>("getAepsTransactionDetails",null,ref errorMessage);
            }
            catch (Exception)
            {

            }
            return View();
        }
    }
}
