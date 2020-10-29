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
        public IActionResult Index(LoginModelReq req)
        {
            var values = new { userName = req.UserName, password = req.Password };
            string errorMessage = string.Empty;
            try
            {
                var response = new CallService().PostResponse<LoginResData>("getLoginAuth", values, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    var listParams = new List<KeyValuePair<string, string>>();
                    listParams.Add(new KeyValuePair<string, string>("customerId", Convert.ToString(response.customerId)));
                    var response1 = new CallService().GetResponse<List<ProfileResponse>>("geCustomerProfile", listParams, ref errorMessage);
                    response.Address = response1[0].permAddressLine1 + ", " + response1[0].permAddressLine2;
                    HttpContext.Session.SetString("LoginDetails", JsonConvert.SerializeObject(response));
                    var menuList =new MenuBinding().BindSideMenu();
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
    }
}
