﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.Recharge.Models
{
    public class PrepaidModel
    {
        [Required(ErrorMessage = "Please enter mobile number")]
        [MinLength(10, ErrorMessage = "10 digit number required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "MobileNumber must be numeric")]
        public long MobileNumber { get; set; }
        [Required(ErrorMessage = "Please select operator")]
        public int? OperatorId { get; set; }
        public long Amount { get; set; }
    }
    public class OperatorResponse
    {
        public int serviceProviderId { get; set; }
        public int serviceProviderTypeId { get; set; }
        public string serviceProviderName { get; set; }
        public string serviceProviderShortName { get; set; }
        public int serviceProviderStatus { get; set; }
        public string serviceProviderImage { get; set; }
    }

}