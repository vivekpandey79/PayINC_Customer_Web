using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.OnBoarding.Models
{
    public class DeclaredKycInfo
    {
        public string mobile { get; set; }
    }

    public class Photo
    {
        public string document_image { get; set; }
        public string match_status { get; set; }
    }

    public class OriginalKycInfo
    {
        public string country { get; set; }
        public string loc { get; set; }
        public string subdist { get; set; }
        public string address { get; set; }
        public string gender { get; set; }
        public string vtc { get; set; }
        public string mobile { get; set; }
        public string dist { get; set; }
        public string doc_type { get; set; }
        public string document_id { get; set; }
        public string house { get; set; }
        public string referenceId { get; set; }
        public string verified_by { get; set; }
        public string pc { get; set; }
        public string street { get; set; }
        public string dob { get; set; }
        public string name { get; set; }
        public string state { get; set; }
        public string landmark { get; set; }
        public string verified_using { get; set; }
        public string po { get; set; }
    }

    public class AadharOTPRes
    {
        public DeclaredKycInfo declared_kyc_info { get; set; }
        public Photo photo { get; set; }
        public OriginalKycInfo original_kyc_info { get; set; }
    }
}
