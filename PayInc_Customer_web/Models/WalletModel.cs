using PayInc_Customer_web.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Models
{
    
    public class WalletRes
    {
        public int customerId { get; set; }
        public string customerCode { get; set; }
        public long mobileNumber { get; set; }
        public string outletName { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string emailAddress { get; set; }
        public int? customerCategoryId { get; set; }
        public int? customerWalletId { get; set; }
        public int? customerAccountTypeId { get; set; }
        public decimal customerMainAccountBalance { get; set; }
        public decimal customerFundsInClearing { get; set; }
        public double customerFloatBalance { get; set; }
        public decimal customerLienBalance { get; set; }
        public decimal customerOverDueLiability { get; set; }
        public decimal customerUnclearBalance { get; set; }
        public decimal customerSystemReservedBalance { get; set; }
        public decimal customerEffectiveBalance { get; set; }
        public int? walletTypeId { get; set; }
        public string walletType { get; set; }
        public decimal aepsBalance { get; set; }
    }
    public class WalletDetails
    {
        public decimal GetBalanceByCustomerID(string customerId)
        {
            var listParam = new List<KeyValuePair<string, string>>();
            listParam.Add(new KeyValuePair<string, string>("CustomerId", customerId));
            string errorMessage = string.Empty;
            var response = new CallService().GetResponse<List<WalletRes>>(APIMethodConst.GetBalanceByCustomerId, listParam, ref errorMessage);
            return response[0].customerEffectiveBalance;
        }
    }
}
