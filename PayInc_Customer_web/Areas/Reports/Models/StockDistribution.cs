using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.Reports.Models
{
    public class StockDistributionReq
    {
        [Required]
        public string FromDate { get; set; }
        [Required]
        public string ToDate { get; set; }
    }
}
