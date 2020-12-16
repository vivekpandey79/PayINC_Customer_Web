using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PayInc_Customer_web.Areas.Recharge.Models;
using PayInc_Customer_web.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Areas.Recharge.Controllers
{
    [Area("Recharge")]
    [Authentication]
    public class PrepaidController : Controller
    {
        public IActionResult Index()
        {
            BindOperator();
            return View();
        }

        public void BindOperator()
        {
            var listParam = new List<KeyValuePair<string, string>>();
            listParam.Add(new KeyValuePair<string, string>("ServiceProviderTypeId", "1"));
            string errorMessage = string.Empty;
            var response = new CallService().GetResponse<List<OperatorResponse>>(APIMethodConst.GetServiceProvidersByTypeId, listParam,ref errorMessage);
            if (string.IsNullOrEmpty(errorMessage))
            {
                ViewBag.Operator = response;
            }
            else
            {
                ViewBag.Operator = null;
            }
        }

        [HttpPost]
        public IActionResult GetMobilePlans(string mobileNumber, string operatorName)
        {
            try
            {
                string strOperatorName = string.Empty;
                switch (operatorName.Trim())
                {
                    case "JIO":
                    strOperatorName = "Jio";
                        break;
                    case "AIRTEL":
                        strOperatorName = "Airtel";
                        break;
                    default:
                        break;
                }
                var listParams = new List<KeyValuePair<string, string>>();
                listParams.Add(new KeyValuePair<string, string>("offer", "roffer"));
                listParams.Add(new KeyValuePair<string, string>("mobileNumber", mobileNumber));
                listParams.Add(new KeyValuePair<string, string>("operators", strOperatorName));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<MPlansResponse>("getMobileROfferPlan", listParams, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return PartialView("ViewPlans", response);
                }
                else
                {
                    return PartialView("ViewPlans");
                }
            }
            catch (Exception ex)
            {
                return PartialView("ViewPlans");
            }
        }
    }
}
