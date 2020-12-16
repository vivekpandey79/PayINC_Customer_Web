using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PayInc_Customer_web.Areas.Recharge.Models;
using PayInc_Customer_web.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Areas.Recharge.Controllers
{
    [Area("Recharge")]
    [Authentication]
    public class DTHController : Controller
    {
        public IActionResult Index()
        {
            BindOperator();
            return View();
        }
        public void BindOperator()
        {
            var listParam = new List<KeyValuePair<string, string>>();
            listParam.Add(new KeyValuePair<string, string>("ServiceProviderTypeId", "3"));
            string errorMessage = string.Empty;
            var response = new CallService().GetResponse<List<OperatorResponse>>(APIMethodConst.GetServiceProvidersByTypeId, listParam, ref errorMessage);
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
        public IActionResult GetDTHPlans(string accountName, string operatorName)
        {
            try
            {
                string strOperatorName = string.Empty;
                switch (operatorName.Trim())
                {
                    case "TATA SKY":
                        strOperatorName = "TataSky";
                        break;
                    case "AIRTEL":
                        strOperatorName = "Airtel";
                        break;
                    default:
                        break;
                }
                var listParams = new List<KeyValuePair<string, string>>();
                listParams.Add(new KeyValuePair<string, string>("offer", "roffer"));
                listParams.Add(new KeyValuePair<string, string>("dthNumber", accountName));
                listParams.Add(new KeyValuePair<string, string>("operators", strOperatorName));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<string>("getDthCustomerInfoPlan", listParams, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    var jsonObject = JsonConvert.DeserializeObject<DTHPlanResp>(response);
                    return PartialView("ViewPlan", jsonObject);
                }
                else
                {
                    return PartialView("ViewPlan");
                }
            }
            catch (Exception ex)
            {
                return PartialView("ViewPlan");
            }
        }
    }
}
