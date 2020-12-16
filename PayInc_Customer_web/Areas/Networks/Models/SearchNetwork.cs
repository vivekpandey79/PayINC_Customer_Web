using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.Networks.Models
{
    public class SearchNetwork
    {
        [Required(ErrorMessage ="Please enter mobile number")]
        public string MobileNumber { get; set; }
    }
}
