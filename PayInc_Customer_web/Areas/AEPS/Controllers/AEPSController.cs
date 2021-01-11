using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PayInc_Customer_web.Areas.AEPS.Models;
using PayInc_Customer_web.Areas.OnBoarding.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Areas.AEPS.Controllers
{
    [Area("AEPS")]
    public class AEPSController : Controller
    {
        public IActionResult Index()
        {
            
            return View();
        }
        [HttpPost]
        public IActionResult RegisterAgent(IFormCollection fc)
        {
            try
            {
                var sessionUtility = new SessionUtility();
                var req1= new {
                    customerId= sessionUtility.GetLoginSession().customerId,
                    mobileNumber= Convert.ToString(sessionUtility.GetLoginSession().mobileNumber),
                    aepsName = Convert.ToString(fc["agentname"]),
                    shopName = Convert.ToString(fc["shopname"]),
                    emailId = "test@gmail.com",
                    pinCode = Convert.ToInt64(fc["pincode"]),
                    address = Convert.ToString(fc["address"]),
                    districtId=1,
                    panCard = Convert.ToString(fc["pannumber"]),
                    panCardImagePath = "http://images.payinc.in/tpskyc/PAN/PAN_CARD_1442_6292.jpeg",
                    aadharNumber = Convert.ToString(fc["aadharnumber"]),
                    longitude = "78.09278884562441",
                    latitude = "26.125321647834",
                    agentStatus = 1,
                    ipAddress = "176.9.24.146",
                    boardedBy = sessionUtility.GetLoginSession().customerId,
                    remark = "AEPS Registration"
                };

                string errorMessage = string.Empty;
                var response = new CallService().PostResponse<string>("putDetailsIciciAepsAgents", req1, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {

                }
            }
            catch (Exception)
            {

            }
            return View("Index");
        }

        public IActionResult AEPSServices()
        {
            return View();
        }

        public IActionResult AEPSAllServices()
        {
            GetBankList();
            var sessionUtility = new SessionUtility();
            var listParam = new List<KeyValuePair<string, string>>();
            listParam.Add(new KeyValuePair<string, string>("mobileNumber", Convert.ToString(sessionUtility.GetLoginSession().mobileNumber)));
            string errorMessage = string.Empty;
            var response = new CallService().GetResponse<List<AEPSAgentRes>>("getDetailsAepsAgents", listParam, ref errorMessage);
            if (string.IsNullOrEmpty(errorMessage))
            {
                sessionUtility.SetSession("AEPSAgentID",response[0].agentId);
            }
            return View();
        }

        public void GetBankList()
        {
            string errorMessage = string.Empty;
            try
            {
                var listParams = new List<KeyValuePair<string, string>>();
                listParams.Add(new KeyValuePair<string, string>("bankId", "0"));
                var response = new CallService().GetResponse<List<BankResponse>>("getMasterBanks", listParams, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    ViewBag.BankList = response;
                }
                else
                {
                    ViewBag.BankList = null;
                }
            }
            catch (Exception)
            {
                ViewBag.BankList = null;
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CaptureBalanceEnquiry(AEPSInput input)
        {
            try
            {
                input.AadharNumber = input.AadharNumber.Replace("-", "").ToString();
                var sessionUtility = new SessionUtility();
                var pidData = new PidData();
                XmlSerializer serializer = new XmlSerializer(typeof(PidData));
                using (TextReader reader = new StringReader(input.PidData))
                {
                    pidData = (PidData)serializer.Deserialize(reader);
                }

                var captureReq = new IciciCaptureInternalResponse {
                    ci = pidData.Skey.Ci,
                    dc = pidData.DeviceInfo.Dc,
                    dpID = pidData.DeviceInfo.DpId,
                    errCode = pidData.Resp.ErrCode,
                    errInfo = pidData.Resp.ErrInfo,
                    fCount = pidData.Resp.FCount,
                    fType = pidData.Resp.FType,
                    hmac = pidData.Hmac,
                    iCount = "0",
                    iType = "0",
                    mc = pidData.DeviceInfo.Mc,
                    mi = pidData.DeviceInfo.Mi,
                    nmPoints = pidData.Resp.NmPoints,
                    pCount = "0",
                    Piddata = pidData.Data.Text,
                    PidDatatype = pidData.Data.Type,
                    pType = "0",
                    qScore = pidData.Resp.QScore,
                    rdsID = pidData.DeviceInfo.RdsId,
                    rdsVer = pidData.DeviceInfo.RdsVer,
                    sessionKey = pidData.Skey.Text,
                    
                };
                var allReqInput = new DetailsAepReq {
                    accessModeType = "",
                    adhaarNumber = input.AadharNumber,
                    agentId = Convert.ToString(sessionUtility.GetStringSession("AEPSAgentID")),
                    customerId = sessionUtility.GetLoginSession().customerId,
                    customerNumber = input.CustomerNumber,
                    deviceSerialNumber = pidData.DeviceInfo.Mi,
                    deviceTransactionId = "",
                    iciciAepsCaptureRes = captureReq,
                    indicatorforUID = 0,
                    ipAddress = "176.9.24.146",
                    latitude = "26.125321647834",
                    longitude = "78.09278884562441",
                    merchantTransactionId = GetOrderID(),
                    nbin = input.BankName,
                    paymentType = "B",
                    requestRemarks = "Balance Enquiry Web",
                    serviceChannelId = 2,
                    transactionAmount = 0,
                    transactionType = "BE",
                    virtualId = string.Empty
                    
                };
                var sting = JsonConvert.SerializeObject(allReqInput);
                string errorMessage = string.Empty;
                var response = new CallService().PostResponse<AEPSResponse>("puticiciAepsBalanceEnq", allReqInput, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    var response1 = new resAEPSTransaction();
                    response1.AadhaarNumber = "XXXXXXXXX" + input.AadharNumber.Substring(input.AadharNumber.ToString().Length - 4, 4); ;
                    response1.Amount = Convert.ToDecimal(input.Amount);
                    response1.ClientTransactionId = allReqInput.merchantTransactionId;
                    response1.TransactionReference = response.data.fpTransactionId;
                    response1.AEPSModeType = "Balance Enquiry";
                    response1.MobileNumber = input.CustomerNumber;
                    response1.BalanceAmount =Convert.ToDecimal(response.data.balanceAmount);
                    response1.Status = 1;
                    response1.BankReferenceNumber = input.BankName;
                    response1.BankResponseMessage = response.data.transactionStatus;
                    response1.ResponseMessage = "Successfully retrived";
                    return PartialView("AckView", response1);
                }
                else
                {
                    var response1 = new resAEPSTransaction();
                    response1.AadhaarNumber = input.AadharNumber;
                    response1.Amount = Convert.ToDecimal(input.Amount);
                    response1.ClientTransactionId = allReqInput.merchantTransactionId;
                    response1.AEPSModeType = "Balance Enquiry";
                    response1.MobileNumber = input.CustomerNumber;
                    response1.BalanceAmount = 0;
                    response1.Status = 0;
                    response1.ResponseMessage = errorMessage;
                    return PartialView("AckView", response1);
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return PartialView("AckView");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CaptureWithdrawal(AEPSInput input)
        {
            try
            {
                input.AadharNumber = input.AadharNumber.Replace("-", "").ToString();
                var sessionUtility = new SessionUtility();
                var pidData = new PidData();
                XmlSerializer serializer = new XmlSerializer(typeof(PidData));
                using (TextReader reader = new StringReader(input.PidData))
                {
                    pidData = (PidData)serializer.Deserialize(reader);
                }
                var captureReq = new IciciCaptureInternalResponse
                {
                    ci = pidData.Skey.Ci,
                    dc = pidData.DeviceInfo.Dc,
                    dpID = pidData.DeviceInfo.DpId,
                    errCode = pidData.Resp.ErrCode,
                    errInfo = pidData.Resp.ErrInfo,
                    fCount = pidData.Resp.FCount,
                    fType = pidData.Resp.FType,
                    hmac = pidData.Hmac,
                    iCount = "0",
                    iType = "0",
                    mc = pidData.DeviceInfo.Mc,
                    mi = pidData.DeviceInfo.Mi,
                    nmPoints = pidData.Resp.NmPoints,
                    pCount = "0",
                    Piddata = pidData.Data.Text,
                    PidDatatype = pidData.Data.Type,
                    pType = "0",
                    qScore = pidData.Resp.QScore,
                    rdsID = pidData.DeviceInfo.RdsId,
                    rdsVer = pidData.DeviceInfo.RdsVer,
                    sessionKey = pidData.Skey.Text,

                };
                var allReqInput = new DetailsAepReq
                {
                    accessModeType = "",
                    adhaarNumber = input.AadharNumber,
                    agentId = Convert.ToString(sessionUtility.GetStringSession("AEPSAgentID")),
                    customerId = sessionUtility.GetLoginSession().customerId,
                    customerNumber = input.CustomerNumber,
                    deviceSerialNumber = pidData.DeviceInfo.Mi,
                    deviceTransactionId = "",
                    iciciAepsCaptureRes = captureReq,
                    indicatorforUID = 0,
                    ipAddress = "176.9.24.146",
                    latitude = "26.125321647834",
                    longitude = "78.09278884562441",
                    merchantTransactionId = GetOrderID(),
                    nbin = input.BankName,
                    paymentType = "B",
                    requestRemarks = "Cash Withdrawal",
                    serviceChannelId = 2,
                    transactionAmount = Convert.ToInt32(input.Amount),
                    transactionType = "CW",
                    virtualId = string.Empty

                };
                string errorMessage = string.Empty;
                var response = new CallService().PostResponse<AEPSResponse>("puticiciAepsCashWithdrawal", allReqInput, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    var response1 = new resAEPSTransaction();
                    response1.AadhaarNumber = input.AadharNumber;
                    response1.Amount = Convert.ToDecimal(input.Amount);
                    response1.ClientTransactionId = allReqInput.merchantTransactionId;
                    response1.AEPSModeType = "Cash Withdrawal";
                    response1.MobileNumber = input.CustomerNumber;
                    response1.BalanceAmount = Convert.ToDecimal(response.data.balanceAmount);
                    response1.ResponseMessage = "Successfully transferred.";
                    response1.Status = 1;
                    return PartialView("AckView", response1);
                }
                else
                {
                    var response1 = new resAEPSTransaction();
                    response1.AadhaarNumber = input.AadharNumber;
                    response1.Amount = Convert.ToDecimal(input.Amount);
                    response1.ClientTransactionId = allReqInput.merchantTransactionId;
                    response1.AEPSModeType = "Cash Withdrawal";
                    response1.MobileNumber = input.CustomerNumber;
                    response1.ResponseMessage = errorMessage;
                    response1.Status = 0;
                    return PartialView("AckView", response1);
                }
                
            }
            catch (Exception)
            {

            }
            return PartialView("AckView");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CaptureMiniStmnt(AEPSInput input)
        {
            try
            {
                var sessionUtility = new SessionUtility();
                var pidData = new PidData();
                XmlSerializer serializer = new XmlSerializer(typeof(PidData));
                using (TextReader reader = new StringReader(input.PidData))
                {
                    pidData = (PidData)serializer.Deserialize(reader);
                }
                var captureReq = new IciciCaptureInternalResponse
                {
                    ci = pidData.Skey.Ci,
                    dc = pidData.DeviceInfo.Dc,
                    dpID = pidData.DeviceInfo.DpId,
                    errCode = pidData.Resp.ErrCode,
                    errInfo = pidData.Resp.ErrInfo,
                    fCount = pidData.Resp.FCount,
                    fType = pidData.Resp.FType,
                    hmac = pidData.Hmac,
                    iCount = "0",
                    iType = "0",
                    mc = pidData.DeviceInfo.Mc,
                    mi = pidData.DeviceInfo.Mi,
                    nmPoints = pidData.Resp.NmPoints,
                    pCount = "0",
                    Piddata = pidData.Data.Text,
                    PidDatatype = pidData.Data.Type,
                    pType = "0",
                    qScore = pidData.Resp.QScore,
                    rdsID = pidData.DeviceInfo.RdsId,
                    rdsVer = pidData.DeviceInfo.RdsVer,
                    sessionKey = pidData.Skey.Text,

                };
                var allReqInput = new DetailsAepReq
                {
                    accessModeType = "",
                    adhaarNumber = input.AadharNumber,
                    agentId = Convert.ToString(sessionUtility.GetStringSession("AEPSAgentID")),
                    customerId = sessionUtility.GetLoginSession().customerId,
                    customerNumber = input.CustomerNumber,
                    deviceSerialNumber = pidData.DeviceInfo.Mi,
                    deviceTransactionId = "test",
                    iciciAepsCaptureRes = captureReq,
                    indicatorforUID = 0,
                    ipAddress = "176.9.24.146",
                    latitude = "26.125321647834",
                    longitude = "78.09278884562441",
                    merchantTransactionId = GetOrderID(),
                    nbin = input.BankName,
                    paymentType = "B",
                    requestRemarks = "Mini Statement",
                    serviceChannelId = 2,
                    transactionAmount = Convert.ToInt32(input.Amount),
                    transactionType = "MS",
                    virtualId = string.Empty
                };
                string errorMessage = string.Empty;
                var response = new CallService().PostResponse<MINIStatementResponse>("puticiciAepsMiniStatement", allReqInput, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    var response1 = new resAEPSTransaction();
                    response1.AadhaarNumber = input.AadharNumber;
                    response1.Amount = Convert.ToDecimal(input.Amount);
                    response1.ClientTransactionId = allReqInput.merchantTransactionId;
                    response1.AEPSModeType = "Mini Statement Web";
                    response1.MobileNumber = input.CustomerNumber;
                    if (response.data!=null)
                    {
                        response1.ministatementList = response.data.miniStatementStructureModel;
                    }
                    
                    return PartialView("AckView", response1);
                }
                else
                {
                    var response1 = new resAEPSTransaction();
                    response1.AadhaarNumber = input.AadharNumber;
                    response1.Amount = Convert.ToDecimal(input.Amount);
                    response1.ClientTransactionId = allReqInput.merchantTransactionId;
                    response1.AEPSModeType = "Mini Statement Web";
                    response1.MobileNumber = input.CustomerNumber;
                    return PartialView("AckView", response1);
                }
            }
            catch (Exception)
            {

            }
            return PartialView("AckView");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CaptureAadharPay(AEPSInput input)
        {
            try
            {
                var sessionUtility = new SessionUtility();
                var pidData = new PidData();
                XmlSerializer serializer = new XmlSerializer(typeof(PidData));
                using (TextReader reader = new StringReader(input.PidData))
                {
                    pidData = (PidData)serializer.Deserialize(reader);
                }
                var captureReq = new IciciCaptureInternalResponse
                {
                    ci = pidData.Skey.Ci,
                    dc = pidData.DeviceInfo.Dc,
                    dpID = pidData.DeviceInfo.DpId,
                    errCode = pidData.Resp.ErrCode,
                    errInfo = pidData.Resp.ErrInfo,
                    fCount = pidData.Resp.FCount,
                    fType = pidData.Resp.FType,
                    hmac = pidData.Hmac,
                    iCount = "0",
                    iType = "0",
                    mc = pidData.DeviceInfo.Mc,
                    mi = pidData.DeviceInfo.Mi,
                    nmPoints = pidData.Resp.NmPoints,
                    pCount = "0",
                    Piddata = pidData.Data.Text,
                    PidDatatype = pidData.Data.Type,
                    pType = "0",
                    qScore = pidData.Resp.QScore,
                    rdsID = pidData.DeviceInfo.RdsId,
                    rdsVer = pidData.DeviceInfo.RdsVer,
                    sessionKey = pidData.Skey.Text,

                };
                var allReqInput = new DetailsAepReq
                {
                    accessModeType = "",
                    adhaarNumber = input.AadharNumber,
                    agentId = Convert.ToString(sessionUtility.GetStringSession("AEPSAgentID")),
                    customerId = sessionUtility.GetLoginSession().customerId,
                    customerNumber = input.CustomerNumber,
                    deviceSerialNumber = pidData.DeviceInfo.Mi,
                    deviceTransactionId = "",
                    iciciAepsCaptureRes = captureReq,
                    indicatorforUID = 0,
                    ipAddress = "176.9.24.146",
                    latitude = "26.125321647834",
                    longitude = "78.09278884562441",
                    merchantTransactionId = GetOrderID(),
                    nbin = input.BankName,
                    paymentType = "B",
                    requestRemarks = "Mini Statement",
                    serviceChannelId = 2,
                    transactionAmount = Convert.ToInt32(input.Amount),
                    transactionType = "MS",
                    virtualId = string.Empty

                };
                string errorMessage = string.Empty;
                var response = new CallService().PostResponse<AEPSResponse>("puticiciAepsMiniStatement", allReqInput, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    var response1 = new resAEPSTransaction();
                    response1.AadhaarNumber = input.AadharNumber;
                    response1.Amount = Convert.ToDecimal(input.Amount);
                    response1.ClientTransactionId = allReqInput.merchantTransactionId;
                    response1.AEPSModeType = "Mini Statement Web";
                    response1.MobileNumber = input.CustomerNumber;
                    return PartialView("AckView", response1);
                }
                else
                {
                    var response1 = new resAEPSTransaction();
                    response1.AadhaarNumber = input.AadharNumber;
                    response1.Amount = Convert.ToDecimal(input.Amount);
                    response1.ClientTransactionId = allReqInput.merchantTransactionId;
                    response1.AEPSModeType = "Mini Statement Web";
                    response1.MobileNumber = input.CustomerNumber;
                    return PartialView("AckView", response1);
                }
            }
            catch (Exception)
            {

            }
            return PartialView("AckView");
        }

        public string GetOrderID()
        {
            try
            {
                DateTime date = System.DateTime.Now;
                string nofoday = Convert.ToString(System.DateTime.Now.DayOfYear);
                if ((nofoday).Length == 1)
                {
                    nofoday = "00" + nofoday;
                }
                else if ((nofoday).Length == 2)
                {
                    nofoday = "0" + nofoday;
                }
                string orderid = date.Year % 10 + "" + nofoday + System.DateTime.Now.Hour + "" + System.DateTime.Now.Minute + "" + System.DateTime.Now.Second + System.DateTime.Now.Millisecond;
                return orderid;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
