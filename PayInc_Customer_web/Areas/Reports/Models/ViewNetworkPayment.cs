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
}
