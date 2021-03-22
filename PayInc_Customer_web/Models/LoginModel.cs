using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Models
{
    public class LoginModelReq
    {
        [Required(ErrorMessage ="Please enter username/mobile number")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please enter password")]
        [MinLength(8,ErrorMessage ="Please enter 8 digit password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter mobile number")]
        public string MobileNumber { get; set; }
    }
    public class ChangePassword
    {
        [Required(ErrorMessage = "Please enter old password")]
        [MinLength(8, ErrorMessage = "Please enter 8 digit password")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "Please enter new password")]
        [MinLength(8, ErrorMessage = "Please enter 8 digit password")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Please enter confirm password")]
        [MinLength(8, ErrorMessage = "Please enter 8 digit password")]
        [Compare("NewPassword", ErrorMessage ="Not match")]
        public string ConfirmPassword { get; set; }
    }


    public class LoginResData
    {
        public int customerId { get; set; }
        public string customerCode { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string middleName { get; set; }
        public int customerBoardedOnDate { get; set; }
        public int customerBoardedOnTime { get; set; }
        public int customerBoardedBy { get; set; }
        public int customerBankingId { get; set; }
        public int customerOperatingHoursId { get; set; }
        public int customerProfileId { get; set; }
        public int customerWalletId { get; set; }
        public int customerAuthenticationId { get; set; }
        public int customerParentId { get; set; }
        public int customerRoleId { get; set; }
        public long mobileNumber { get; set; }
        public string customerRole { get; set; }
        public string customerRoleDesc { get; set; }
         
        public string Address { get; set; }
        public int? customerAccountStatus { get; set; }
        public string accountStatusDescription { get; set; }
        public int? customerLastActivityDate { get; set; }
        public int? customerLastActivityTime { get; set; }
        public int? customerLastLoginDate { get; set; }
        public int? customerLastLoginTime { get; set; }
    }

    public class ApiResponse
    {
        public bool status { get; set; }
        public int errorCode { get; set; }
        public object response { get; set; }
        public string message { get; set; }
    }


}
