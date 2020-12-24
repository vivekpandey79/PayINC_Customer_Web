using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.Reports.Models
{
    public class StockRequestModel
    {
        public string StockType { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
    public class StockReqRes
    {
        public int stockRequestId { get; set; }
        public int? stockCount { get; set; }
        public string stockType { get; set; }
        public string stockStatus { get; set; }
        public string stockRequestedDate { get; set; }
        public string stockApprovedDate { get; set; }
        public string stockProcessedDate { get; set; }
        public string stockRequestedDescription { get; set; }
        public int? transactionId { get; set; }
    }

}
