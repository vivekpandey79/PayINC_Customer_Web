using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Models
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthenticationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContextAccessor = new Microsoft.AspNetCore.Http.HttpContextAccessor();
            var result = httpContextAccessor.HttpContext.Session.GetString("LoginDetails");
            if (result == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary {{ "Controller", "Login" },
                                      { "Action", "Index" } });
            }
            var menus = new SessionUtility().GetMenuSession();
            if (menus != null)
            {
                Controller controller = context.Controller as Controller;
                controller.ViewBag.MenuList = menus;
            }

            base.OnActionExecuting(context);
        }
    }
}
