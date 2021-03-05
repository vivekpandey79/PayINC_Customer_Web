using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Models
{
    public class ViewProfileModel
    {
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class ProfileResponse
    {
        public int customerId { get; set; }
        public string customerCode { get; set; }
        public int customerBankingId { get; set; }
        public int customerOperatingHoursId { get; set; }
        public int customerProfileId { get; set; }
        public int customerWalletId { get; set; }
        public int customerAuthenticationId { get; set; }
        public int customerRoleId { get; set; }
        public long mobileNumber { get; set; }
        public string outletName { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string fathersName { get; set; }
        public string mothersName { get; set; }
        public int dateOfBirth { get; set; }
        public int genderID { get; set; }
        public int maritalStatusId { get; set; }
        public string corrAddressLine1 { get; set; }
        public string corrAddressLine2 { get; set; }
        public string corrAddressLandMark { get; set; }
        public string permAddressLine1 { get; set; }
        public string permAddressLine2 { get; set; }
        public string permAddressLandMark { get; set; }
        public long phoneNumber { get; set; }
        public string alternatePhoneNumber { get; set; }
        public string emailAddress { get; set; }
        public int customerCategoryId { get; set; }
        public int customerLastLoginDate { get; set; }
        public int customerLastLoginTime { get; set; }
        public int customerInvalidPasswordAttemptCount { get; set; }
        public int customerAccountLockoutCount { get; set; }
        public int customerPasswordId { get; set; }
        public int passwordTypeId { get; set; }
        public string passwordHash { get; set; }
        public int customerAccountTypeId { get; set; }
        public decimal? customerMainAccountBalance { get; set; }
        public decimal? customerFundsInClearing { get; set; }
        public double? customerFloatBalance { get; set; }
        public decimal? customerOverDueLiability { get; set; }
        public decimal? customerUnclearBalance { get; set; }
        public decimal? customerSystemReservedBalance { get; set; }
        public decimal? customerEffectiveBalance { get; set; }
        public long? paretMobileNumber { get; set; }
        public string parentName { get; set; }
        public string area { get; set; }
        public string districtName { get; set; }
        public string stateName { get; set; }



        public decimal? aepsMainAccountBalance { get; set; }
        public decimal? aepsFundsInClearing { get; set; }
        public double? aepsFloatBalance { get; set; }
        public decimal? aepsOverDueLiability { get; set; }
        public decimal? aepsUnclearBalance { get; set; }
        public decimal? aepsSystemReservedBalance { get; set; }
        public decimal? aepsEffectiveBalance { get; set; }

        public string customerRole { get; set; }
        public string customerRoleDesc { get; set; }
        public int? walletTypeId { get; set; }
        public string walletType { get; set; }
        public int? customerAccountStatus { get; set; }
        public decimal? aepsBalance { get; set; }
    }

}
