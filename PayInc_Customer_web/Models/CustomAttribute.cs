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
using Microsoft.AspNetCore.Authorization;

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
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                       {
                           { "Action", "LogOutRedirect" },
                           { "Controller", "Login" },
                           { "Area", "SessionExpire" }
                       });
            }
            if (Convert.ToString(context.HttpContext.Request.Headers["Referer"])==null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                       {
                           { "Action", "LogOutRedirect" },
                           { "Controller", "Login" },
                           { "Area", "SessionExpire" }
                       });
            }
            base.OnActionExecuting(context);
        }
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.HttpContext.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
            {
                var httpContextAccessor = new Microsoft.AspNetCore.Http.HttpContextAccessor();
                var menuList = new MenuBinding().BindSideMenu();
                httpContextAccessor.HttpContext.Session.SetString("menuList", JsonConvert.SerializeObject(menuList));
                var menus = new SessionUtility().GetMenuSession();
                if (menus != null)
                {
                    Controller controller = context.Controller as Controller;
                    controller.ViewBag.MenuList = menus;
                }
            }
            base.OnResultExecuting(context);
        }
    }


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class NewAuthenticationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContextAccessor = new Microsoft.AspNetCore.Http.HttpContextAccessor();
            var result = httpContextAccessor.HttpContext.Session.GetString("LoginDetails");
            if (result == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                       {
                           { "Action", "LogOutRedirect" },
                           { "Controller", "Login" },
                       });
            }
            if (Convert.ToString(context.HttpContext.Request.Headers["Referer"]) == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                       {
                           { "Action", "LogOutRedirect" },
                           { "Controller", "Login" },
                       });
            }
            base.OnActionExecuting(context);
        }
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.HttpContext.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
            {
                var httpContextAccessor = new Microsoft.AspNetCore.Http.HttpContextAccessor();
                var menuList = new MenuBinding().BindSideMenu();
                httpContextAccessor.HttpContext.Session.SetString("menuList", JsonConvert.SerializeObject(menuList));
                var menus = new SessionUtility().GetMenuSession();
                if (menus != null)
                {
                    Controller controller = context.Controller as Controller;
                    controller.ViewBag.MenuList = menus;
                }
            }
            base.OnResultExecuting(context);
        }
    }
}
