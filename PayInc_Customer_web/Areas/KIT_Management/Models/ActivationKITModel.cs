using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.KIT_Management.Models
{
    public class ActivationKITModel
    {
        public string NoOfStocks { get; set; }
    }
    public class ActivationKitRes
    {
        public int? stockId { get; set; }
        public string stockSerialNo { get; set; }
        public string stockPINNo { get; set; }
        public int? stockManufactureDate { get; set; }
        public int? stockManufactureTime { get; set; }
        public int? stockExpiryDate { get; set; }
        public int? stockExpiryTime { get; set; }
        public int? stockUsedDate { get; set; }
        public int? stockUsedTime { get; set; }
        public int? stockAllocatedTo { get; set; }
        public int? stockUsedBy { get; set; }
        public int? stockStatus { get; set; }
        public int? stockType { get; set; }
    }
}
