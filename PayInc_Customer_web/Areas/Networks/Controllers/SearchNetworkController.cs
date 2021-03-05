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
                var response = new CallService().GetResponse<List<OnBoarding.Models.LowChainResponse>>("getLowerAllCustomerNetwork", listParam, ref errorMessage);
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
                    var listParam1 = new List<KeyValuePair<string, string>>();
                    listParam1.Add(new KeyValuePair<string, string>("CustomerId",Convert.ToString(response.customerId)));
                    string errorMessage1 = string.Empty;
                    var response1 = new CallService().GetResponse<List<WalletRes>>(APIMethodConst.GetBalanceByCustomerId, listParam1, ref errorMessage1);
                    if (string.IsNullOrEmpty(errorMessage1))
                    {
                        if (response1.Where(m => m.walletTypeId == 2).ToList().Count>0)
                        {
                            response.aepsEffectiveBalance = response1.Where(m => m.walletTypeId == 2).FirstOrDefault().customerEffectiveBalance;
                            response.aepsFloatBalance = response1.Where(m => m.walletTypeId == 2).FirstOrDefault().customerFloatBalance;
                            response.aepsMainAccountBalance = response1.Where(m => m.walletTypeId == 2).FirstOrDefault().customerMainAccountBalance;
                            response.aepsSystemReservedBalance = response1.Where(m => m.walletTypeId == 2).FirstOrDefault().customerSystemReservedBalance;
                            response.aepsOverDueLiability = response1.Where(m => m.walletTypeId == 2).FirstOrDefault().customerOverDueLiability;
                            response.aepsFundsInClearing = response1.Where(m => m.walletTypeId == 2).FirstOrDefault().customerFundsInClearing;
                        }
                    }
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
