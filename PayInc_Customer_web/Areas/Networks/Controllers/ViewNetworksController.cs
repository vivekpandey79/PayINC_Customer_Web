using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PayInc_Customer_web.Areas.OnBoarding.Models;
using PayInc_Customer_web.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Areas.Networks.Controllers
{
    [Area("Networks")]
    [Authentication]
    public class ViewNetworksController : Controller
    {
        public IActionResult Index()
        {
            try
            {
                var listParam = new List<KeyValuePair<string, string>>();
                listParam.Add(new KeyValuePair<string, string>("cutomerId", Convert.ToString(new SessionUtility().GetLoginSession().customerId)));
                string errorMessage = string.Empty;string errorMessage1 = string.Empty;
                var response = new CallService().GetResponse<List<LowChainResponse>>("getViewCustomerNetwork", listParam, ref errorMessage);
                var employeeList = new CallHelpDeskService().GetResponse<List<EmployeeResponse>>("getEmployeeRolesDetails", listParam, ref errorMessage1);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    if (string.IsNullOrEmpty(errorMessage1))
                    {
                        if (employeeList!=null)
                        {
                            for (int i = 0; i < response.Count; i++)
                            {
                                var employee= employeeList.Where(m => m.EmployeeId == response[i].employeeId).FirstOrDefault();
                                if (employee != null)
                                {
                                    response[i].employeeName = employee.EmployeeName;
                                }
                                
                            }
                        }   
                    }
                    return View(response);
                }
                return View();
            }
            catch (Exception)
            {
                return View();
            }
        }
        [HttpGet]
        public IActionResult ViewNetwork(int customerId)
        {
            try
            {
                var listParam = new List<KeyValuePair<string, string>>();
                listParam.Add(new KeyValuePair<string, string>("cutomerId", Convert.ToString(customerId)));
                string errorMessage = string.Empty; string errorMessage1 = string.Empty;
                var response = new CallService().GetResponse<List<LowChainResponse>>("getViewCustomerNetwork", listParam, ref errorMessage);
                var employeeList = new CallHelpDeskService().GetResponse<List<EmployeeResponse>>("getEmployeeRolesDetails", listParam, ref errorMessage1);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    if (employeeList != null)
                    {
                        for (int i = 0; i < response.Count; i++)
                        {
                            var employee = employeeList.Where(m => m.EmployeeId == response[i].employeeId).FirstOrDefault();
                            if (employee != null)
                            {
                                response[i].employeeName = employee.EmployeeName;
                            }

                        }
                    }
                    return PartialView("LowerNetwork", response);
                }
                return PartialView("LowerNetwork");
            }
            catch (Exception)
            {
                return PartialView("LowerNetwork");
            }
        }
    }
}
