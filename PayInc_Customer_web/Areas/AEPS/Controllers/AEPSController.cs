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
                var sessionUtility = new SessionUtility();
                var pidData = new PidData();
                XmlSerializer serializer = new XmlSerializer(typeof(PidData));
                using (TextReader reader = new StringReader(input.PidData))
                {
                    pidData = (PidData)serializer.Deserialize(reader);
                }
                var pktlocation = new LocationDatum()
                {
                    Latitude = "26.125321647834",
                    Longitude = "78.09278884562441",
                    IpAddress = "176.9.24.146"
                };
                var objCaptureResponse = new captureResponse()
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
                    sessionKey = pidData.Skey.Text
                };
                var reqInput = new DetailsAepsRequestsReq
                {
                    customerId=sessionUtility.GetLoginSession().customerId,
                    adhaarNumber= input.AadharNumber,
                    accessModeType="",
                    agentId=Convert.ToString(sessionUtility.GetStringSession("AEPSAgentID")),
                    nbin= input.BankName,
                    clientTransactionId="",
                    customerNumber=input.CustomerNumber,
                    deviceSerialNumber="",
                    deviceTransactionId="",
                    ipAddress= pktlocation.IpAddress,
                    latitude=pktlocation.Latitude,
                    longitude=pktlocation.Longitude,
                    merchantTransactionId="",
                    requestRemarks="",
                    req_Timestamp="",
                    transactionAmount=1,
                    transactionType="BE",
                    vendorId=1
                };
                var objCardnumberORUID = new cardnumberORUID()
                {
                    adhaarNumber = input.AadharNumber,
                    indicatorforUID = 0,
                    nationalBankIdentificationNumber = input.BankName,
                };
                var pktAEPS = new AepsBalanceEnquiryInput()
                {
                    cardnumberORUID = objCardnumberORUID,
                    mobileNumber = input.CustomerNumber,
                    //paymentType = "B",// LU - Last Used Bank, Bank- B,PayTM - P, Mpesa - M, Mobikwik - K, Bitcoin - C        
                    timestamp = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),// dd/MM/yyyy HH:mm:ss
                    transactionType = "BE",// Merchant - M, Cash withdraw - C, Bill Payments Prepaid Phone - P, Postpaid Phone- Q, Electricity- E                
                    requestRemarks = "Balance Enquiry Web",
                    deviceTransactionId = "Test",
                    captureResponse = objCaptureResponse,
                    merchantTransactionId = new Random().Next(10000,99999).ToString(),
                    subMerchantId = Convert.ToString(sessionUtility.GetStringSession("AEPSAgentID")),
                };
                var listParams = new List<KeyValuePair<string, string>>();
                listParams.Add(new KeyValuePair<string, string>("detailsAepsRequestsData",JsonConvert.SerializeObject(reqInput)));
                string errorMessage = string.Empty;
                //var response = new CallService().PostResponse<string>("iciciAepsBalanceEnq", pktAEPS,ref errorMessage);
                var response = new CallService().PostWithParams<resAEPSTransaction>("puticiciAepsBalanceEnq", listParams, pktAEPS,ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    var response1 = new resAEPSTransaction();
                    response1.AadhaarNumber = input.AadharNumber;
                    response1.Amount = Convert.ToDecimal(input.Amount);
                    response1.ClientTransactionId = pktAEPS.merchantTransactionId;
                    response1.AEPSModeType = "Balance Enquiry";
                    response1.MobileNumber = input.CustomerNumber;
                    response1.BalanceAmount = 2930;
                    response1.ResponseMessage = "Successfully retrived";
                    return PartialView("AckView", response1);
                }
                else
                {
                    return Json(new { success = false, errorMessage= errorMessage });
                }
               
            }
            catch (Exception)
            {

            }
            return PartialView("AckView");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CaptureWithdrawal(AEPSInput input)
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
                var pktlocation = new LocationDatum()
                {
                    Latitude = "26.125321647834",
                    Longitude = "78.09278884562441",
                    IpAddress = "176.9.24.146"
                };
                var objCaptureResponse = new captureResponse()
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
                    sessionKey = pidData.Skey.Text
                };
                var objCardnumberORUID = new cardnumberORUID()
                {
                    adhaarNumber = input.AadharNumber,
                    indicatorforUID = 0,
                    nationalBankIdentificationNumber = input.BankName,
                };
                var reqInput = new DetailsAepsRequestsReq
                {
                    customerId = sessionUtility.GetLoginSession().customerId,
                    adhaarNumber = input.AadharNumber,
                    accessModeType = "",
                    agentId = Convert.ToString(sessionUtility.GetStringSession("AEPSAgentID")),
                    nbin = input.BankName,
                    clientTransactionId = "",
                    customerNumber = input.CustomerNumber,
                    deviceSerialNumber = "test",
                    deviceTransactionId = "test",
                    ipAddress = pktlocation.IpAddress,
                    latitude = pktlocation.Latitude,
                    longitude = pktlocation.Longitude,
                    merchantTransactionId = "",
                    requestRemarks = "Cash Withdrawal",
                    req_Timestamp = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    transactionAmount =Convert.ToDecimal(input.Amount),
                    transactionType = "CW",
                    vendorId = 1
                };
                var pktAEPS = new AepsBalanceEnquiryInput()
                {
                    cardnumberORUID = objCardnumberORUID,
                    mobileNumber = input.CustomerNumber,
                    //paymentType = "B",// LU - Last Used Bank, Bank- B,PayTM - P, Mpesa - M, Mobikwik - K, Bitcoin - C        
                    timestamp = DateTime.Now.ToString("yyyyMMddHHmmss"),// dd/MM/yyyy HH:mm:ss
                    transactionType = "CW",// Merchant - M, Cash withdraw - C, Bill Payments Prepaid Phone - P, Postpaid Phone- Q, Electricity- E                
                    requestRemarks = "Cash Withdrawal",
                    deviceTransactionId = "Test",
                    captureResponse = objCaptureResponse,
                    merchantTransactionId = new Random().Next(10000, 99999).ToString(),
                    subMerchantId = Convert.ToString(sessionUtility.GetStringSession("AEPSAgentID")),
                };
                string errorMessage = string.Empty;
                var listParams = new List<KeyValuePair<string, string>>();
                listParams.Add(new KeyValuePair<string, string>("detailsAepsRequestsData", JsonConvert.SerializeObject(reqInput)));
                var response = new CallService().PostWithParams<resAEPSTransaction>("puticiciAepsCashWithdrawal", listParams, pktAEPS, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    var response1 = new resAEPSTransaction();
                    response1.AadhaarNumber = input.AadharNumber;
                    response1.Amount = Convert.ToDecimal(input.Amount);
                    response1.ClientTransactionId = pktAEPS.merchantTransactionId;
                    response1.AEPSModeType = "Cash Withdrawal";
                    response1.MobileNumber = input.CustomerNumber;
                    response1.ResponseMessage = "Successfully transferred.";
                    response1.Status = 1;
                    return PartialView("AckView", response1);
                }
                else
                {
                    return Json(new { success = false, errorMessage = errorMessage });
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
                var pktlocation = new LocationDatum()
                {
                    Latitude = "26.125321647834",
                    Longitude = "78.09278884562441",
                    IpAddress = "176.9.24.146"
                };
                var objCaptureResponse = new captureResponse()
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
                    sessionKey = pidData.Skey.Text
                };
                var objCardnumberORUID = new cardnumberORUID()
                {
                    adhaarNumber = input.AadharNumber,
                    indicatorforUID = 0,
                    nationalBankIdentificationNumber = input.BankName,
                };
                var reqInput = new DetailsAepsRequestsReq
                {
                    customerId = sessionUtility.GetLoginSession().customerId,
                    adhaarNumber = input.AadharNumber,
                    accessModeType = "",
                    agentId = Convert.ToString(DateTime.Now.ToString("yyyyMMddHHmmss")),
                    nbin = input.BankName,
                    clientTransactionId = "",
                    customerNumber = input.CustomerNumber,
                    deviceSerialNumber = "test",
                    deviceTransactionId = "test",
                    ipAddress = pktlocation.IpAddress,
                    latitude = pktlocation.Latitude,
                    longitude = pktlocation.Longitude,
                    merchantTransactionId = "",
                    requestRemarks = "Mini Statement",
                    req_Timestamp = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    transactionAmount = Convert.ToDecimal(input.Amount),
                    transactionType = "B",
                    vendorId = 1
                };
                var pktAEPS = new AepsBalanceEnquiryInput()
                {
                    cardnumberORUID = objCardnumberORUID,
                    mobileNumber = input.CustomerNumber,
                    //paymentType = "B",// LU - Last Used Bank, Bank- B,PayTM - P, Mpesa - M, Mobikwik - K, Bitcoin - C        
                    timestamp = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),// dd/MM/yyyy HH:mm:ss
                    transactionType = "B",// Merchant - M, Cash withdraw - C, Bill Payments Prepaid Phone - P, Postpaid Phone- Q, Electricity- E                
                    requestRemarks = "Mini Statement",
                    deviceTransactionId = "Test",
                    captureResponse = objCaptureResponse,
                    merchantTransactionId = new Random().Next(10000, 99999).ToString(),
                    subMerchantId = sessionUtility.GetLoginSession().customerId.ToString(),
                };
                string errorMessage = string.Empty;
                var listParams = new List<KeyValuePair<string, string>>();
                listParams.Add(new KeyValuePair<string, string>("detailsAepsRequestsData", JsonConvert.SerializeObject(reqInput)));
                var response = new CallService().PostWithParams<resAEPSTransaction>("puticiciAepsCashWithdrawal", listParams, pktAEPS, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    var response1 = new resAEPSTransaction();
                    response1.AadhaarNumber = input.AadharNumber;
                    response1.Amount = Convert.ToDecimal(input.Amount);
                    response1.ClientTransactionId = pktAEPS.merchantTransactionId;
                    response1.AEPSModeType = "Mini Statement";
                    response1.MobileNumber = input.CustomerNumber;
                    return PartialView("AckView", response1);
                }
                else
                {
                    return Json(new { success = false, errorMessage = errorMessage });
                }
            }
            catch (Exception)
            {

            }
            return PartialView("AckView");
        }
    }
}
