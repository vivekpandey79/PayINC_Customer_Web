using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.Reports.Models
{
    public class PaymentReportModel
    {
        [Required(ErrorMessage="Please select From Date")]
        
        public DateTime FromDate { get; set; }
        [Required(ErrorMessage = "Please select To Date")]
        public DateTime ToDate { get; set; }
    }
    public class PaymentReportRes
    {
        public int lLoadRequestId { get; set; }
        public string branchIFSCCode { get; set; }
        public int paymentModeId { get; set; }
        public string paymentModeName { get; set; }
        public double loadAmount { get; set; }
        public int loadTransactionId { get; set; }
        public int loadCategoryId { get; set; }
        public object loadDescription { get; set; }
        public int loadRequestedBy { get; set; }
        public string loadRequestedDate { get; set; }
        public int? loadProcessedBy { get; set; }
        public DateTime? loadProcessedDate { get; set; }
        public string loadProcessedTime { get; set; }
        public int? loadApprovedBy { get; set; }
        public string loadApprovedDate { get; set; }
        public string loadApprovedTime { get; set; }
        public DateTime? loadDepositDate { get; set; }
        public string loadDepositTime { get; set; }
        public int? loadStatus { get; set; }
        public string bankRefNo { get; set; }
        public int customerId { get; set; }
        public int serviceChannelId { get; set; }
        public string creditRemarks { get; set; }
        public string loadStatusDescription { get; set; }
        public object customerNumber { get; set; }
        public object requestedBy { get; set; }
        public long approvedBy { get; set; }
        public long processedBy { get; set; }
    }
}
