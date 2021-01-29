using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.Reports.Models
{
    public class ViewTicketRes
    {
        public int TicketId { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmailId { get; set; }
        public string AlternateMobileNumber { get; set; }
        public int? TransactionId { get; set; }
        public string CustomerRemark { get; set; }
        public string BookedOn { get; set; }
        public int? BookedBy { get; set; }
        public string BookedRemarks { get; set; }
        public string LanguageName { get; set; }
        public string SubCategoryName { get; set; }
        public string SourceName { get; set; }
        public string ChannelName { get; set; }
        public string TicketStatusName { get; set; }
        public string ClosedOn { get; set; }
        public string ClosedRemarks { get; set; }
    }
}
