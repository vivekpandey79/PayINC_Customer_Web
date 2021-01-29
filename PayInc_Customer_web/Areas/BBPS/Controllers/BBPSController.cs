using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PayInc_Customer_web.Areas.BBPS.Models;
using PayInc_Customer_web.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Areas.BBPS.Controllers
{
    [Area("BBPS")]
    [Authentication]
    public class BBPSController : Controller
    {
        public IActionResult Index(string TypeName, string TypeId)
        {
            ViewData["Title"] = TypeName;
            GetOpertorList(TypeId);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult InputParams(BBPSModel input)
        {
            try
            {
                var listParams = new List<KeyValuePair<string, string>>();
                listParams.Add(new KeyValuePair<string, string>("serviceProviderId", Convert.ToString(input.Operator)));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<List<InputParamsRes>>("getDetailsBBPSBillerInputParams",listParams,ref errorMessage);
                var sessionUtility = new SessionUtility();
                sessionUtility.SetSession("ServiceProviderId", input.Operator);
                sessionUtility.SetSession("InputParams",JsonConvert.SerializeObject(response));
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return PartialView(response);
                }
            }
            catch (Exception)
            {

            }
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FetchBill(List<InputParamsRes> input)
        {
            try
            {
                var sessionUtility = new SessionUtility();
                var inputData = JsonConvert.DeserializeObject<List<InputParamsRes>>(sessionUtility.GetStringSession("InputParams"));
                for (int i = 0; i < inputData.Count; i++)
                {
                    inputData[i].UserInput = input[i].UserInput;
                }
                sessionUtility.SetSession("InputParams", JsonConvert.SerializeObject(inputData));
                var listParams = new List<KeyValuePair<string, string>>();
                listParams.Add(new KeyValuePair<string, string>("ssCode", Convert.ToString(input[0].SSCode)));
                listParams.Add(new KeyValuePair<string, string>("caNumber", Convert.ToString(input[0].UserInput)));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<BillFetchRes>("getSpecificStepBill", listParams, ref errorMessage);
                sessionUtility.SetSession("FetchBillData", JsonConvert.SerializeObject(response));
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return PartialView(response);
                }
                else
                {
                    ViewBag.InputData = inputData;
                    return PartialView();
                }
            }
            catch (Exception)
            {
                return PartialView();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DoTransaction(IFormCollection fc)
        {
            try
            {
                var tpin = Convert.ToString(fc["digit1"]) + Convert.ToString(fc["digit2"]) + Convert.ToString(fc["digit3"]) + Convert.ToString(fc["digit4"]) + Convert.ToString(fc["digit5"]) + Convert.ToString(fc["digit6"]) + Convert.ToString(fc["digit7"]) + Convert.ToString(fc["digit8"]);
                var listAddtionParam = new List<AdditonalParam>();
                var sessionUtility = new SessionUtility();
                var inputData=JsonConvert.DeserializeObject<List<InputParamsRes>>(sessionUtility.GetStringSession("InputParams"));
                if (inputData!=null)
                {
                    if (inputData.Count>1)
                    {
                        for (int i = 1; i < listAddtionParam.Count; i++)
                        {
                            listAddtionParam.Add(new AdditonalParam { key = inputData[i].columnMapping, value = inputData[i].UserInput });
                        }
                    }
                }
                var fetchResult = JsonConvert.DeserializeObject<BillFetchRes>(sessionUtility.GetStringSession("FetchBillData"));
                decimal billAmount = 0;
                if (Convert.ToString(fc["Amount"]) != null)
                {
                    billAmount = Convert.ToDecimal(fc["Amount"]);
                }
                else
                {
                    billAmount = Convert.ToDecimal(fetchResult.addinfo.bill_amount);
                }
                var req = new
                {
                    customerId = sessionUtility.GetLoginSession().customerId,
                    rechageNumber = Convert.ToString(inputData[0].UserInput),
                    txnAmount = Convert.ToDecimal(billAmount),
                    serviceProviderId = Convert.ToInt32(sessionUtility.GetStringSession("ServiceProviderId")),
                    serviceCircleId = 1,
                    serviceChannelId = 2,
                    remarks = "BBPS",
                    tPin = new PasswordHash().HashShA1(tpin),
                    addlParameters = listAddtionParam

                };
                string errorMessage = string.Empty;
                var response = new CallService().PostTransaction<TransactionResult>("doRecharge", req, ref errorMessage);
                return PartialView("AckView", response);
            }
            catch (Exception)
            {

            }
            return PartialView();
        }

        public void GetOpertorList(string ServiceProviderTypeId)
        {
            var listParams = new List<KeyValuePair<string, string>>();
            listParams.Add(new KeyValuePair<string, string>("ServiceProviderTypeId", ServiceProviderTypeId));
            string errorMessage = string.Empty;
            var response = new CallService().GetResponse<List<ServiceProviderResp>>("getServiceProvidersByTypeId", listParams, ref errorMessage);
            if (string.IsNullOrEmpty(errorMessage))
            {
                ViewBag.OperatorList = response;
            }
        }
    }
}
