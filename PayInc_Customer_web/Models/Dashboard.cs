using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Models
{
    public class DashboardRes
    {
        public int imageAllocationId { get; set; }
        public int customerRoleId { get; set; }
        public int imageId { get; set; }
        public int imageAllocationStatus { get; set; }
        public string imageUrl { get; set; }
        public int isLinked { get; set; }
        public string linkedUrl { get; set; }
        public int imageStatus { get; set; }
        public int imageIndex { get; set; }
        public int portalId { get; set; }
        public string imageName { get; set; }
        public string imageType { get; set; }
        public string offerTitle { get; set; }
        public string offerDesc { get; set; }
    }
    public class CustomerUsageRes
    {
        public decimal? currentUsage { get; set; }
        public decimal? currentPaymentReceived { get; set; }
        public decimal? currentCommisionReceived { get; set; }
        public decimal? currentMonthUsage { get; set; }
        public decimal? currentMonthPaymentReceived { get; set; }
        public decimal? currentMonthCommisionReceived { get; set; }
    }
    public class NetworkUsage
    {
        public int currentNetworkUsage { get; set; }
        public int currentNetworkPaymentReceived { get; set; }
        public int currentNetworkCommisionReceived { get; set; }
        public int currentNetworkMonthUsage { get; set; }
        public int currentNetworkMonthPaymentReceived { get; set; }
        public int currentNetworkMonthCommisionReceived { get; set; }
    }
    public class Top10LedgerResp
    {
        public int transactionId { get; set; }
        public int? transactionTypeId { get; set; }
        public string customerName { get; set; }
        public string customerRoleDesc { get; set; }
        public int? transactionCustomerIDFrom { get; set; }
        public string consumerNumber { get; set; }
        public string message { get; set; }
        public int? transactionStatus { get; set; }
        public int? serviceChannelId { get; set; }
        public string isoTransactionId { get; set; }
        public string clientTransactionId { get; set; }
        public string operatorId { get; set; }
        public string vendorTransactionId { get; set; }
        public decimal? transactionAmount { get; set; }
        public decimal? creditAmount { get; set; }
        public decimal? debitAmount { get; set; }
        public decimal? customerOpBal { get; set; }
        public decimal? customerClBal { get; set; }
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
    public class Top10TrnxRes
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
        public decimal? customerOpBal { get; set; }
        public decimal? customerClBal { get; set; }
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

    public class DayWiseUsage
    {
        public string Date { get; set; }
        public decimal? usageAmount { get; set; }
        public string walletType { get; set; }
    }
}

