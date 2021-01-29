using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayInc_Customer_web.Areas.Reports.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Areas.Reports.Controllers
{
    [Area("Reports")]
    public class TransactionReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Report(IFormCollection fc)
        {
            try
            {
                var fromDate = string.IsNullOrEmpty(Convert.ToString(fc["fromDate"])) ? DateTime.Now.ToString("yyyyMMdd") : Convert.ToDateTime(fc["fromDate"]).ToString("yyyyMMdd");
                var toDate = string.IsNullOrEmpty(Convert.ToString(fc["toDate"])) ? DateTime.Now.ToString("yyyyMMdd") : Convert.ToDateTime(fc["toDate"]).ToString("yyyyMMdd");
                var listParams = new List<KeyValuePair<string, string>>();
                listParams.Add(new KeyValuePair<string, string>("fromDate", Convert.ToString(Convert.ToInt32(fromDate))));
                listParams.Add(new KeyValuePair<string, string>("toDate", Convert.ToString(Convert.ToInt32(toDate))));
                listParams.Add(new KeyValuePair<string, string>("customerId", Convert.ToString(new SessionUtility().GetLoginSession().customerId)));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<List<TransactionRes>>("getRechargeTransactionByCustomer", listParams, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return PartialView("ViewReport", response);
                }
            }
            catch (Exception)
            {

            }
            return PartialView("ViewReport");

        }
        [HttpPost]
        public IActionResult BookComplaint(string txnId)
        {
            try
            {
                var sessionUtility = new SessionUtility();
                var req = new
                {
                    CustomerId = sessionUtility.GetLoginSession().customerId,
                    CustomerName = sessionUtility.GetLoginSession().firstName,
                    CustomerEmailId = "",
                    AlternateMobileNumber = "",
                    CustomerLanguageId = 1,
                    TicketSubCategoryId = 2,
                    TicketSourceId = 1,
                    TicketChannelId = 2,
                    TransactionId = txnId,
                    CustomerRemark = "Book Complaint",
                    BookedBy = sessionUtility.GetLoginSession().customerId,
                    BookedRemarks = "Automatic Booked"
                };
                string errorMessage = string.Empty;
                var response = new CallHelpDeskService().PostResponse<string>("putDetailstickets", req, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return Json("Ticket Booked. Ticket ID: " + response);
                }
            }
            catch (Exception)
            {

            }
            return Json(null);
        }
    }
}
