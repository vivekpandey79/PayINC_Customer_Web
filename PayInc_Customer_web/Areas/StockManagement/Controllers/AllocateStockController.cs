using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PayInc_Customer_web.Models;

namespace PayInc_Customer_web.Areas.StockManagement.Controllers
{
    [Area("StockManagement")]
    [Authentication]
    public class AllocateStockController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
