using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public int employeeId { get; set; }
        public string employeeName { get; set; }
        public string employeeMobileNo { get; set; }
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


    #region On Boarding Reponse
    public class OnBoardedResp
    {
        public long OnboardingId { get; set; }
        public long CustomerId { get; set; }
        public long CustomerNumber { get; set; }
        public string PanCardNumber { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public long Dob { get; set; }
        public long ParentCustomerId { get; set; }
        public string SubmittedOn { get; set; }
        public long SubmittedBy { get; set; }
        public long OnboardingStatus { get; set; }
        public string KycChannel { get; set; }
        public string UpdateOn { get; set; }
        public long UpdatedBy { get; set; }
        public string Remarks { get; set; }
    }
    #endregion


    public class DistributorInputReq
    {
        [Required]
        public string RoleType { get; set; }
        [Required(ErrorMessage = "Select Partner Chain")]
        public string PartnerId { get; set; }
        [Required(ErrorMessage = "Select Distributor Chain")]
        public string DistributorId { get; set; }
    }



    public class EmployeeResponse
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string EmailId { get; set; }
        public string MobileNumber { get; set; }
        public int ParentEmployeeId { get; set; }
        public int DesignationId { get; set; }
        public int EmployeeRoleId { get; set; }
        public int LevelId { get; set; }
        public string CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string DOJ { get; set; }
        public string DOB { get; set; }
        public string Address { get; set; }
        public int AreaId { get; set; }
        public int EmployeeStatusId { get; set; }
        public string ModifiedOn { get; set; }
        public int ModifiedBy { get; set; }
        public string Remarks { get; set; }
        public string EmployeePassword { get; set; }
        public string DesignationName { get; set; }
        public string EmployeeRole { get; set; }
        public string LevelName { get; set; }
        public string Area { get; set; }
        public string EmployeeStatusName { get; set; }
    }
}