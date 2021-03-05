using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using PayInc_Customer_web.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Controllers
{
    [Authentication]
    public class WalletController : Controller
    {
        public JsonResult GetBalance()
        {
            var listParam = new List<KeyValuePair<string, string>>();
            listParam.Add(new KeyValuePair<string, string>("CustomerId", Convert.ToString(new SessionUtility().GetLoginSession().customerId)));
            string errorMessage = string.Empty;
            var response = new CallService().GetResponse<List<WalletRes>>(APIMethodConst.GetBalanceByCustomerId, listParam, ref errorMessage);
            if (string.IsNullOrEmpty(errorMessage))
            {
                //if (response != null)
                //{
                //    if (response.Count > 1)
                //    {
                //        response.Where(m => m.walletTypeId == 1).FirstOrDefault().aepsBalance = response.Where(m => m.walletTypeId == 2).FirstOrDefault().customerEffectiveBalance;
                //    }
                //}
                return Json(response);
            }
            else
            {
                return Json(null);
            }
        }
        public JsonResult GetBalance2()
        {
            var listParam = new List<KeyValuePair<string, string>>();
            listParam.Add(new KeyValuePair<string, string>("CustomerId", Convert.ToString(new SessionUtility().GetLoginSession().customerId)));
            string errorMessage = string.Empty;
            var response = new CallService().GetResponse<List<WalletRes>>(APIMethodConst.GetBalanceByCustomerId, listParam, ref errorMessage);
            if (string.IsNullOrEmpty(errorMessage))
            {
                if (response != null)
                {
                    if (response.Count > 1)
                    {
                        response.Where(m => m.walletTypeId == 2).FirstOrDefault().aepsBalance = response.Where(m => m.walletTypeId == 2).FirstOrDefault().customerEffectiveBalance;
                    }
                }
                return Json(response.Where(m => m.walletTypeId == 1).FirstOrDefault());
            }
            else
            {
                return Json(null);
            }
        }
    }
}
