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
    public class StockReqStatusController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(StockRequestModel input)
        {
            try
            {
                var listParam = new List<KeyValuePair<string, string>>();
                listParam.Add(new KeyValuePair<string, string>("customerId", Convert.ToString(new SessionUtility().GetLoginSession().customerId)));
                listParam.Add(new KeyValuePair<string, string>("fromDate", Convert.ToString(input.FromDate.ToString("yyyyMMdd"))));
                listParam.Add(new KeyValuePair<string, string>("toDate", Convert.ToString(input.ToDate.ToString("yyyyMMdd"))));
                
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<List<PaymentReportRes>>("getStockRequestStatus", listParam, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    ViewBag.StockReqReport = response;
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
