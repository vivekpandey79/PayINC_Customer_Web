using Microsoft.AspNetCore.Http;
using PayInc_Customer_web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Utility
{
    public class SessionUtility
    {
        public LoginResData GetLoginSession()
        {

            var httpContextAccessor = new HttpContextAccessor();
            var result = httpContextAccessor.HttpContext.Session.GetString("LoginDetails");
            if (result!=null)
            
                return Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResData>(result);
                
            
            return null;
        }
        public List<MenuRes> GetMenuSession()
        {
            var httpContextAccessor = new HttpContextAccessor();
            var result = httpContextAccessor.HttpContext.Session.GetString("menuList");
            if (result != null)
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<MenuRes>>(result);
            return null;
        }

        public string GetStringSession(string sessionKey)
        {
            var httpContextAccessor = new HttpContextAccessor();
            var result = httpContextAccessor.HttpContext.Session.GetString(sessionKey);
            return result;
        }
        public void SetSession(string sessionKey, string data)
        {
            var httpContextAccessor = new HttpContextAccessor();
            httpContextAccessor.HttpContext.Session.SetString(sessionKey, data);
        }

        public void RemoveSession(string sessionKey)
        {
            var httpContextAccessor = new HttpContextAccessor();
            httpContextAccessor.HttpContext.Session.Remove(sessionKey);
        }
    }
}
