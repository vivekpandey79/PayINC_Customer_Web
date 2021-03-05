using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.Reports.Models
{
    public class AEPSTransactionModel
    {
        public string TransactionType { get; set; }
        //[Required]
        public string SenderMobileNumber { get; set; }
        //[Required]
        public string AadharNumber { get; set; }
        [Required]
        public string FromDate { get; set; }
        [Required]
        public string ToDate { get; set; }
    }
    public class AEPSTransactionRes
    {
        public int aepsRequestId { get; set; }
        public int customerId { get; set; }
        public string agentId { get; set; }
        public object customerNumber { get; set; }
        public string adhaarNumber { get; set; }
        public decimal? transactionAmount { get; set; }
        public string transactionType { get; set; }
        public string accessmodeType { get; set; }
        public string nbin { get; set; }
        public string timeStamp { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public string ipAddress { get; set; }
        public string deviceSerialNumber { get; set; }
        public string deviceTransactionId { get; set; }
        public string clientTransactionId { get; set; }
        public string merchantTransactionId { get; set; }
        public string requestDate { get; set; }
        public int vendorId { get; set; }
        public string requestRemarks { get; set; }
        public string responseTime { get; set; }
        public int vendorStatus { get; set; }
        public string vendorStatusCode { get; set; }
        public string vendorMessage { get; set; }
        public string vendorTransactionId { get; set; }
        public decimal? balanceAmount { get; set; }
        public string bankRrn { get; set; }
        public string terminalId { get; set; }
        public string aepsTransactionStatus { get; set; }
        public int transactionId { get; set; }
        public decimal serviceCharge { get; set; }
        public int serviceChargetxnId { get; set; }
        public decimal commission { get; set; }
        public int commissionTxnId { get; set; }
        public string isSettled { get; set; }
        public int settlementTransactionId { get; set; }
        public string message { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public int transactionStatus { get; set; }
        public string transactionStatusDescription { get; set; }
        public decimal? customeropBal { get; set; }
        public decimal? customerclBal { get; set; }
        public string transactionCategoryDescription { get; set; }
    }
}
