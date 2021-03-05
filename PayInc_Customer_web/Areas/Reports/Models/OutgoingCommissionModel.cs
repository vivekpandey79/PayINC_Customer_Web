using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.Reports.Models
{
    public class OutgoingCommissionModel
    {
    }
    public class OutgoingCommissionRes
    {
        public int commissionId { get; set; }
        public int? commissionTemplateId { get; set; }
        public double? fromAmount { get; set; }
        public double? toAmount { get; set; }
        public int? serviceProviderId { get; set; }
        public int? commissionTypeId { get; set; }
        public double? cnfCommission { get; set; }
        public double? mdCommission { get; set; }
        public double? adCommission { get; set; }
        public double? retailerCommission { get; set; }
        public int? serviceChargeTypeId { get; set; }
        public double? cnfServiceCharge { get; set; }
        public double? mdServiceCharge { get; set; }
        public double? adServiceCharge { get; set; }
        public double? retailerServiceCharge { get; set; }
        public string category { get; set; }
        public int marginStatus { get; set; }
        public string commissionTemplateName { get; set; }
        public string commissionTypeName { get; set; }
        public string serviceChargeTypeName { get; set; }
        public string serviceProviderName { get; set; }
        public string serviceProviderTypeName { get; set; }
        public int customerId { get; set; }
        public long? mobileNumber { get; set; }
        public string customerRoleDesc { get; set; }
        public string name { get; set; }
    }
}
