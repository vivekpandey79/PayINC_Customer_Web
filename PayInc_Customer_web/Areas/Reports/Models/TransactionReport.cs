using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.Reports.Models
{
    public class TransactionReport
    {

    }
    public class TransactionRes
    {
        public int transactionId { get; set; }
        public int transactionTypeId { get; set; }
        public string customerName { get; set; }
        public string customerRoleDesc { get; set; }
        public int transactionCustomerIDFrom { get; set; }
        public string consumerNumber { get; set; }
        public int transactionStatus { get; set; }
        public int serviceChannelId { get; set; }
        public string isoTransactionId { get; set; }
        public string clientTransactionId { get; set; }
        public string operatorId { get; set; }
        public string vendorTransactionId { get; set; }
        public decimal? transactionAmount { get; set; }
        public decimal? creditAmount { get; set; }
        public decimal? debitAmount { get; set; }
        public double customerOpBal { get; set; }
        public double customerClBal { get; set; }
        public int transactionCategoryId { get; set; }
        public string category { get; set; }
        public string serviceProviderName { get; set; }
        public string serviceProviderShortName { get; set; }
        public string txnDate { get; set; }
        public string transactionTypeDescription { get; set; }
        public string transactionStatusDescription { get; set; }
        public string serviceProviderTypeName { get; set; }
        public string serviceChannelName { get; set; }
        public string referenceTransactionId { get; set; }
    }
}
