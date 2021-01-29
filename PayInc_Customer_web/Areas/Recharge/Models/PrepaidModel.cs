using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.Recharge.Models
{
    public class PrepaidModel
    {
        [Required(ErrorMessage = "Please enter mobile number")]
        [MinLength(10, ErrorMessage = "10 digit number required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "MobileNumber must be numeric")]
        public long MobileNumber { get; set; }
        [Required(ErrorMessage = "Please select operator")]
        public int? OperatorId { get; set; }
        public long Amount { get; set; }
    }
    public class OperatorResponse
    {
        public int serviceProviderId { get; set; }
        public int serviceProviderTypeId { get; set; }
        public string serviceProviderName { get; set; }
        public string serviceProviderShortName { get; set; }
        public int serviceProviderStatus { get; set; }
        public string serviceProviderImage { get; set; }
    }

    public class TransactionResult
    {
        public bool status { get; set; }
        public int errorCode { get; set; }
        public DoTransactionRes response { get; set; }
        public string message { get; set; }
    }

    public class DoTransactionRes
    {
        public string statusCode { get; set; }
        public string clientTransactionId { get; set; }
        public string vendorTransactionId { get; set; }
        public string operatorId { get; set; }
        public string OperatorImage { get; set; }
        public string OperatorNm { get; set; }
        public string amount { get; set; }
        public string customerNumber { get; set; }
        public string txnId { get; set; }
        public string message { get; set; }
        public object addlParameters { get; set; }
    }
    public class PlanRecord
    {
        public string rs { get; set; }
        public string desc { get; set; }
    }

    public class MPlansResponse
    {
        public string tel { get; set; }
        public string @operator { get; set; }
        public List<PlanRecord> records { get; set; }
        public int status { get; set; }
        public double time { get; set; }
    }

    
}
