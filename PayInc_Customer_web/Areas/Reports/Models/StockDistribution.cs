using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.Reports.Models
{
    public class StockDistributionReq
    {
        public string StockType { get; set; }
        [Required]
        public string FromDate { get; set; }
        [Required]
        public string ToDate { get; set; }
    }
    public class StockDistRes
    {
        public long? transferTo { get; set; }
        public string customerRoleDesc { get; set; }
        public int stockCount { get; set; }
        public string remarks { get; set; }
        public string txnDate { get; set; }
        public int? openingStock { get; set; }
        public int? closingStock { get; set; }
        public string stockType { get; set; }
    }
    public class StockTypeRes
    {
        public string stockTypeId { get; set; }
        public string stockType { get; set; }
        public double basePrice { get; set; }
        public double gst { get; set; }
        public double mrp { get; set; }
        public double msp { get; set; }
    }
}
