using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.Reports.Models
{
    public class StockAllocationModel
    {
        [Required]
        public string StockType { get; set; }
        [Required]
        public string FromDate { get; set; }
        [Required]
        public string ToDate { get; set; }
    }
    public class StockAllocationRes
    {
        public int stockAllocationId { get; set; }
        public int stockAllocatedTo { get; set; }
        public int totalStockAllocated { get; set; }
        public int availableStocks { get; set; }
        public int usedStocks { get; set; }
        public string stockAllocatedDate { get; set; }
        public int stockAllocatedTime { get; set; }
        public int stockTypeId { get; set; }
        public string stockType { get; set; }
    }
}
