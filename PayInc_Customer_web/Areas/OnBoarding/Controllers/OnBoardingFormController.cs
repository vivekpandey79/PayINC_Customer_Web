using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using PayInc_Customer_web.Areas.OnBoarding.Models;
using PayInc_Customer_web.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Areas.OnBoarding.Controllers
{
    [Area("OnBoarding")]
    [Authentication]
    public class OnBoardingFormController : Controller
    {
        public IActionResult Index(string rolename)
        {
            if (!string.IsNullOrEmpty(rolename))
            {
                new SessionUtility().SetSession("BoardingRoleName", rolename);

            }
            return View();
        }

        [HttpPost]
        public IActionResult VerifyMobileNumber(IFormCollection fc)
        {
            try
            {
                SetSession("custMobileNumber", fc["mobilenumber"]);
                var listParam = new List<KeyValuePair<string, string>>();
                listParam.Add(new KeyValuePair<string, string>("mobileNumber", fc["mobilenumber"]));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<LoginResData>("getCustomerDetailsByMobileNo", listParam,ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return Json(new { success=true, errorMessage="Mobile Number already exist" });
                }
                else
                {
                    var listParam1 = new List<KeyValuePair<string, string>>();
                    listParam1.Add(new KeyValuePair<string, string>("mobileNumber", fc["mobilenumber"]));
                    string errorMessage1 = string.Empty;
                    var reponse1 = new CallService().GetResponse<List<OnBoardedResp>>("getDetailsCustomerOnboarding", listParam1, ref errorMessage1);
                    if (string.IsNullOrEmpty(errorMessage1))
                    {
                        return Json(new { success = true, errorMessage = "This mobile number already in on-boarding proccess." });
                    }
                    return PartialView("VerifyPAN");
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = true, errorMessage=ex.Message });
            }
        }
        [HttpPost]
        public IActionResult CheckPANDetails(IFormCollection fc)
        {
            try
            {
                SetSession("panCardNumber", fc["panNumber"]);
                if (string.IsNullOrEmpty(Convert.ToString(fc["panNumber"])))
                {
                    return Json(new { success = false, errorMessage="Please enter pan card number", data = "" });
                }
                var listParam = new List<KeyValuePair<string, string>>();
                listParam.Add(new KeyValuePair<string, string>("panCardNumber", fc["panNumber"]));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<OnBoardingResp>("getsignzyPanFetch", listParam, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    if (response!=null)
                    {
                        if (response.response != null)
                        {
                            if (response.response.result != null)
                            {
                                var data = new
                                {
                                    response.response.result.name,
                                    response.response.result.fatherName,
                                    response.response.result.dob
                                };
                                SetSession("panName",Convert.ToString(response.response.result.name));
                                SetSession("panfatherName", Convert.ToString(response.response.result.fatherName));
                                SetSession("pandob", Convert.ToString(response.response.result.dob));
                                return Json(new { success = true, message = "", data });
                            }
                            else
                            {
                                return Json(new { success = false, errorMessage, data = "" });
                            }
                        }
                        else
                        {
                            return Json(new { success = false, errorMessage, data = "" });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, errorMessage, data = "" });
                    }
                }
                else
                {
                    return Json(new { success = false, errorMessage, data="" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message, data="" });
            }
        }
        [HttpPost]
        public IActionResult VerifyPANCard(IFormCollection fc)
        {
            try
            {
                var sessionUtility = new SessionUtility();
                var obj = sessionUtility.GetLoginSession();
                if(sessionUtility.GetStringSession("BoardingRoleName") == "retailer")
                {
                    if (obj.customerRoleId != 5)
                    {
                        var listParam = new List<KeyValuePair<string, string>>();
                        listParam.Add(new KeyValuePair<string, string>("parentMobileNumber", Convert.ToString(obj.mobileNumber)));
                        string errorMessage = string.Empty;
                        var response = new CallService().GetResponse<List<LowChainResponse>>("getNetworkADByNumber", listParam, ref errorMessage);
                        if (response.Count > 0)
                        {
                            foreach (var item in response)
                            {
                                item.mobileNumber = item.mobileNumber + " - " + item.firstName + " " + item.lastName + " - " + item.customerRoleDesc;
                            }
                            ViewBag.ADList = response.ToList();
                        }
                        else
                        {
                            return Json(new { success = false, errorMessage = "Distributor list not found. Please try again." });
                        }
                    }
                    else
                    {
                        ViewData["ADCustomerName"] = obj.mobileNumber + " - " + obj.firstName + " " + obj.lastName + " - " + obj.customerRoleDesc;
                    }
                }
                else
                {
                    var listParam = new List<KeyValuePair<string, string>>();
                    listParam.Add(new KeyValuePair<string, string>("cutomerId", Convert.ToString(obj.customerId)));
                    string errorMessage = string.Empty;
                    var response = new CallService().GetResponse<List<LowChainResponse>>("getViewCustomerNetwork", listParam, ref errorMessage);
                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        if (response.Count > 0)
                        {
                            foreach (var item in response)
                            {
                                item.mobileNumber = item.mobileNumber + " - " + item.firstName + " " + item.lastName + " - " + item.customerRoleDesc;
                            }
                            ViewBag.NetworkList = response.ToList();
                        }
                        else
                        {
                            return Json(new { success = false, errorMessage = "Distributor list not found. Please try again." });
                        }
                    }
                }
                
                return PartialView("DistributorList");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult VerifyMethod(IFormCollection fc)
        {
            try
            {
                var sessionUtility = new SessionUtility();
                if (sessionUtility.GetStringSession("BoardingRoleName") == "retailer")
                {
                    if (GetSession<LoginResData>("LoginDetails").customerRoleId == 5)
                    {
                        return PartialView("VerifyMethod");
                    }
                    if (fc["ddlADlist"] == "")
                    {
                        return Json(new { success = true, errorMessage = "Please select Distributor" });
                    }
                    SetSession("distributorId", fc["ddlADlist"]);
                    return PartialView("VerifyMethod");
                }
                else
                {
                    if (sessionUtility.GetLoginSession().customerRoleId==3)
                    {
                        if (Convert.ToInt32(fc["RoleType"])==5)
                        {
                            SetSession("distributorId", fc["PartnerId"]);
                        }
                        else
                        {
                            SetSession("distributorId", fc["DistributorId"]);
                        }
                        return PartialView("VerifyMethod");
                    }
                    else if(sessionUtility.GetLoginSession().customerRoleId == 4)
                    {
                        SetSession("distributorId", Convert.ToString(sessionUtility.GetLoginSession().customerId));
                        return PartialView("VerifyMethod");
                    }
                    else
                    {
                        SetSession("distributorId", Convert.ToString(sessionUtility.GetLoginSession().customerId));
                        return PartialView("VerifyMethod");
                    }
                }
                
            }
            catch (Exception ex)
            {
                return Json(new { success = true, errorMessage = ex.Message });
            }
        }
        [HttpPost]
        public IActionResult SubmitOnBoarding(IFormCollection fc)
        {
            try
            {
                string verifyMode = Convert.ToString(fc["vmode"]);
                var param = new
                {
                    customerId = GetSession<LoginResData>("LoginDetails").customerId,
                    customerNumber = Convert.ToInt64(GetStringSession("custMobileNumber")),
                    panCardNumber = GetStringSession("panCardNumber"),
                    firstName = GetStringSession("panName"),
                    middleName = "",
                    lastName = "",
                    fatherName = GetStringSession("panfatherName"),
                    dob = 0,
                    parentCustomerId = Convert.ToInt64(GetStringSession("distributorId") ?? Convert.ToString(GetSession<LoginResData>("LoginDetails").customerId)),
                    submittedBy = GetSession<LoginResData>("LoginDetails").customerId,
                    kycChannel = verifyMode =="1" ? "Manual Kyc": "Aadhar OTP",
                    remarks = "OnBoarding Request",
                    kycChannelId = Convert.ToInt32(verifyMode),
                    employeeId = 0
                };
                string errorMessage = string.Empty;
                var response = new CallService().PostResponse<string>("putDetailsCustomerOnbBoarding", param, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return Json(new { success = true, errorMessage = errorMessage });
                }
                return Json(new { success = false, errorMessage = errorMessage });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }
        [HttpPost]
        public IActionResult GetNetworkChain(IFormCollection fc)
        {
            try
            {
                var sessionUtility = new SessionUtility();
                var response = new List<LowChainResponse>();
                var listParam = new List<KeyValuePair<string, string>>();
                listParam.Add(new KeyValuePair<string, string>("cutomerId", Convert.ToString(fc["customerId"])));
                string errorMessage = string.Empty;
                response = new CallService().GetResponse<List<LowChainResponse>>("getViewCustomerNetwork", listParam, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    if (response.Count > 0)
                    {
                        foreach (var item in response)
                        {
                            item.mobileNumber = item.mobileNumber + " - " + item.firstName + " " + item.lastName + " - " + item.customerRoleDesc;
                        }
                        if (response.Where(m => m.customerRoleDesc == "PARTNER").ToList().Count > 0)
                        {
                            return Json(new { success = true, role = "PARTNER", responseData = response.Where(m => m.customerRoleDesc == "PARTNER").ToList() });
                        }
                        if (response.Where(m => m.customerRoleDesc == "DISTRIBUTOR").ToList().Count > 0)
                        {
                            return Json(new { success = true, role = "DISTRIBUTOR", responseData = response.Where(m => m.customerRoleDesc == "DISTRIBUTOR").ToList() });
                        }
                    }
                }
                return Json(new { success = false, errorMessage="data not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult GetLowerNetworkChain(IFormCollection fc)
        {
            try
            {
                var listParam = new List<KeyValuePair<string, string>>();
                listParam.Add(new KeyValuePair<string, string>("cutomerId", Convert.ToString(fc["customerId"])));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<List<LowChainResponse>>("getLowerCustomerNetwork", listParam, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    if (response.Count > 0)
                    {
                        foreach (var item in response)
                        {
                            item.mobileNumber = item.mobileNumber + " - " + item.firstName + " " + item.lastName + " - " + item.customerRoleDesc;
                        }
                        response = response.Where(m => m.customerId != Convert.ToInt32(fc["customerId"])).ToList();
                        return Json(new { success = true, role = "", responseData = response.ToList() });
                    }
                }
                return Json(new { success = false, errorMessage = "data not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        public T GetSession<T>(string sessionKey)
        {
            var httpContextAccessor = new HttpContextAccessor();
            var result = httpContextAccessor.HttpContext.Session.GetString(sessionKey);
            return JsonConvert.DeserializeObject<T>(result);
        }
        public string GetStringSession(string sessionKey)
        {
            var httpContextAccessor = new HttpContextAccessor();
            var result = httpContextAccessor.HttpContext.Session.GetString(sessionKey);
            return result;
        }
        public void SetSession(string sessionKey,string data)
        {
            HttpContext.Session.SetString(sessionKey, data);
        }
    }
}
