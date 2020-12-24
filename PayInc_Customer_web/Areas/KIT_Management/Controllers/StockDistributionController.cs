using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PayInc_Customer_web.Areas.KIT_Management.Models;
using PayInc_Customer_web.Areas.Reports.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Areas.KIT_Management.Controllers
{
    [Area("KIT_Management")]
    public class StockDistributionController : Controller
    {
        public IActionResult Index()
        {
            GetStockType();
            GetNetworkChain();
            return View();
        }
        [HttpPost]
        public IActionResult ShowProfile(StockDistributionInput input)
        {
            try
            {
                string errorMessage = string.Empty;
                var listParam = new List<KeyValuePair<string, string>>();
                listParam.Add(new KeyValuePair<string, string>("mobileNumber", input.MobileNumber));
                var response = new CallService().GetResponse<ShowProfile>("getCustomerDetailsByMobileNo", listParam, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    response.NumberOfStock = input.NumberOfStock;
                    var sessionUtility = new SessionUtility();
                    sessionUtility.SetSession("StockTypeId", input.StockType);
                    sessionUtility.SetSession("PayeeMobileNo", Convert.ToString(response.mobileNumber));
                    sessionUtility.SetSession("PayeeNumberOfStock", Convert.ToString(input.NumberOfStock));
                    sessionUtility.SetSession("PayeeCustomerID", Convert.ToString(response.customerId));
                    return View("ViewProfile", response);
                }
            }
            catch (Exception)
            {

            }
            return View("ViewProfile");
        }
        public void GetNetworkChain()
        {
            try
            {
                var listParam = new List<KeyValuePair<string, string>>();
                listParam.Add(new KeyValuePair<string, string>("cutomerId", Convert.ToString(new SessionUtility().GetLoginSession().customerId)));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<List<OnBoarding.Models.LowChainResponse>>("getViewCustomerNetwork", listParam, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    if (response.Count > 0)
                    {
                        foreach (var item in response)
                        {
                            item.customerAccountStatus = item.mobileNumber + " - " + item.firstName + " " + item.lastName + " - " + item.customerRoleDesc;
                        }
                        ViewBag.NetworkChain = response;
                    }
                }

            }
            catch (Exception)
            {
                ViewBag.NetworkChain = null;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TransferStock(ShowProfile input) 
        {
            try
            {
                var sessionUtility = new SessionUtility();
                string errorMessage = string.Empty;
                var req = new {
                    parentMobileNumber = sessionUtility.GetLoginSession().mobileNumber,
                    childMobileNumber = Convert.ToInt64(sessionUtility.GetStringSession("PayeeMobileNo")),
                    stockTypeId = Convert.ToInt32(sessionUtility.GetStringSession("stockTypeId")),
                    stockCount = Convert.ToInt32(sessionUtility.GetStringSession("PayeeNumberOfStock")),
                    tpin = string.Empty
                };
                var response = new CallService().PostResponse<string>("putCustomerStockTransafer", req,ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {

                }
            }
            catch (Exception)
            {

            }
            return View();
        }
    }
}
