using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PayInc_Customer_web.Areas.KIT_Management.Models;
using PayInc_Customer_web.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Areas.KIT_Management
{
    [Authentication]
    [Area("KIT_Management")]
    public class ServiceAllocationController : Controller
    {
        public IActionResult Index()
        {
            var listParam = new List<KeyValuePair<string, string>>();
            string errorMessage = string.Empty;
            listParam.Add(new KeyValuePair<string, string>("stockAllocatedTo", new SessionUtility().GetLoginSession().customerId.ToString()));
            var response = new CallService().GetResponse<List<ServiceAllocationResponse>>("getCustomerServiceAllocation", listParam, ref errorMessage);
            if (string.IsNullOrEmpty(errorMessage))
            {
                return View(response);
            }
            else
            {
                TempData["ErrorMessage"] = errorMessage;
                return View();
            }
        }

        public IActionResult Details(int stockTypeId, string stockType)
        {
            try
            {
                var listParam = new List<KeyValuePair<string, string>>();
                string errorMessage = string.Empty;
                listParam.Add(new KeyValuePair<string, string>("stockTypeId", stockTypeId.ToString()));
                var response = new CallService().GetResponse<List<StocktypeMasterResponse>>("getMasterStockTypes", listParam, ref errorMessage);
                var model = new ServiceAllocationInput
                {
                    stockTypeId = stockTypeId,
                    stockType = stockType,
                    childMobileNumber = Convert.ToInt64(new SessionUtility().GetLoginSession().mobileNumber),
                    amount = response[0].msp
                };

                return PartialView(model);
            }
            catch (Exception)
            {

            }

            return PartialView();
        }

        [HttpPost]
        public ActionResult AllocateService(ServiceAllocationInput input)
        {
            try
            {
                var reqParam = new
                {
                    parentMobileNumber = Convert.ToInt64("123456789"),
                    childMobileNumber = new SessionUtility().GetLoginSession().mobileNumber,
                    serviceChannelId = 2, //for Web
                    stockTypeId = input.stockTypeId,
                    stockCount = 1,
                    amount = input.amount,
                    tPin = "payInc",
                };
                string errorMessage = string.Empty;
                var response = new CallService().PostResponse<string>("putCustomerStockTransafer", reqParam, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    TempData["SuccessMessage"] = "Allocation Done.";
                }
                else
                {
                    TempData["ErrorMessage"] = errorMessage;

                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}
