using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PayInc_Customer_web.Areas.KIT_Management.Models;
using PayInc_Customer_web.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Areas.KIT_Management.Controllers
{
    [Area("KIT_Management")]
    [Authentication]
    public class ActivationKitController : Controller
    {
        public IActionResult Index()
        {
            try
            {
                var listParams = new List<KeyValuePair<string, string>>();
                listParams.Add(new KeyValuePair<string, string>("stockId", ""));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<List<ActivationKitRes>>("getDetailsStock", listParams, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {

                }
                else
                {

                }
            }
            catch (Exception)
            {

            }
            return View();
        }
    }
}
