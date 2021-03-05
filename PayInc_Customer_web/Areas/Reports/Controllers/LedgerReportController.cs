 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PayInc_Customer_web.Areas.Reports.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Areas.Reports.Controllers
{
    [Area("Reports")]
    public class LedgerReportController : Controller
    {
        // GET: LedgerReportController
        public ActionResult Index()
        {
            GetTransactionType();
            return View();
        }
        [HttpPost]
        public IActionResult Report(IFormCollection fc)
        {
            try
            {
                var loginData = new SessionUtility().GetLoginSession();
                int customerId = 0;
                if (loginData.customerRoleId!=6)
                {
                    if (!string.IsNullOrEmpty(fc["ddlNetworkUser"]))
                    {
                        customerId = Convert.ToInt32(fc["ddlNetworkUser"]);
                    }
                    else
                    {
                        customerId = loginData.customerId;
                    }
                    
                }
                else
                {
                    customerId = loginData.customerId;
                }
                var fromDate= string.IsNullOrEmpty(Convert.ToString(fc["fromDate"])) ? DateTime.Now.ToString("yyyyMMdd") : Convert.ToDateTime(fc["fromDate"]).ToString("yyyyMMdd");
                var toDate = string.IsNullOrEmpty(Convert.ToString(fc["toDate"])) ? DateTime.Now.ToString("yyyyMMdd") : Convert.ToDateTime(fc["toDate"]).ToString("yyyyMMdd");
                var listParams = new List<KeyValuePair<string, string>>();
                listParams.Add(new KeyValuePair<string, string>("fromDate",Convert.ToString(Convert.ToInt32(fromDate))));
                listParams.Add(new KeyValuePair<string, string>("toDate", Convert.ToString(Convert.ToInt32(toDate))));
                listParams.Add(new KeyValuePair<string, string>("customerId", Convert.ToString(customerId)));
                listParams.Add(new KeyValuePair<string, string>("transactionTypeId", string.IsNullOrEmpty(Convert.ToString(fc["ddlTransType"])) ? "0": Convert.ToString(fc["ddlTransType"])));
                listParams.Add(new KeyValuePair<string, string>("walletTypeId", string.IsNullOrEmpty(Convert.ToString(fc["ddlWalletType"])) ? "1" : Convert.ToString(fc["ddlWalletType"])));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<List<LedgerRes>>("getAccountLedgerByCustomer",listParams, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return PartialView("ViewReport",response);
                }
            }
            catch (Exception)
            {

            }
            return PartialView("ViewReport");

        }

        public void GetTransactionType()
        {
            try
            {
                string json =Startup.AppSetting["TransactionType"];
                //string path = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.IndexOf("\\bin"));
                //path = path + @"\wwwroot\assets\json\transaction_type.json";
                var txnType = JsonConvert.DeserializeObject<List<TransactionTypeRes>>(json);
                if (txnType!=null)
                {
                    ViewBag.TransactionType = txnType;
                }
            }
            catch (Exception)
            {

            }
        }
        [HttpPost]
        public IActionResult BookComplaint(string txnId)
        {
            try
            {
                var sessionUtility = new SessionUtility();
                var req=new {
                    CustomerId= sessionUtility.GetLoginSession().customerId,
                    CustomerName= sessionUtility.GetLoginSession().firstName,
                    CustomerEmailId= "",
                    AlternateMobileNumber ="",
                    CustomerLanguageId=1,
                    TicketSubCategoryId=2,
                    TicketSourceId=1,
                    TicketChannelId=2,
                    TransactionId=txnId,
                    CustomerRemark="Transaction Related",
                    BookedBy= sessionUtility.GetLoginSession().customerId,
                    BookedRemarks="Booked from Web"
                };
                string errorMessage = string.Empty;
                var response = new CallHelpDeskService().PostResponse<string>("putDetailstickets", req,ref errorMessage);
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

        [HttpPost]
        public IActionResult GetNetworkUser()
        {
            try
            {
                var loginData = new SessionUtility().GetLoginSession();
                if (loginData.customerRoleId != 6)
                {
                    var listParams = new List<KeyValuePair<string, string>>();
                    listParams.Add(new KeyValuePair<string, string>("cutomerId", Convert.ToString(loginData.customerId)));
                    string errorMessage = string.Empty;
                    var response = new CallService().GetResponse<List<OnBoarding.Models.LowChainResponse>>("getLowerCustomerNetwork", listParams, ref errorMessage);
                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        return Json(response);
                    }
                }
            }
            catch (Exception)
            {

            }
            return Json("");
        }
    }
}
