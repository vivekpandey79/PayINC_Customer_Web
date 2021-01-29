using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PayInc_Customer_web.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Controllers
{
    [AuthenticationAttribute]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            GetDashboard();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public void GetDashboard()
        {
            try
            {
                var listParam = new List<KeyValuePair<string, string>>();
                listParam.Add(new KeyValuePair<string, string>("test","test"));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<List<DashboardRes>>("getDashboardImages", listParam, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    //int i = 0;
                    //foreach (var item in response)
                    //{
                    //    if (i==0)
                    //    {
                    //        item.imageUrl = "/assets/media/dashboard-images/1.png";
                    //    }
                    //    else if(i==1)
                    //    {
                    //        item.imageUrl = "/assets/media/dashboard-images/2.png";
                    //    }
                    //    else
                    //    {
                    //        item.imageUrl = "/assets/media/dashboard-images/3.png";
                    //    }
                    //    i = i + 1;
                    //}
                    ViewBag.DashboardImage = response;
                }
                else
                {

                }
            }
            catch (Exception)
            {

            }
           
        }

        [HttpPost]
        public IActionResult GetCustomerUsage()
        {
            try
            {
                var listParams = new List<KeyValuePair<string, string>>();
                listParams.Add(new KeyValuePair<string, string>("customerId", Convert.ToString(new SessionUtility().GetLoginSession().customerId)));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<CustomerUsageRes>("getCustomerUsageDashBoard", listParams,ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return Json(response);
                }
                else
                {
                    return Json("");
                }
            }
            catch (Exception)
            {
                return Json("");
            }
        }

        [HttpPost]
        public IActionResult GetNetworkUsage()
        {
            try
            {
                var listParams = new List<KeyValuePair<string, string>>();
                listParams.Add(new KeyValuePair<string, string>("customerId", Convert.ToString(new SessionUtility().GetLoginSession().customerId)));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<NetworkUsage>("getCustomerNetworkUsageDashBoard", listParams, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return Json(response);
                }
                else
                {
                    return Json("");
                }
            }
            catch (Exception)
            {
                return Json("");
            }
        }


        [HttpPost]
        public IActionResult GetTopTransaction()
        {
            try
            {
                var listParams = new List<KeyValuePair<string, string>>();
                listParams.Add(new KeyValuePair<string, string>("customerId", Convert.ToString(new SessionUtility().GetLoginSession().customerId)));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<List<Top10TrnxRes>>("getTop10RechargeTransactionByCustomer", listParams, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return Json(response);
                }
                else
                {
                    return Json("");
                }
            }
            catch (Exception)
            {
                return Json("");
            }
        }

        [HttpPost]
        public IActionResult GetTopLedger()
        {
            try
            {
                var listParams = new List<KeyValuePair<string, string>>();
                listParams.Add(new KeyValuePair<string, string>("customerId", Convert.ToString(new SessionUtility().GetLoginSession().customerId)));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<List<Top10LedgerResp>>("getTop10AccountLedgerByCustomer", listParams, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return Json(response);
                }
                else
                {
                    return Json("");
                }
            }
            catch (Exception)
            {
                return Json("");
            }
        }

        [HttpPost]
        public IActionResult GetDayWiseUsage()
        {
            try
            {
                var listParams = new List<KeyValuePair<string, string>>();
                listParams.Add(new KeyValuePair<string, string>("cutomerId", Convert.ToString(new SessionUtility().GetLoginSession().customerId)));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<List<DayWiseUsage>>("getDaywiseUsageMonthly", listParams, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    if (response!=null)
                    {
                        foreach (var item in response)
                        {
                            item.Date = Convert.ToDateTime(item.Date).ToString("dd-MMM");
                        }
                        response = response.Where(m => m.walletType == "Main Wallet").ToList();
                    }
                    
                    return Json(response);
                }
                else
                {
                    return Json("");
                }
            }
            catch (Exception)
            {
                return Json("");
            }
        }
    }
}
