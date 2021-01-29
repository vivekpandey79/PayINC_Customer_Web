using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayInc_Customer_web.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Controllers
{
    [Authentication]
    public class ViewProfileController : Controller
    {
        public IActionResult Index()
        {
            try
            {
                string errorMessage = string.Empty;
                var listParams = new List<KeyValuePair<string, string>>();
                var httpContextAccessor = new Microsoft.AspNetCore.Http.HttpContextAccessor();
                var result = httpContextAccessor.HttpContext.Session.GetString("LoginDetails");
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResData>(result);
                listParams.Add(new KeyValuePair<string, string>("customerId", Convert.ToString(obj.customerId)));
                var response = new CallService().GetResponse<List<ProfileResponse>>("geCustomerProfile", listParams, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    if (response.Count>1)
                    {
                        response[0].aepsBalance = response.Where(m => m.walletTypeId == 2).FirstOrDefault().customerEffectiveBalance;
                    }
                    return View(response[0]);
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
        
    }
}
