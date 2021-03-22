using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PayInc_Customer_web.Areas.Reports.Models;
using PayInc_Customer_web.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Areas.Reports.Controllers
{
    [Area("Reports")]
    [Authentication]
    public class StockDistributionController : Controller
    {
        public IActionResult Index()
        {
            GetStockType();
            return View();
        }
        [HttpPost]
        public IActionResult Index(StockDistributionReq input)
        {
            try
            {
                var listParam = new List<KeyValuePair<string, string>>();
                listParam.Add(new KeyValuePair<string, string>("fromDate", Convert.ToString(Convert.ToDateTime(input.FromDate).ToString("yyyyMMdd"))));
                listParam.Add(new KeyValuePair<string, string>("toDate", Convert.ToString(Convert.ToDateTime(input.ToDate).ToString("yyyyMMdd"))));
                listParam.Add(new KeyValuePair<string, string>("customerId", Convert.ToString(new SessionUtility().GetLoginSession().customerId)));
                listParam.Add(new KeyValuePair<string, string>("stockTypeId", input.StockType));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<List<StockDistRes>>("getStockDistribution", listParam, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    ViewBag.StockDistReport = response;
                }
                else
                {
                    ViewData["ErrorMessage"] = errorMessage;
                }
            }
            catch (Exception)
            {

            }
            GetStockType();
            return View(input);
        }

        public void GetStockType()
        {
            try
            {
                var listParam = new List<KeyValuePair<string, string>>();
                listParam.Add(new KeyValuePair<string, string>("stockTypeId", "0"));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<List<StockTypeRes>>("getMasterStockTypes", listParam, ref errorMessage);
                response.Add(new StockTypeRes { stockTypeId = "0", stockType = "All Stocks" });
                if (string.IsNullOrEmpty(errorMessage))
                {
                    ViewBag.StockType = response.OrderBy(m => m.stockTypeId).ToList();
                }
                else
                {
                    ViewBag.StockType = response;
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
