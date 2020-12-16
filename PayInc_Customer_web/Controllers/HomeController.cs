using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PayInc_Customer_web.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Controllers
{
    [AuthenticationAttribute]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            GetDashboard();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public void GetDashboard()
        {
            try
            {
                var listParam = new List<KeyValuePair<string, string>>();
                listParam.Add(new KeyValuePair<string, string>("test","test"));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<List<DashboardRes>>("getDashboardImages", listParam, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    int i = 0;
                    foreach (var item in response)
                    {
                        if (i==0)
                        {
                            item.imageUrl = "/assets/media/dashboard-images/1.png";
                        }
                        else if(i==1)
                        {
                            item.imageUrl = "/assets/media/dashboard-images/2.png";
                        }
                        else
                        {
                            item.imageUrl = "/assets/media/dashboard-images/3.png";
                        }
                        i = i + 1;
                    }
                    ViewBag.DashboardImage = response;
                }
                else
                {

                }
            }
            catch (Exception)
            {

            }
           
        }
    }
}
