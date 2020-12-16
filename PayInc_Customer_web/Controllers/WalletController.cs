using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PayInc_Customer_web.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Controllers
{
    
    public class WalletController : Controller
    {
        public JsonResult GetBalance()
        {
            var listParam = new List<KeyValuePair<string, string>>();
            listParam.Add(new KeyValuePair<string, string>("CustomerId",Convert.ToString(new SessionUtility().GetLoginSession().customerId)));
            string errorMessage = string.Empty;
            var response = new CallService().GetResponse<WalletRes>(APIMethodConst.GetBalanceByCustomerId, listParam,ref errorMessage);
            if (string.IsNullOrEmpty(errorMessage))
                return Json(response);
            return Json(null);
        }


       
    }
}
