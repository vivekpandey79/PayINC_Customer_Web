using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PayInc_Customer_web.Areas.Networks.Models;
using PayInc_Customer_web.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Areas.Networks.Controllers
{
    [Area("Networks")]
    [Authentication]
    public class SearchNetworkController : Controller
    {
        public IActionResult Index()
        {
            GetNetworkChain();
            return View();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ViewProfile(SearchNetwork input)
        {
            try
            {
                string errorMessage = string.Empty;
                var listParam = new List<KeyValuePair<string, string>>();
                listParam.Add(new KeyValuePair<string, string>("mobileNumber", input.MobileNumber));
                var response = new CallService().GetResponse<ProfileResponse>("getCustomerDetailsByMobileNo", listParam, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return View("ViewProfile", response);
                }
                else
                {
                    TempData["NTErrorMessage"] = errorMessage;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["NTErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
