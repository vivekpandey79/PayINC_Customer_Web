using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PayInc_Customer_web.Areas.KIT_Management.Models;
using PayInc_Customer_web.Areas.Reports.Models;
using PayInc_Customer_web.Models;
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
        [ValidateAntiForgeryToken]
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
                    response.StockType = input.StockType;
                    response.StockAmount = GetStockTypeById(Convert.ToInt32(input.StockType)) * Convert.ToInt32(input.NumberOfStock);
                    var sessionUtility = new SessionUtility();
                    sessionUtility.SetSession("StockTypeId", input.StockType);
                    sessionUtility.SetSession("StockAmount", Convert.ToString(response.StockAmount));
                    sessionUtility.SetSession("PayeeMobileNo", Convert.ToString(response.mobileNumber));
                    sessionUtility.SetSession("PayeeName", response.firstName + " " + response.lastName);
                    sessionUtility.SetSession("PayeeNumberOfStock", Convert.ToString(input.NumberOfStock));
                    sessionUtility.SetSession("PayeeCustomerID", Convert.ToString(response.customerId));
                    return PartialView("ViewProfile", response);
                }
            }
            catch (Exception)
            {

            }
            return PartialView("ViewProfile");
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


        public double GetStockTypeById(int stockId)
        {
            try
            {
                var listParam = new List<KeyValuePair<string, string>>();
                listParam.Add(new KeyValuePair<string, string>("stockTypeId", Convert.ToString(stockId)));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<List<StockTypeRes>>("getMasterStockTypes", listParam, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    if (response != null)
                    {
                        if (response.Count > 0)
                        {
                            return response[0].msp;
                        }
                    }

                }
            }
            catch (Exception)
            {

            }
            return 0;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TransferStock(ShowProfile input)
        {
            try
            {
                var sessionUtility = new SessionUtility();
                string errorMessage = string.Empty;
                var req = new
                {
                    parentMobileNumber = sessionUtility.GetLoginSession().mobileNumber,
                    childMobileNumber = Convert.ToInt64(sessionUtility.GetStringSession("PayeeMobileNo")),
                    stockTypeId = Convert.ToInt32(sessionUtility.GetStringSession("StockTypeId")),
                    stockCount = Convert.ToInt32(sessionUtility.GetStringSession("PayeeNumberOfStock")),
                    serviceChannelId=2,
                    amount = GetStockTypeById(Convert.ToInt32(sessionUtility.GetStringSession("StockTypeId")))*Convert.ToInt32(sessionUtility.GetStringSession("PayeeNumberOfStock")),
                    tPin = new PasswordHash().HashShA1(input.TPIN)//input.TPIN // new PasswordHash().HashShA1(input.TPIN)
                };
                var response = new CallService().PostResponse<string>("putCustomerStockTransafer", req, ref errorMessage);
                var resp = new PaymentManagement.Models.PaymentTransferAck()
                {
                    PayeeMobileNumber = sessionUtility.GetStringSession("PayeeMobileNo"),
                    PayeeName = sessionUtility.GetStringSession("PayeeName"),
                    Amount = sessionUtility.GetStringSession("PayeeNumberOfStock"),
                    PayeeWalletBal = Convert.ToString(new PayInc_Customer_web.Models.WalletDetails().GetBalanceByCustomerID(sessionUtility.GetStringSession("PayeeCustomerID"))),
                };
                if (string.IsNullOrEmpty(errorMessage))
                {
                    resp.Status = "Stock transferred.";
                    resp.StatusId = 1;
                }
                else
                {
                    resp.Status = errorMessage;
                    resp.StatusId = 0;
                }
                return PartialView("AckView", resp);
            }
            catch (Exception)
            {

            }
            return PartialView("AckView");
        }
    }
}
