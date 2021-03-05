using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayInc_Customer_web.Models;
using PayInc_Customer_web.Utility;
using System.Security.Cryptography;
using System.Text;

namespace PayInc_Customer_web.Controllers
{

    public class PasswordSettingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ChangeMPIN(PasswordSettingReq req)
        {
            var httpContextAccessor = new Microsoft.AspNetCore.Http.HttpContextAccessor();
            var result = httpContextAccessor.HttpContext.Session.GetString("LoginDetails");
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResData>(result);
            string errorMessage = string.Empty;
            var parameter = new { customerId = obj.customerId, passwordType = 1, oldPassword = HashShA1(req.oldPassword), newPassword = HashShA1(req.newPassword) };
            var response = new CallService().PostResponse<int>(APIMethodConst.ChangePassword, parameter, ref errorMessage);
            if (string.IsNullOrEmpty(errorMessage))
            {
                return Json(new { success=true,errorMessage="Password Changed" });
            }
            else
            {
                return Json(new { success = false, errorMessage = errorMessage });
            }
        }

        [HttpPost]
        public IActionResult ChangeTPIN(PasswordSettingReq req)
        {
            var httpContextAccessor = new Microsoft.AspNetCore.Http.HttpContextAccessor();
            var result = httpContextAccessor.HttpContext.Session.GetString("LoginDetails");
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResData>(result);
            string errorMessage = string.Empty;
            var parameter = new { customerId = obj.customerId, passwordType = 2, oldPassword = HashShA1(req.oldPassword), newPassword = HashShA1(req.newPassword) };
            var response = new CallService().PostResponse<int>(APIMethodConst.ChangePassword, parameter, ref errorMessage);
            if (string.IsNullOrEmpty(errorMessage))
            {
                return Json(new { success = true, errorMessage = "Password Changed" });
            }
            else
            {
                return Json(new { success = false, errorMessage = errorMessage });
            }
        }

        [HttpPost]
        public IActionResult ResetTPIN(PasswordSettingReq req)
        {
            var httpContextAccessor = new Microsoft.AspNetCore.Http.HttpContextAccessor();
            var result = httpContextAccessor.HttpContext.Session.GetString("LoginDetails");
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResData>(result);
            string errorMessage = string.Empty;
            var parameter = new { mobileNumber=obj.mobileNumber, passwordType=2 };
            var response = new CallService().PostResponse<ChangePasswordResponse>(APIMethodConst.ResendPassword, parameter, ref errorMessage);
            if (string.IsNullOrEmpty(errorMessage))
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        public string HashShA1(string password)
        {
            string response = string.Empty;
            try
            {
                using (SHA1Managed sha1 = new SHA1Managed())
                {
                    var hashSh1 = sha1.ComputeHash(Encoding.UTF8.GetBytes(password));

                    return System.Convert.ToBase64String(hashSh1);
                    // declare stringbuilder
                    //var sb = new StringBuilder(hashSh1.Length * 2);
                    //// computing hashSh1
                    //foreach (byte b in hashSh1) { sb.Append(b.ToString("X2").ToLower()); }
                    //return sb.ToString();
                }
            }
            catch
            {

            }
            return response;
        }
    }
}
