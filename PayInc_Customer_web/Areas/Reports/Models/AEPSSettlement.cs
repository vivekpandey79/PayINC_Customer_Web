using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.Reports.Models
{
    public class AEPSSettlement
    {
    }
    public class AEPSSettlementRes
    {
        public int Customerid { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMobileNo { get; set; }
        public string BeneficiaryName { get; set; }
        public string AccountNumber { get; set; }
        public string IFSCCode { get; set; }
        public string Remarks { get; set; }
        public string PaymentMode { get; set; }
        public decimal? RequestedAmount { get; set; }
        public double CustomerOpBal { get; set; }
        public double CustomerClBal { get; set; }
        public string ISOTransactionID { get; set; }
        public string bankRefenreceNo { get; set; }
        public string requestDate { get; set; }
        public string processedDate { get; set; }
        public string transactionStatusDescription { get; set; }
    }
}
