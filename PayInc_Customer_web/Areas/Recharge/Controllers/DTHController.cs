using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
                    ViewData["OperatorName"] = operatorName;
                    ViewData["AccountNo"] = accountName;
                    return PartialView("ViewPlan");
                }
            }
            catch (Exception)
            {
                ViewData["OperatorName"] = operatorName;
                ViewData["AccountNo"] = accountName;
                return PartialView("ViewPlan");
            }
        }

        [HttpPost]
        public IActionResult DoDTHTransaction(IFormCollection fc)
        {
            try
            {
                var tpin = Convert.ToString(fc["digit1"]) + Convert.ToString(fc["digit2"]) + Convert.ToString(fc["digit3"]) + Convert.ToString(fc["digit4"]) + Convert.ToString(fc["digit5"]) + Convert.ToString(fc["digit6"]) + Convert.ToString(fc["digit7"]) + Convert.ToString(fc["digit8"]);
                var sessionUtility = new SessionUtility();
                var req = new
                {
                    customerId = sessionUtility.GetLoginSession().customerId,
                    rechageNumber = Convert.ToString(fc["MobileNumber"]),
                    txnAmount = Convert.ToDecimal(fc["Amount"]),
                    serviceProviderId = Convert.ToInt32(fc["OperatorId"]),
                    serviceCircleId = 1,
                    serviceChannelId = 2,
                    remarks = "DTH",
                    tPin = new PasswordHash().HashShA1(tpin)

                };
                string errorMessage = string.Empty;
                var response = new CallService().PostTransaction<TransactionResult>("doRecharge", req, ref errorMessage);
                if (response != null)
                {
                    if (response.response != null)
                    {
                        response.response.OperatorNm = Convert.ToString(fc["OperatorNm"]);
                    }
                }
                return PartialView("AckView", response);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return PartialView("AckView");
        }
    }
}
