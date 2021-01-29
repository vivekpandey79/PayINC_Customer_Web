using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.AEPS.Models
{
    public class AEPSSettlement
    {

    }
    public class AEPSAccountList
    {
        public int recordId { get; set; }
        public int customerId { get; set; }
        public string beneficiaryName { get; set; }
        public string accountNumber { get; set; }
        public string ifscCCode { get; set; }
        public string imagePath { get; set; }
        public string ext1 { get; set; }
        public string ext2 { get; set; }
        public int status { get; set; }
        public int bankId { get; set; }
        public string submittedOnDate { get; set; }
        public string submittedOnTime { get; set; }
        public string approavedOnDate { get; set; }
        public string approvedOnTime { get; set; }
        public string approvedRemark { get; set; }
        public int approvedBy { get; set; }
        public long customerMobileNumber { get; set; }
        public string name { get; set; }
        public string bankName { get; set; }
        public string customerKYCStatus { get; set; }
    }

    public class AEPSAvailableLimit
    {
       public string aepsSettlementAvailableLimit { get; set; }
    }

    public class SettleToWalletInput
    {
        [Required(ErrorMessage ="Please enter amount")]
        public double Amount { get; set; }
        [Required(ErrorMessage = "Please enter password")]
        public double Password { get; set; }
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
        public string amount { get; set; }
        public string customerNumber { get; set; }
        public string txnId { get; set; }
        public string message { get; set; }
        public string SettlementType { get; set; }
        public List<KeyValue> addlParameters { get; set; }

    }

    public class KeyValue
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class AEPSSettleInput
    {
        [Required]
        public string AccountNo { get; set; }
        [Required]
        public string ddlBank { get; set; }
        [Required]
        public string BeneficiaryName { get; set; }
        [Required]
        public string IfscCode { get; set; }
        [Required]
        public string Amount { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
