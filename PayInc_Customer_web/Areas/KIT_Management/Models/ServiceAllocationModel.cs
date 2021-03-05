using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.KIT_Management.Models
{
    public class ServiceAllocationResponse
    {
        public int stockTypeId { get; set; }
        public string stockType { get; set; }
        public bool isAllocated { get; set; }
    }
    public class StocktypeMasterResponse
    {
        public int stockTypeId { get; set; }
        public string stockType { get; set; }
        public decimal basePrice { get; set; }
        public decimal gst { get; set; }
        public decimal mrp { get; set; }
        public decimal msp { get; set; }
    }
    public class ServiceAllocationInput
    {
        public int stockTypeId { get; set; }
        public long parentMobileNumber { get; set; }
        public long childMobileNumber { get; set; }
        public int serviceChannelId { get; set; }
        public int stockCount { get; set; }
        public decimal amount { get; set; }
        public string tPin { get; set; }
        public string stockType { get; set; }

    }
}
