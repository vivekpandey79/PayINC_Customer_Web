using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PayInc_Customer_web.Areas.KIT_Management.Controllers
{
    [Area("KIT_Management")]
    public class ActivationKITRequestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
