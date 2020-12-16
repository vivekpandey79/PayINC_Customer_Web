using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PayInc_Customer_web.Areas.OnBoarding.Models;
using PayInc_Customer_web.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Areas.Networks.Controllers
{
    [Area("Networks")]
    [Authentication]
    public class ViewNetworksController : Controller
    {
        public IActionResult Index()
        {
            try
            {
                var listParam = new List<KeyValuePair<string, string>>();
                listParam.Add(new KeyValuePair<string, string>("cutomerId", Convert.ToString(new  SessionUtility().GetLoginSession().customerId)));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<List<LowChainResponse>>("getViewCustomerNetwork", listParam, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return View(response);
                }
                return View();
            }
            catch (Exception)
            {
                return View();
            }
        }
        [HttpGet]
        public IActionResult ViewNetwork(int customerId)
        {
            try
            {
                var listParam = new List<KeyValuePair<string, string>>();
                listParam.Add(new KeyValuePair<string, string>("cutomerId", Convert.ToString(customerId)));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<List<LowChainResponse>>("getViewCustomerNetwork", listParam, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return PartialView("LowerNetwork",response);
                }
                return PartialView("LowerNetwork");
            }
            catch (Exception)
            {
                return PartialView("LowerNetwork");
            }
        }
    }
}
