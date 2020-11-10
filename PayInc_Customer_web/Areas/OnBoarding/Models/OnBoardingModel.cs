using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.OnBoarding.Models
{
    public class OnBoardingModel
    {
    }
    public class Instance
    {
    }

    public class Essentials
    {
        public string number { get; set; }
        public Instance instance { get; set; }
    }

    public class Instance2
    {
        public string id { get; set; }
        public string callbackUrl { get; set; }
    }

    public class Result
    {
        public string number { get; set; }
        public string name { get; set; }
        public string fatherName { get; set; }
        public string dob { get; set; }
    }

    public class Response
    {
        public string number { get; set; }
        public int id { get; set; }
        public Instance2 instance { get; set; }
        public Result result { get; set; }
    }

    public class OnBoardingResp
    {
        public string service { get; set; }
        public string itemId { get; set; }
        public string task { get; set; }
        public Essentials essentials { get; set; }
        public string accessToken { get; set; }
        public string id { get; set; }
        public Response response { get; set; }

    }


    public class LowChainResponse
    {
        public int customerId { get; set; }
        public string mobileNumber { get; set; }
        public string customerRoleDesc { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string emailAddress { get; set; }
        public string customerAccountStatus { get; set; }

    }

    public class AllInputs
    {
        public string customerId { get; set; }
        public string customerNumber { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string fatherName { get; set; }
        public string dob { get; set; }
        public string parentCustomerId { get; set; }
        public string submittedBy { get; set; }
        public string kycChannel { get; set; }
        public string remarks { get; set; }
    }
}