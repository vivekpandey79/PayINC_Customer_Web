using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Models
{
    public class PasswordSettingReq
    {
        public int customerId { get; set; }
        public int passwordType { get; set; }
        [Required(ErrorMessage ="Please enter the Old Password")]
        [RegularExpression("^[0-9]+$",ErrorMessage ="Please enter numeric password")]
        public string oldPassword { get; set; }
        [Required(ErrorMessage = "Please enter the New Password")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Please enter numeric password")]
        public string newPassword { get; set; }
        [Required(ErrorMessage = "Please enter the Confirm Password")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Please enter numeric password")]
        [Compare(nameof(newPassword), ErrorMessage = "Passwords don't match.")]

        public string confirmPassword { get; set; }

       
             
    }
    public class ChangePasswordResponse
    {
        
    }
}
