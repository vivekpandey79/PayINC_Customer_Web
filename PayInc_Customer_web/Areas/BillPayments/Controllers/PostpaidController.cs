using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}
