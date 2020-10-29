using Newtonsoft.Json.Schema;
using PayInc_Customer_web.Utility;
using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PayInc_Customer_web.Models
{
    public class MenuBinding
    {
        public List<MenuRes> BindSideMenu()
        {
            var httpContextAccessor = new Microsoft.AspNetCore.Http.HttpContextAccessor();
            var result = httpContextAccessor.HttpContext.Session.GetString("LoginDetails");
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResData>(result);
            var listParam = new List<KeyValuePair<string, string>>();
            listParam.Add(new KeyValuePair<string, string>("portalId",Convert.ToString(PortalDetails.PortalId)));
            listParam.Add(new KeyValuePair<string, string>("customerRoleId", Convert.ToString(6)));
            string errorMessage = string.Empty;
            var response = new CallService().GetResponse<List<MenuRes>>(APIMethodConst.GetMenusPortalRoleId, listParam,ref errorMessage);
            if (string.IsNullOrEmpty(errorMessage))
            {
                return response;
            }
            else
            {
                return null;
            }
        }
       
    }
}
