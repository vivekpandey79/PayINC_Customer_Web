using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayInc_Customer_web.Areas.Recharge.Models;
using PayInc_Customer_web.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Areas.BillPayments.Controllers
{
    [Area("BillPayments")]
    [Authentication]
    public class PostpaidController : Controller
    {
        public IActionResult Index()
        {
            BindOperator();
            return View();
        }
        public void BindOperator()
        {
            var listParam = new List<KeyValuePair<string, string>>();
            listParam.Add(new KeyValuePair<string, string>("ServiceProviderTypeId", "2"));
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
        [ValidateAntiForgeryToken]
        public IActionResult DoTransaction(IFormCollection fc)
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
                    remarks = "Postpaid",
                    tPin = new PasswordHash().HashShA1(tpin)

                };
                string errorMessage = string.Empty;
                var response = new CallService().PostTransaction<TransactionResult>("doRecharge", req, ref errorMessage);
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
