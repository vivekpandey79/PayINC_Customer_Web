using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PayInc_Customer_web.Areas.Reports.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Areas.Reports.Controllers
{
    [Area("Reports")]
    public class AEPSTransactionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(AEPSTransactionModel input)
        {
            try
            {
                var listParam = new List<KeyValuePair<string, string>>();
                listParam.Add(new KeyValuePair<string, string>("customerId", Convert.ToString(new SessionUtility().GetLoginSession().customerId)));
                listParam.Add(new KeyValuePair<string, string>("transactionType", input.TransactionType));
                listParam.Add(new KeyValuePair<string, string>("senderMobileNumber", input.SenderMobileNumber));
                listParam.Add(new KeyValuePair<string, string>("aadharNumber", input.AadharNumber));
                listParam.Add(new KeyValuePair<string, string>("fromDate", Convert.ToDateTime(input.FromDate).ToString("yyyyMMdd")));
                listParam.Add(new KeyValuePair<string, string>("toDate", Convert.ToDateTime(input.ToDate).ToString("yyyyMMdd")));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<List<AEPSTransactionRes>>("getAepsTransactionDetails", listParam, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    ViewBag.AEPSTransactionReport = response;
                }
                else
                {
                    ViewData["ErrorMessage"] = errorMessage;
                }
            }
            catch (Exception)
            {

            }
            return View();
        }
    }
}
