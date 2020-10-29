using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Models
{
    public class Cookie
    {
        public void Set(string key, string value, int? expireTime)
        {
            try
            {
                CookieOptions option = new CookieOptions();

                if (expireTime.HasValue)

                    option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
                else
                    option.Expires = DateTime.Now.AddMilliseconds(10);

                var httpContextAccessor = new Microsoft.AspNetCore.Http.HttpContextAccessor();
                httpContextAccessor.HttpContext.Response.Cookies.Append(key, value);
            }
            catch (Exception)
            {

            }
            
        }
        public string Get(string key)
        {
            try
            {
                var httpContextAccessor = new Microsoft.AspNetCore.Http.HttpContextAccessor();
                return httpContextAccessor.HttpContext.Request.Cookies[key];
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
