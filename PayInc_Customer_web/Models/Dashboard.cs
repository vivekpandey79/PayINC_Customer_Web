using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Models
{
    public class DashboardRes
    {
        public int imageAllocationId { get; set; }
        public int customerRoleId { get; set; }
        public int imageId { get; set; }
        public int imageAllocationStatus { get; set; }
        public string imageUrl { get; set; }
        public int isLinked { get; set; }
        public string linkedUrl { get; set; }
        public int imageStatus { get; set; }
        public int imageIndex { get; set; }
        public int portalId { get; set; }
        public string imageName { get; set; }
        public string imageType { get; set; }
        public string offerTitle { get; set; }
        public string offerDesc { get; set; }
    }
}
