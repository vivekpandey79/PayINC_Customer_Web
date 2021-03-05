using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayInc_Customer_web.Areas.Reports.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Areas.Reports.Controllers
{
    [Area("Reports")]
    public class OutgoingCommissionReportController : Controller
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
                var loginData = new SessionUtility().GetLoginSession();
                var listParams = new List<KeyValuePair<string, string>>();
                listParams.Add(new KeyValuePair<string, string>("customerId", Convert.ToString(loginData.customerId)));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<List<OutgoingCommissionRes>>("getOutgoingCommissionsByCustId", listParams, ref errorMessage);
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
