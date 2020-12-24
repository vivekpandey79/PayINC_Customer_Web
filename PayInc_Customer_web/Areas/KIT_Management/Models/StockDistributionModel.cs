using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.KIT_Management.Models
{
    public class StockDistributionInput
    {
        [Required(ErrorMessage ="Please enter mobile number")]
        public string MobileNumber { get; set; }
        [Required(ErrorMessage = "Please select stock type")]
        public string StockType { get; set; }
        [Required(ErrorMessage = "Please enter number of stock")]
        public int NumberOfStock { get; set; }
        public string mode { get; set; }
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

        public int NumberOfStock { get; set; }
        [Required]
        public string TPIN { get; set; }
        [Required]
        public string Remarks { get; set; }
    }
}
