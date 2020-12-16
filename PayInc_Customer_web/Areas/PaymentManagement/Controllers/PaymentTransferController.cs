using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PayInc_Customer_web.Areas.PaymentManagement.Models;
using PayInc_Customer_web.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Areas.PaymentManagement.Controllers
{
    [Area("PaymentManagement")]
    [Authentication]
    public class PaymentTransferController : Controller
    {
        public IActionResult Index()
        {
            GetNetworkChain();
            return View();
        }
        [HttpPost]
        public IActionResult ShowProfile(PaymentTransferModel input)
        {
            try
            {
                string errorMessage = string.Empty;
                var listParam = new List<KeyValuePair<string, string>>();
                listParam.Add(new KeyValuePair<string, string>("mobileNumber", input.MobileNumber));
                var response = new CallService().GetResponse<ShowProfile>("getCustomerDetailsByMobileNo", listParam, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    response.amount = input.Amount;
                    var sessionUtility = new SessionUtility();
                    sessionUtility.SetSession("PayeeMobileNo",Convert.ToString(response.mobileNumber));
                    sessionUtility.SetSession("PayeeAmount", Convert.ToString(input.Amount));
                    sessionUtility.SetSession("PayeeCustomerID", Convert.ToString(response.customerId));
                    return PartialView("ShowProfile", response);
                }
            }
            catch (Exception)
            {

            }
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PaymentTransfer(ShowProfile inputReq)
        {
            try
            {
                var sessionUtility = new SessionUtility();
                string errorMessage = string.Empty;
                var req = new {
                    payerMobileNumber= sessionUtility.GetLoginSession().mobileNumber,
                    payeeMobileNumber= Convert.ToInt64(sessionUtility.GetStringSession("PayeeMobileNo")),
                    transferAmount= Convert.ToInt64(sessionUtility.GetStringSession("PayeeAmount")),
                    transferType=1,
                    transactionRequestedBy= sessionUtility.GetLoginSession().customerId,
                    serviceChannelId=1,
                    transactionRemarks= inputReq.Remarks
                };
                var response = new CallService().PostResponse<string>("PaymentTransfer", req, ref errorMessage);
                //if (string.IsNullOrEmpty(errorMessage))
                //{
                    var ackResp = new PaymentTransferAck
                    {
                        PayeeMobileNumber = sessionUtility.GetStringSession("PayeeMobileNo"),
                        PayeeWalletBal = Convert.ToString(new WalletDetails().GetBalanceByCustomerID(sessionUtility.GetStringSession("PayeeCustomerID"))),
                        Amount= sessionUtility.GetStringSession("PayeeAmount"),
                        Status="Successfully Transferred."
                    };
                    return PartialView("AckView", ackResp);
                //}
            }
            catch (Exception)
            {

            }
            return PartialView("AckView");
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
    }
}
