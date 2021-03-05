using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.Reports.Models
{
    public class GetNetworkPayment
    {
        public long customerId { get; set; }
        public string customerName { get; set; }
        public string customerCode { get; set; }
        public long mobileNumber { get; set; }
        public string outletName { get; set; }
        public decimal paymentReceived { get; set; }
        public decimal paymentTransfer { get; set; }
    }
    public class GetNetworkUsage
    {
        public int customerId { get; set; }
        public string customerName { get; set; }
        public string customerCode { get; set; }
        public object mobileNumber { get; set; }
        public string outletName { get; set; }
        public decimal? mobileUsage { get; set; }
        public decimal? dthUsage { get; set; }
        public decimal? postPaidUsage { get; set; }
        public decimal? utilityUsage { get; set; }
        public decimal? aepsUsage { get; set; }
        public decimal? dmtUsage { get; set; }
        public decimal? cashDepositeUsage { get; set; }
        public decimal? travelUsage { get; set; }
        public decimal? walletUsage { get; set; }
        public decimal? microAtmUsage { get; set; }
        public decimal? insuranceUsage { get; set; }
    }
}
