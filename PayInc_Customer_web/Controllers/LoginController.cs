using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PayInc_Customer_web.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {   
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(LoginModelReq req)
        {
            var values = new { userName = req.UserName, password = new PasswordHash().HashShA1(req.Password) };
            string errorMessage = string.Empty;
            try
            {
                var response = new CallService().PostResponse<LoginResData>("getLoginAuth", values, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    if (response==null)
                    {
                        ViewData["ErrorMessage"] = "NO response from server.";
                        return View();
                    }
                    //var listParams = new List<KeyValuePair<string, string>>();
                    //listParams.Add(new KeyValuePair<string, string>("customerId", Convert.ToString(response.customerId)));
                    //var response1 = new CallService().GetResponse<List<ProfileResponse>>("geCustomerProfile", listParams, ref errorMessage);
                    //if (response1 == null)
                    //{
                    //    ViewData["ErrorMessage"] = "NO response from server.";
                    //    return View();
                    //}
                    //response.Address = response1[0].permAddressLine1 + ", " + response1[0].permAddressLine2;
                    HttpContext.Session.SetString("LoginDetails", JsonConvert.SerializeObject(response));
                    var menuList = new MenuBinding().BindSideMenu();
                    HttpContext.Session.SetString("menuList", JsonConvert.SerializeObject(menuList));
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewData["ErrorMessage"] = errorMessage;
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View();
            }
        }
        [HttpGet]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public IActionResult ForgotPassword(LoginModelReq req)
        {
            try
            {
                if (req.MobileNumber != null)
                {
                    var listParam = new List<KeyValuePair<string, string>>();
                    listParam.Add(new KeyValuePair<string, string>("mobileNumber", req.MobileNumber));
                    listParam.Add(new KeyValuePair<string, string>("passwordType", "1"));
                    listParam.Add(new KeyValuePair<string, string>("serviceChannelId", "2"));
                    string errorMessage = string.Empty;
                    var response = new CallService().GetResponse<string>("getForgotPassword", listParam, ref errorMessage);
                    if (string.IsNullOrEmpty(response))
                    {
                        new SessionUtility().SetSession("otpId", response);
                        return Json(new { success = true });
                    }
                    return Json(new { success = false, errorMessage = errorMessage });
                }
                else
                {
                    return Json(new { success = false, errorMessage = "Please enter mobile Number" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }
        [HttpPost]
        public IActionResult  VerifyOTP(IFormCollection fc)
        {
            try
            {
                string otp = Convert.ToString(fc["digit1"]) + Convert.ToString(fc["digit2"]) + Convert.ToString(fc["digit3"]) + Convert.ToString(fc["digit4"]) + Convert.ToString(fc["digit5"]) + Convert.ToString(fc["digit6"]);
                if (otp.Length != 6)
                {
                    return Json(new { success = false, errorMessage = "Please enter 6 digit otp" });
                }
                var listParam = new List<KeyValuePair<string, string>>();
                listParam.Add(new KeyValuePair<string, string>("otp", otp));
                listParam.Add(new KeyValuePair<string, string>("OtpId", new SessionUtility().GetStringSession("otpId")));
                listParam.Add(new KeyValuePair<string, string>("passwordType", "1"));
                listParam.Add(new KeyValuePair<string, string>("serviceChannelId", "2"));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<string>("verifyForgotPasswordOtp", listParam, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return Json(new { success = true });
                }
                return Json(new { success = false, errorMessage = errorMessage });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }
    }
}
