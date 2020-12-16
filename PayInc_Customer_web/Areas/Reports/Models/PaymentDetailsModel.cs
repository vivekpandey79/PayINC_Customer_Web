using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.Reports.Models
{
    public class PaymentDetailsModel
    {

    }
    public class PaymentDetailsRes
    {
        public string transferFrom { get; set; }
        public string transferFromMobile { get; set; }
        public string transferFromRole { get; set; }
        public string transferFromName { get; set; }
        public string transferTo { get; set; }
        public string transferToMobile { get; set; }
        public string transferToRole { get; set; }
        public string transferToName { get; set; }
        public string transactionTypeDescription { get; set; }
        public double transactionAmount { get; set; }
        public string transactionCategoryDescription { get; set; }
        public string txnDate { get; set; }
        public double customerOpBal { get; set; }
        public double customerClBal { get; set; }
    }
}
