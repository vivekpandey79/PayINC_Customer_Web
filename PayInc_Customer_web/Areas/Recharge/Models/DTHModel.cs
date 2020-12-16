using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.Recharge.Models
{
    public class DTHRecord
    {
        public string MonthlyRecharge { get; set; }
        public string Balance { get; set; }
        public string customerName { get; set; }
        public string status { get; set; }
        public string NextRechargeDate { get; set; }
        public string lastrechargedate { get; set; }
        public int lastrechargeamount { get; set; }
        public string planname { get; set; }
    }

    public class DTHPlanResp
    {
        public string tel { get; set; }
        public string @operator { get; set; }
        public List<DTHRecord> records { get; set; }
        public int status { get; set; }
    }
}
