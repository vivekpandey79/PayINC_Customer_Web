using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayInc_Customer_web.Areas.AEPS.Models;
using PayInc_Customer_web.Areas.OnBoarding.Models;
using PayInc_Customer_web.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Areas.AEPS.Controllers
{
    [Area("AEPS")]
    [Authentication]
    public class AEPSSettlementController : Controller
    {
        
        // GET: AEPSSettlementController
        public IActionResult Index()
        {
            //GetAccountsList();
            ViewData["AvlLimit"]=GetAvailableAmt();
            return View();
        }

        public IActionResult SettleToWallet()
        {
            ViewData["AvlLimit"] = GetAvailableAmt();
            return View();
        }

        [HttpPost]
        public IActionResult SubmitSettleToBank()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DoTransaction(IFormCollection fc)
        {
            try
            {
                var req = new {
                    customerId=new SessionUtility().GetLoginSession().customerId,
                    txnAmount=Convert.ToDouble(fc["Amount"]),
                    serviceProviderId=97,
                    serviceCircleId=1,
                    serviceChannelId=2,
                    remarks="AEPS Settlement",
                    tPin=new PasswordHash().HashShA1(fc["Password"]),
                    aepsRecordId=Convert.ToInt32(fc["recordId"]),
                    paymentMode=Convert.ToString(fc["paymode"]),
                    transactionType=1
                };
                string errorMessage = string.Empty;
                var response = new CallService().PostTransaction<TransactionResult>("DoAepsSettlementTransactions",req,ref errorMessage);
                if (response != null)
                {
                    response.response.SettlementType = "Settle to Bank";
                }
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return PartialView("AckView", response);
                }
                else
                {
                    return PartialView("AckView", response);
                }
            }
            catch (Exception)
            {
                return PartialView("AckView");
            }
        }

        [HttpPost]
        public IActionResult DoWalletTransaction(IFormCollection fc)
        {
            try
            {
                var req = new
                {
                    customerId = new SessionUtility().GetLoginSession().customerId,
                    txnAmount = Convert.ToDouble(fc["Amount"]),
                    serviceProviderId = 98,
                    serviceCircleId = 1,
                    serviceChannelId = 1,
                    remarks = "AESP Wallet Settlement",
                    tPin = new PasswordHash().HashShA1(fc["Password"]),
                    aepsRecordId = 0,
                    paymentMode = "NA",
                    transactionType = 2
                };
                string errorMessage = string.Empty;
                var response = new CallService().PostTransaction<TransactionResult>("DoAepsSettlementTransactions", req, ref errorMessage);
                if (response != null)
                {
                    response.response.SettlementType = "Settle to Wallet";
                }
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return PartialView("AckView", response);
                }
                else
                {
                    return PartialView("AckView", response);
                }
            }
            catch (Exception)
            {
                return PartialView("AckView");
            }
        }
        [HttpPost]
        public IActionResult GetAccountList()
        {
            try
            {
                var listParams = new List<KeyValuePair<string, string>>();
                listParams.Add(new KeyValuePair<string, string>("customerId", Convert.ToString(new SessionUtility().GetLoginSession().customerId)));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<List<AEPSAccountList>>("getAepsSettlementAccountsByCustId", listParams, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return PartialView("AccountList",response);
                }
            }
            catch (Exception)
            {

            }
            return PartialView("AccountList");
        }
        public void GetAccountsList()
        {
            try
            {
                var listParams = new List<KeyValuePair<string, string>>();
                listParams.Add(new KeyValuePair<string, string>("customerId",Convert.ToString(new SessionUtility().GetLoginSession().customerId)));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<List<AEPSAccountList>>("getAepsSettlementAccountsByCustId",listParams,ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    ViewBag.GetAccountList = response;
                }
            }
            catch (Exception)
            {

            }
        }
        public string GetAvailableAmt()
        {
            try
            {

                var listParams = new List<KeyValuePair<string, string>>();
                listParams.Add(new KeyValuePair<string, string>("customerId", Convert.ToString(new SessionUtility().GetLoginSession().customerId)));
                string errorMessage=string.Empty;
                var response = new CallService().GetResponse<AEPSAvailableLimit>("getAEPSSettlementAvailableLimitByCustomerId", listParams, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return response.aepsSettlementAvailableLimit;
                }
            }
            catch (Exception)
            {

            }
            return "";
        }
        [HttpGet]
        public IActionResult GetBankList()
        {
            try
            {
                var listParams = new List<KeyValuePair<string, string>>();
                listParams.Add(new KeyValuePair<string, string>("bankId", "0"));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<List<BankResponse>>("getMasterBanks", listParams,ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return Json(response);
                }
                else
                {
                    return Json(null);
                }
            }
            catch (Exception)
            {

            }
            return Json(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegisterAccount(IFormCollection fc)
        {
            try
            {                
                var req = new {
                    customerId=new SessionUtility().GetLoginSession().customerId,
                    beneficiaryName=Convert.ToString(fc["BeneficiaryName"]),
                    accountNumber= Convert.ToString(fc["AccountNo"]),
                    ifscCode= Convert.ToString(fc["ifsccode"]),
                    imagePath= Convert.ToString(""),
                    ext1="",
                    ext2="",
                    status=1,
                    bankId=Convert.ToInt32(Convert.ToString(fc["ddlBank"]).Split('~')[0])
                };
                string errorMessage = string.Empty;
                var response = new CallService().PostResponse<string>("putAepsSettlementAccounts", req, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return Json(new { success=true });
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
    }
}
