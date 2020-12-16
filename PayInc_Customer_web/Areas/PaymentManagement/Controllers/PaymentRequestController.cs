using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PayInc_Customer_web.Areas.PaymentManagement.Models;
using PayInc_Customer_web.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Areas.PaymentManagement.Controllers
{
    [Area("PaymentManagement")]
    [Authentication]
    public class PaymentRequestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PaymentType(IFormCollection fc)
        {
            try
            {
                var listParam = new List<KeyValuePair<string,string>>();
                listParam.Add(new KeyValuePair<string, string>("paymentModeAllName", fc["paymentType"]));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<List<PaymentTypeRes>>("getPaymentModesSubName", listParam,ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return PartialView("PaymentType",response);
                }
            }
            catch (Exception)
            {

            }
            return PartialView("PaymentType");
        }
        [HttpPost]
        public IActionResult GetCompanyBank(IFormCollection fc)
        {
            try
            {
                var sessionUtility = new SessionUtility();
                sessionUtility.SetSession("paymentModeId",fc["paymentType"]);
                var listParam = new List<KeyValuePair<string, string>>();
                listParam.Add(new KeyValuePair<string, string>("bankAccountId", "0"));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<List<BankListResponse>>("getCompanyBankAccounts", listParam, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    
                    return PartialView("BankList", response);
                }
            }
            catch (Exception)
            {

            }
            return PartialView("BankList");
        }


        [HttpPost]
        public IActionResult GetPaymentReqFields(IFormCollection fc)
        {
            try
            {
                var sessionUtility = new SessionUtility();
                var listParam = new List<KeyValuePair<string, string>>();
                listParam.Add(new KeyValuePair<string, string>("bankId", fc["bankId"]));
                sessionUtility.SetSession("bankId", fc["bankId"]);
                listParam.Add(new KeyValuePair<string, string>("paymentModeId", sessionUtility.GetStringSession("paymentModeId")));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<List<RequestInputRes>>("getPaymentReqFieldMapping", listParam, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    var viewModel = new RequestViewModel();
                    viewModel.reqList = response;
                    sessionUtility.SetSession("RequestInput", JsonConvert.SerializeObject(response));
                    return PartialView("RequestField", viewModel);
                }
            }
            catch (Exception)
            {

            }
            return PartialView("RequestField");
        }

        [HttpPost]
        public IActionResult SubmitPaymentReq(RequestViewModel request)
        {
            try
            {
                string errorMessage = string.Empty;
                var sessionUtility = new SessionUtility();
                var requestInput = JsonConvert.DeserializeObject<List<RequestInputRes>>(sessionUtility.GetStringSession("RequestInput"));
                var reqParam = new SubmitPaymentReqInput
                {
                    customerId = sessionUtility.GetLoginSession().customerId,
                    requestedBy = sessionUtility.GetLoginSession().customerId,
                    branchIFSCCode = "ICIC0001238",
                    bankRefNo = "",
                    paymentModeId = Convert.ToInt32(sessionUtility.GetStringSession("paymentModeId")),
                    loadAmount = 0,
                    loadCategoryId = 1,
                    loadDescription = "",
                    bankId = Convert.ToInt32(sessionUtility.GetStringSession("bankId")),
                    loadDepositDate = null,
                    serviceChannelId = 1,
                    paymentSlipPath = ""
                };
                for (int i = 0; i < requestInput.Count; i++)
                {
                    requestInput[i].UsersInput = request.reqList[i].UsersInput;
                }
                foreach (var item in requestInput)
                {
                    if (item.columnMapping.Trim() == GetPropertyName(() => reqParam.bankRefNo))
                    {
                        if (!string.IsNullOrEmpty(reqParam.bankRefNo))
                            reqParam.bankRefNo = reqParam.bankRefNo + " | " + item.UsersInput.Trim();
                        else
                            reqParam.bankRefNo = item.UsersInput.Trim();
                    }
                    if (item.columnMapping.Trim() == GetPropertyName(() => reqParam.loadDepositDate))
                    {
                        reqParam.loadDepositDate = item.UsersInput.Trim();
                    }
                    if (item.columnMapping.Trim() == "LoadAmount")
                    {
                        reqParam.loadAmount = Convert.ToInt32(item.UsersInput.Trim());
                    }
                    if (item.columnMapping.Trim() == GetPropertyName(() => reqParam.branchIFSCCode))
                    {
                        reqParam.branchIFSCCode = item.UsersInput.Trim();
                    }
                }
                var response = new CallService().PostResponse<string>("putDetailsLoadRequest", reqParam, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {                   
                    var ackModel = new Acknowledgement()
                    {
                        BankRefNo = reqParam.bankRefNo,
                        LoadAmount = reqParam.loadAmount,
                        LoadDepositDate = reqParam.loadDepositDate,
                        LoadRequestId =Convert.ToInt32(response),
                        Status = "Success"
                    };
                    return PartialView("AckView", ackModel);
                }
            }
            catch (Exception)
            {

            }
            return PartialView("AckView");
        }


        public string GetPropertyName<T>(Expression<Func<T>> propertyLambda)
        {
            var me = propertyLambda.Body as MemberExpression;

            if (me == null)
            {
                throw new ArgumentException("You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'");
            }

            return me.Member.Name;
        }
    }
}
