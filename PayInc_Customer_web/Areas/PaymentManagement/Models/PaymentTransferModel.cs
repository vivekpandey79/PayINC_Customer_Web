using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.PaymentManagement.Models
{
    public class PaymentTransferModel
    {
        [Required(ErrorMessage ="Please enter mobile number")]
        public string MobileNumber { get; set; }
        [Required(ErrorMessage = "Please enter amount")]
        public string Amount { get; set; }
    }

    public class ShowProfile
    {
        public int customerId { get; set; }
        public int customerRoleId { get; set; }
        public long mobileNumber { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public decimal customerMainAccountBalance { get; set; }
        public decimal customerEffectiveBalance { get; set; }
        public string customerRole { get; set; }
        public string customerRoleDesc { get; set; }

        public string amount { get; set; }
        [Required]
        public string TPIN { get; set; }
        [Required]
        public string Remarks { get; set; }
    }

    public class PaymentTransferAck
    {
        public string PayeeMobileNumber { get; set; }
        public string PayeeWalletBal { get; set; }
        public string Amount { get; set; }
        public string Status { get; set; }
    }
}
