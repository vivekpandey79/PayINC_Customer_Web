using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.DMT.Models
{
    public class MoneyTransferModel
    {
        [Required(ErrorMessage="Please enter mobile number")]
        public string MobileNumber { get; set; }
    }


    public class RegistrationInput
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }

        public string City { get; set; }
        public string State { get; set; }
        public string PinCode { get; set; }
        public string CustomerMobile { get; set; }

        public string OTP { get; set; }

    }
}
