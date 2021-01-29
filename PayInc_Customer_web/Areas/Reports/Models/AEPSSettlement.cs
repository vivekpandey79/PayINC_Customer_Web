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
    }
}
