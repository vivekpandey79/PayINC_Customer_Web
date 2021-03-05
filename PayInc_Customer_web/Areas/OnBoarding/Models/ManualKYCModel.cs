using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Areas.OnBoarding.Models
{
    public class ManualKYCModel
    {
        [Required(ErrorMessage ="Please enter Mobile Number")]
        public string MobileNumber { get; set; }
        [Required(ErrorMessage = "Please enter PAN Number")]
        public string PANCard { get; set; }

        [Required(ErrorMessage = "Please enter DL Number")]
        public string DLNumber { get; set; }

        [Required(ErrorMessage = "Please enter DL Date of Birth")]
        public string DLDOB { get; set; }
        [Required(ErrorMessage = "Please enter email address")]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "Please select gender")]
        public string Gender { get; set; }
        [Required(ErrorMessage = "Please select maritial status")]
        public string MaritialStatus { get; set; }
        [Required(ErrorMessage = "Please select caste")]
        public string Caste { get; set; }
        [Required(ErrorMessage = "Please select educational qualification")]
        public string EducationalQualification { get; set; }
        [Required(ErrorMessage = "Please select mobile operator")]
        public string MobileOperator { get; set; }
        [Required(ErrorMessage = "Please select physically disabled or not")]
        public string IsPhysicallyDisabled { get; set; }

        [Required(ErrorMessage = "Please enter pincode")]
        public string pincode { get; set; }

        [Required(ErrorMessage = "Please select area")]
        public int? areaId { get; set; }

        [Required(ErrorMessage = "Please enter landmarks")]
        public string landmark { get; set; }
        [Required(ErrorMessage = "Please enter landmarks")]
        public string firmLandmark { get; set; }
        [Required(ErrorMessage = "Please enter pincode")]
        public string firmPinCode { get; set; }
        [Required(ErrorMessage = "Please select area")]
        public int? firmAreaId { get; set; }
        [Required(ErrorMessage = "Please select bank")]
        public string ddlBank { get; set; }

        [Required(ErrorMessage = "Please enter account name")]
        public string accountname { get; set; }

        [Required(ErrorMessage = "Please enter account number")]
        public string bankaccount { get; set; }
    }

    public class AreaByPinCodeRes
    {
        public int areaId { get; set; }
        public int pinCode { get; set; }
        public string area { get; set; }
        public string taluka { get; set; }
        public int districtId { get; set; }
        public int pincodeStatus { get; set; }
        public string districtShortCode { get; set; }
        public string stateShortCode { get; set; }
        public string countryShortCode { get; set; }
        public string districtName { get; set; }
    }

    public class PersonalDetailsInput
    {
        public string EmailAddress { get; set; }
        public string Gender { get; set; }
        public string MaritialStatus { get; set; }
        public string Caste { get; set; }
        public string EducationalQualification { get; set; }
        public string MobileOperator { get; set; }
        public string IsPhysicallyDisabled { get; set; }
        public string OccupationType { get; set; }
        public string EntityType { get; set; }
        
    }

    public class AddressInput
    {
        public string address { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string landmark { get; set; }
        public string pincode { get; set; }
        public string areaId { get; set; }
    }
    public class BasicInput
    {
        public string firmname { get; set; }
        public string firmaddress { get; set; }
        public string firmstate { get; set; }
        public string firmcity { get; set; }
        public string firmdist { get; set; }
        public string firmLandmark { get; set; }
        public string firmPinCode { get; set; }
        public string firmAreaId { get; set; }
    }

    public class AllBasicDetailsInput
    {
        public PersonalDetailsInput personalDetails { get; set; }
        public AddressInput addressDetails { get; set; }

        public BasicInput basicInput { get; set; }
    }

    public class OverviewDetails
    {
        public AllBasicDetailsInput allDetails { get; set; }

        public SetAllValue allValue { get; set; }
    }

    public class DLInstance
    {
        public string id { get; set; }
        public string callbackUrl { get; set; }
    }

    public class DLEssentials
    {
        public string number { get; set; }
        public string dob { get; set; }
        public DLInstance instance { get; set; }
    }

    public class DLInstance2
    {
        public string id { get; set; }
        public string callbackUrl { get; set; }
    }

    public class BadgeDetail
    {
        public string badgeIssueDate { get; set; }
        public string badgeNo { get; set; }
        public string classOfVehicle { get; set; }
    }

    public class NonTransport
    {
        public string to { get; set; }
        public string from { get; set; }
    }

    public class Transport
    {
        public string to { get; set; }
        public string from { get; set; }
    }

    public class DlValidity
    {
        public NonTransport nonTransport { get; set; }
        public string hazardousValidTill { get; set; }
        public Transport transport { get; set; }
        public string hillValidTill { get; set; }
    }

    public class SplitAddress
    {
        public List<string> district { get; set; }
        public List<List<string>> state { get; set; }
        public List<string> city { get; set; }
        public string pincode { get; set; }
        public List<string> country { get; set; }
        public string addressLine { get; set; }
    }

    public class AddressList
    {
        public string completeAddress { get; set; }
        public string type { get; set; }
        public SplitAddress splitAddress { get; set; }
    }

    public class SplitAddress2
    {
        public List<string> district { get; set; }
        public List<List<string>> state { get; set; }
        public List<string> city { get; set; }
        public string pincode { get; set; }
        public List<string> country { get; set; }
        public string addressLine { get; set; }
    }

    public class CovDetail
    {
        public string covCategory { get; set; }
        public string classOfVehicle { get; set; }
        public string covIssuedate { get; set; }
    }

    public class DetailsOfDrivingLicence
    {
        public string dateOfIssue { get; set; }
        public string dateOfLastTransaction { get; set; }
        public string status { get; set; }
        public string lastTransactedAt { get; set; }
        public string name { get; set; }
        public string fatherOrHusbandName { get; set; }
        public string address { get; set; }
        public List<AddressList> addressList { get; set; }
        public string photo { get; set; }
        public SplitAddress2 splitAddress { get; set; }
        public List<CovDetail> covDetails { get; set; }
    }

    public class DLResult
    {
        public string dlNumber { get; set; }
        public string dob { get; set; }
        public List<BadgeDetail> badgeDetails { get; set; }
        public DlValidity dlValidity { get; set; }
        public DetailsOfDrivingLicence detailsOfDrivingLicence { get; set; }
    }

    public class DLResponse
    {
        public string number { get; set; }
        public string dob { get; set; }
        public int id { get; set; }
        public DLInstance2 instance { get; set; }
        public DLResult result { get; set; }
    }

    public class DLFetchResponse
    {
        public string service { get; set; }
        public string itemId { get; set; }
        public string task { get; set; }
        public DLEssentials essentials { get; set; }
        public string accessToken { get; set; }
        public string id { get; set; }
        public DLResponse response { get; set; }
    }

    #region Name Match API
    public class NameMatchResult
    {
        public string name1_vs_name2_matchResult { get; set; }
        public string name1_vs_name2_matchScore { get; set; }
        public string name1_vs_name2_matchReason { get; set; }
        public string name1 { get; set; }
        public string name2 { get; set; }
    }

    public class NameMatchResponse
    {
        public NameMatchResult result { get; set; }
    }
    #endregion


    #region Text API
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Fields
    {
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string FieldN { get; set; }
    }

    public class TextMatchEssentials
    {
        public List<string> images { get; set; }
        public Fields fields { get; set; }
    }

    public class Field1
    {
        public string score { get; set; }
        public string verb { get; set; }
        public string bestMatch { get; set; }
    }

    public class Field2
    {
        public string score { get; set; }
        public string verb { get; set; }
        public string bestMatch { get; set; }
    }

    public class FieldN
    {
        public string score { get; set; }
        public string verb { get; set; }
        public string bestMatch { get; set; }
    }

    public class TextMatchResult
    {
        public Field1 field1 { get; set; }
        public Field2 field2 { get; set; }
        public FieldN fieldN { get; set; }
    }

    public class TextMatchRes
    {
        public TextMatchEssentials essentials { get; set; }
        public string id { get; set; }
        public string patronId { get; set; }
        public TextMatchResult result { get; set; }
    }


    #endregion


    #region Face Match
    public class FaceMatchEssentials
    {
        public string firstImage { get; set; }
        public string secondImage { get; set; }
    }

    public class FaceMatchResult
    {
        public string verified { get; set; }
        public string message { get; set; }
        public string matchPercentage { get; set; }
        public string imgURL1 { get; set; }
        public string imgURL2 { get; set; }
    }

    public class FaceMatchResp
    {
        public FaceMatchEssentials essentials { get; set; }
        public string id { get; set; }
        public string patronId { get; set; }
        public FaceMatchResult result { get; set; }
    }
    #endregion


    #region PAN Face Extraction
    public class FacePANExtnEssentials
    {
        public string url { get; set; }
    }
    public class PANFaceResult
    {
        public string cropped { get; set; }
    }
    public class PANFaceResponse
    {
        public string url { get; set; }
        public int id { get; set; }
        public PANFaceResult result { get; set; }
    }
    public class FacePanRes
    {
        public string service { get; set; }
        public string itemId { get; set; }
        public string task { get; set; }
        public FacePANExtnEssentials essentials { get; set; }
        public string accessToken { get; set; }
        public string id { get; set; }
        public PANFaceResponse response { get; set; }
    }
    #endregion

    #region Bank Master Model

    public class BankResponse
    {
        public int? bankId { get; set; }
        public int? bankTypeId { get; set; }
        public string bankName { get; set; }
        public int? bankIsP2A { get; set; }
        public int? bankIsP2P { get; set; }
        public int? bankIsRRB { get; set; }
        public int? bankNBIN { get; set; }
        public string bankIFSCCheckDigits { get; set; }
        public int? bankIsAcceptor { get; set; }
        public int? bankIsDonor { get; set; }
        public int? bankStatus { get; set; }
        public string ifsc { get; set; }
        public string bankType { get; set; }
    }

    #endregion


    #region set all Values

    public class SetAllValue
    {
        public string FullName { get; set; }
        public string FatherName { get; set; }
        public string MobileNumber { get; set; }
        public string DateOfBirth { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string PinCode { get; set; }
        public string District { get; set; }
        public string BankName { get; set; }
        public string AccountHolderName { get; set; }
        public string BankAccount { get; set; }
        public string BankIFSCCode { get; set; }
    }

    #endregion


    #region VideoVerifcation Response
    public class VideoVerificationResult
    {
        public string token { get; set; }
        public string videoUrl { get; set; }
    }
    public class VideoVerificationResponse
    {
        public VideoVerificationResult result { get; set; }
    }
    #endregion



    #region GET Video Verification Response

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class MatchStatistics
    {
        public string coVariance { get; set; }
        public string matchPercentage { get; set; }
    }

    public class VideoFaceMatch
    {
        public List<string> videoImages { get; set; }
        public string finalMatchImage { get; set; }
        public MatchStatistics matchStatistics { get; set; }
    }

    public class AudioMatch
    {
        public string matchAudioScore { get; set; }
    }

    public class MatchImageFaceMatch
    {
        public object verified { get; set; }
        public object message { get; set; }
        public object matchPercentage { get; set; }
    }

    public class VideoForensics
    {
        public string staticRisk { get; set; }
        public string prerecordedRisk { get; set; }
        public string videoLandMarks { get; set; }
        public List<string> faceLandMarks { get; set; }
    }

    public class VideoVerification
    {
        public List<VideoFaceMatch> videoFaceMatch { get; set; }
        public AudioMatch audioMatch { get; set; }
        public MatchImageFaceMatch matchImageFaceMatch { get; set; }
        public VideoForensics videoForensics { get; set; }
        public string video { get; set; }
        public string otp { get; set; }
        public string faceFound { get; set; }
        public string isAudioProcessed { get; set; }
        public string isVideoProcessed { get; set; }
    }

    public class FinalVideoVerifyResult
    {
        public string token { get; set; }
        public string videoUrl { get; set; }
        public string callbackUrl { get; set; }
        public int isUsed { get; set; }
        public VideoVerification videoVerification { get; set; }
    }

    public class FinalVideoVerifyResp
    {
        public string PanImage { get; set; }
        public FinalVideoVerifyResult result { get; set; }
    }



    #endregion


    #region Get Customer Boading
    public class DetailsCustomerOnboarding
    {
        public int onboardingId { get; set; }
        public int customerId { get; set; }
        public long customerNumber { get; set; }
        public string panCardNumber { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string fatherName { get; set; }
        public string dob { get; set; }
        public int parentCustomerId { get; set; }
        public string submittedOn { get; set; }
        public int submittedBy { get; set; }
        public int onboardingStatus { get; set; }
        public string kycChannel { get; set; }
        public string updateOn { get; set; }
        public int updatedBy { get; set; }
        public string remarks { get; set; }
        public int employeeId { get; set; }
        public string customerKYCStatus { get; set; }
        public string customerName { get; set; }
        public string parentName { get; set; }
        public string parentOutletName { get; set; }
        public string customerOutletName { get; set; }
        public int parentCommissionTemplateId { get; set; }
    }

    public class DetailsKycPanOcrInfo
    {
        public int panOCRInformationId { get; set; }
        public int onboardingId { get; set; }
        public string nameonPancard { get; set; }
        public string fatherName { get; set; }
        public int dob { get; set; }
        public string dateOfBirth { get; set; }
        public string imagePath { get; set; }
        public string submittedOnDate { get; set; }
        public string submittedOnTime { get; set; }
        public int submittedBy { get; set; }
        public int kycStatus { get; set; }
        public string approvedOnDate { get; set; }
        public string approvedOnTime { get; set; }
        public int approvedBy { get; set; }
        public int remarks { get; set; }
        public string panOCRInfo1 { get; set; }
        public string customerKYCStatus { get; set; }
        public string remarksDescription { get; set; }
    }

    public class DetailsKycBasicInfo
    {
        public int basicInformationId { get; set; }
        public int onboardingId { get; set; }
        public string emailId { get; set; }
        public int genderId { get; set; }
        public int maritalStatusId { get; set; }
        public string casteCategory { get; set; }
        public int serviceProviderId { get; set; }
        public string physicalStatus { get; set; }
        public string occupationType { get; set; }
        public string entityType { get; set; }
        public string alternateNumber { get; set; }
        public string submittedOnDate { get; set; }
        public string submittedOnTime { get; set; }
        public int submittedBy { get; set; }
        public int kycStatus { get; set; }
        public string approvedOnDate { get; set; }
        public string approvedOnTime { get; set; }
        public int approvedBy { get; set; }
        public int remarks { get; set; }
        public string basicInfo1 { get; set; }
        public string basicInfo2 { get; set; }
        public string customerKYCStatus { get; set; }
        public string serviceProviderName { get; set; }
        public string maritalStatusName { get; set; }
        public string genderName { get; set; }
        public string remarksDescription { get; set; }
    }

    public class DetailsKycOutletInfo
    {
        public int outletInformationId { get; set; }
        public int onboardingId { get; set; }
        public string outletName { get; set; }
        public int outletCategoryId { get; set; }
        public int outletDistrictId { get; set; }
        public int outletAreaId { get; set; }
        public int outletPinCode { get; set; }
        public string outletLandmark { get; set; }
        public string outletAddress { get; set; }
        public string occupationDuration { get; set; }
        public string outletLatitude { get; set; }
        public string outletLongitude { get; set; }
        public string submittedOnDate { get; set; }
        public string submittedOnTime { get; set; }
        public int submittedBy { get; set; }
        public int kycStatus { get; set; }
        public string approvedOnDate { get; set; }
        public string approvedOnTime { get; set; }
        public int approvedBy { get; set; }
        public int remarks { get; set; }
        public string outletInfo1 { get; set; }
        public string outletInfo2 { get; set; }
        public string area { get; set; }
        public string districtName { get; set; }
        public string stateName { get; set; }
        public string countryName { get; set; }
        public string customerKYCStatus { get; set; }
        public string customerCategory { get; set; }
        public string remarksDescription { get; set; }
    }

    public class DetailsKycResidentialInfo
    {
        public int residentialInformationId { get; set; }
        public int onboardingId { get; set; }
        public int residentialDistrictId { get; set; }
        public int residentialAreaId { get; set; }
        public int residentialPinCode { get; set; }
        public string residentialLandmark { get; set; }
        public string residentialAddress { get; set; }
        public string submittedOnDate { get; set; }
        public string submittedOnTime { get; set; }
        public int submittedBy { get; set; }
        public int kycStatus { get; set; }
        public string approvedOnDate { get; set; }
        public string approvedOnTime { get; set; }
        public int approvedBy { get; set; }
        public int remarks { get; set; }
        public string residentialInfo1 { get; set; }
        public string residentialInfo2 { get; set; }
        public string area { get; set; }
        public string districtName { get; set; }
        public string stateName { get; set; }
        public string countryName { get; set; }
        public string customerKYCStatus { get; set; }
        public string remarksDescription { get; set; }
    }

    public class DetailsKycBankInfo
    {
        public int bankInformationId { get; set; }
        public int onboardingId { get; set; }
        public int bankAccountTypeId { get; set; }
        public int bankId { get; set; }
        public string accountName { get; set; }
        public string accountNumber { get; set; }
        public string ifscCode { get; set; }
        public string imagePath { get; set; }
        public string submittedOnDate { get; set; }
        public string submittedOnTime { get; set; }
        public int submittedBy { get; set; }
        public int kycStatus { get; set; }
        public string approvedOnDate { get; set; }
        public string approvedOnTime { get; set; }
        public int approvedBy { get; set; }
        public int remarks { get; set; }
        public string bankInfo1 { get; set; }
        public string bankAccountTypeName { get; set; }
        public string bankName { get; set; }
        public string customerKYCStatus { get; set; }
        public string remarksDescription { get; set; }
    }
    public class OnBoardingCustomer
    {
        public List<DetailsCustomerOnboarding> detailsCustomerOnboarding { get; set; }
        public List<DetailsKycPanOcrInfo> detailsKycPanOcrInfo { get; set; }
        public List<DetailsKycBasicInfo> detailsKycBasicInfo { get; set; }
        public List<DetailsKycOutletInfo> detailsKycOutletInfo { get; set; }
        public List<DetailsKycResidentialInfo> detailsKycResidentialInfo { get; set; }
        public List<DetailsKycBankInfo> detailsKycBankInfo { get; set; }
    }

    #endregion
}
