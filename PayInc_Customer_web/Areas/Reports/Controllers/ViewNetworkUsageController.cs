using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayInc_Customer_web.Areas.Reports.Models;
using PayInc_Customer_web.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Areas.Reports.Controllers
{
    [Area("Reports")]
    [Authentication]
    public class ViewNetworkUsageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Report(IFormCollection fc)
        {
            try
            {
                var fromDate = string.IsNullOrEmpty(Convert.ToString(fc["fromDate"])) ? DateTime.Now.ToString("yyyyMMdd") : Convert.ToDateTime(fc["fromDate"]).ToString("yyyyMMdd");
                var toDate = string.IsNullOrEmpty(Convert.ToString(fc["toDate"])) ? DateTime.Now.ToString("yyyyMMdd") : Convert.ToDateTime(fc["toDate"]).ToString("yyyyMMdd");
                var listParams = new List<KeyValuePair<string, string>>();
                listParams.Add(new KeyValuePair<string, string>("fromDate", Convert.ToString(Convert.ToInt32(fromDate))));
                listParams.Add(new KeyValuePair<string, string>("toDate", Convert.ToString(Convert.ToInt32(toDate))));
                listParams.Add(new KeyValuePair<string, string>("customerId", Convert.ToString(new SessionUtility().GetLoginSession().customerId)));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<List<GetNetworkUsage>>("getViewNetworkUsage", listParams, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return PartialView("ViewReport", response);
                }
            }
            catch (Exception)
            {

            }
            return PartialView("ViewReport");

        }
    }
}
