using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayInc_Customer_web.Areas.Reports.Models;
using PayInc_Customer_web.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Areas.KIT_Management.Controllers
{
    [Area("KIT_Management")]
    [Authentication]
    public class ActivationKITRequestController : Controller
    {
        public IActionResult Index()
        {
            GetStockType();
            return View();
        }
        [HttpPost]
        public IActionResult SubmitRequest(IFormCollection fc)
        {
            try
            {
                string errorMessage = string.Empty;
                var req = new {
                    customerId = new SessionUtility().GetLoginSession().customerId,
                    stockTypeId = Convert.ToInt32(fc["stockType"]),
                    stockCount = Convert.ToInt32(fc["stockCount"]),
                    serviceChannelId=2,
                    tPin = new PasswordHash().HashShA1(fc["tpin"])
                };
                var response = new CallService().PostResponse<string>("putCustomerStockRequest", req, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, errorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage=ex.Message });
            }
            
        }
        public void GetStockType()
        {
            try
            {
                var listParam = new List<KeyValuePair<string, string>>();
                listParam.Add(new KeyValuePair<string, string>("stockTypeId", "0"));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<List<StockTypeRes>>("getMasterStockTypes", listParam, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    ViewBag.StockType = response;
                }
                else
                {
                    ViewBag.StockType = new List<StockTypeRes>();
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
