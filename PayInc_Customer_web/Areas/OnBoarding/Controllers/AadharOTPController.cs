using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PayInc_Customer_web.Areas.OnBoarding.Models;
using PayInc_Customer_web.Models;
using PayInc_Customer_web.Utility;

namespace PayInc_Customer_web.Areas.OnBoarding.Controllers
{
    [Area("OnBoarding")]
    public class AadharOTPController : Controller
    {
        public IActionResult Index(string mob)
        {
            if (!string.IsNullOrEmpty(mob))
            {
                var decryptedText = DecryptData(mob, "PayINC_@12345", true);
                if (decryptedText != null)
                {
                    var listParam = new List<KeyValuePair<string, string>>();
                    listParam.Add(new KeyValuePair<string, string>("mobileNumber", decryptedText));
                    string errorMessage = string.Empty;
                    var response = new CallService().GetResponse<LoginResData>("getCustomerDetailsByMobileNo", listParam, ref errorMessage);
                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        ViewData["ErrorMessage"] = "This User is already On-Boarded.";
                    }
                    else
                    {
                        var listParam1 = new List<KeyValuePair<string, string>>();
                        listParam1.Add(new KeyValuePair<string, string>("mobileNumber", decryptedText));
                        string errorMessage1 = string.Empty;
                        var reponse1 = new CallService().GetResponse<List<OnBoardedResp>>("getDetailsCustomerOnboarding", listParam1, ref errorMessage1);

                        if (string.IsNullOrEmpty(errorMessage1))
                        {
                            var sessionUtilty = new SessionUtility();
                            sessionUtilty.SetSession("BoardingPAN", reponse1[0].PanCardNumber);
                            sessionUtilty.SetSession("BoardingFatherName", reponse1[0].FatherName);
                            sessionUtilty.SetSession("BoardingName", reponse1[0].FirstName + " "+ reponse1[0].MiddleName + " " + reponse1[0].LastName);
                            sessionUtilty.SetSession("BoardingDob", Convert.ToString(reponse1[0].Dob));
                            sessionUtilty.SetSession("BoardedMobile", Convert.ToString(reponse1[0].CustomerNumber));
                            sessionUtilty.SetSession("BoardingId", Convert.ToString(reponse1[0].OnboardingId));
                            ViewData["BoardedName"] = reponse1[0].FirstName + " " + reponse1[0].MiddleName + " " + reponse1[0].LastName;
                            ViewData["BoardedMobile"] = reponse1[0].CustomerNumber;
                        }
                        else
                        {
                            ViewData["ErrorMessage"] = "There is no KYC process found for this user.";
                        }
                    }
                }
            }
            else
            {
                //return View();
                ViewData["ErrorMessage"] = "There is no On Boarding Process. Please go through valid link.";
            }

            return View();
        }
        [HttpPost]
        public IActionResult VerifyMobileNumber()
        {
            try
            {

            }
            catch (Exception)
            {

            }
            return Json(new { success = true });
        }
        [HttpPost]
        public IActionResult UploadPancard()
        {
            try
            {
                if (Request.Form.Files != null)
                {
                    var file = Request.Form.Files[0];
                    var filePath = Path.GetTempFileName();

                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            var fileBytes = ms.ToArray();
                            string strBase64 = Convert.ToBase64String(fileBytes);
                            var ext = Path.GetExtension(file.FileName);
                            var parameter = new
                            {
                                fileCategory = "PAN",
                                fileUniqueId = "PAN_CARD_" + System.DateTime.Now.Day + System.DateTime.Now.Minute + "_" + new Random().Next(1000, 9999),
                                fileExtension = ext,
                                base64String = strBase64
                            };
                            string s = Newtonsoft.Json.JsonConvert.SerializeObject(parameter);
                            string errorMessage = string.Empty;
                            var savedImageUrl = new CallService().PostImage("saveImageBase64", parameter, ref errorMessage);
                            savedImageUrl = JsonConvert.DeserializeObject<string>(savedImageUrl);
                            if (string.IsNullOrEmpty(errorMessage))
                            {
                                var listParams = new List<KeyValuePair<string, string>>();
                                listParams.Add(new KeyValuePair<string, string>("getImages", savedImageUrl));
                                string errorPanMsg = string.Empty;
                                var panExtractResponse = new CallService().GetResponse<Models.OnBoardingResp>("getSignzyPanExtraction", listParams, ref errorPanMsg);
                                if (string.IsNullOrEmpty(errorPanMsg))
                                {
                                    if (string.IsNullOrEmpty(panExtractResponse.response.result.number))
                                    {
                                        return Json(new { success = false, errorMessage = "Invalid PAN CARD" });
                                    }
                                    var responseParam = new
                                    {
                                        name = panExtractResponse.response.result.name,
                                        fatherName = panExtractResponse.response.result.fatherName,
                                        dob = panExtractResponse.response.result.dob,
                                        panNumber = panExtractResponse.response.result.number,
                                        imageUrl = savedImageUrl
                                    };
                                    InsertCustomerKyc(1, responseParam.panNumber, null, savedImageUrl, 1, JsonConvert.SerializeObject(panExtractResponse), null, "Pan Card Response");
                                    var sessionUtilty = new SessionUtility();
                                    sessionUtilty.SetSession("PanImageURL", responseParam.imageUrl);
                                    sessionUtilty.SetSession("BoardingDob", panExtractResponse.response.result.dob);
                                    string checkMatchError = string.Empty;
                                    var fieldsInput = new
                                    {
                                        Field1 = sessionUtilty.GetStringSession("BoardingPAN"),
                                        Field2 = sessionUtilty.GetStringSession("BoardingFatherName"),
                                        FieldN = sessionUtilty.GetStringSession("BoardingName")
                                    };

                                    var nameMatchResp = GetMatchTextPercentage(responseParam.imageUrl, fieldsInput, ref checkMatchError);
                                    if (string.IsNullOrEmpty(checkMatchError))
                                    {
                                        var matchRes = new
                                        {
                                            PanNumber = nameMatchResp[0].Value,
                                            FatherName = nameMatchResp[1].Value,
                                            FullName = nameMatchResp[2].Value
                                        };

                                        return Json(new { success = true, responseData = responseParam, matchData = matchRes });
                                    }
                                    return Json(new { success = true, responseData = responseParam, matchData = "" });
                                }
                                else
                                {
                                    return Json(new { success = false, errorMessage });
                                }
                            }
                        }
                    }

                }
                return Json(new { success = false, errorMessage = "Please select pancard image" });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }
        [HttpPost]
        public IActionResult SubmitPAN(IFormCollection fc)
        {
            string mobileNumber = string.Empty;
            string redirectUrl = string.Empty;
            try
            {
                var sessionUtility = new SessionUtility();
                mobileNumber = sessionUtility.GetStringSession("BoardedMobile");
                var additionalData = new AdditionalParam
                {
                    Name = sessionUtility.GetStringSession("BoardingName"),
                    FatherName = sessionUtility.GetStringSession("BoardingFatherName"),
                    PanPhoto = sessionUtility.GetStringSession("PanImageURL"),
                    PanNumber = sessionUtility.GetStringSession("BoardingPAN"),
                    BoardingId = sessionUtility.GetStringSession("BoardingId"),
                    DateOfBirth = sessionUtility.GetStringSession("BoardingDob"),
                    MobileNumber = sessionUtility.GetStringSession("BoardedMobile")
                };
                redirectUrl = Startup.AppSetting["AadharOTP_RedirectURL"] + "?mob=" + mobileNumber+"&targetUrl="+ Startup.AppSetting["Aadhar_ReturnBackURL"]+ "&additionalParmJson=" + JsonConvert.SerializeObject(additionalData);
            }
            catch (Exception)
            {

            }
            return Json(new { success = true, redirect_url = redirectUrl });
        }
        
        [HttpPost]
        public IActionResult GetAadharDetails(IFormCollection fc)
        {
            var sessionUtility = new SessionUtility();
            try
            {
                sessionUtility.SetSession("Aadhar_panel","true");
                string status = Convert.ToString(fc["status"]);
                string additionalParams = Convert.ToString(fc["additionalparam"]);
                var additionalResp = JsonConvert.DeserializeObject<AdditionalParam>(additionalParams);
                sessionUtility.SetSession("BoardedMobile",additionalResp.MobileNumber);
                sessionUtility.SetSession("BoardingFatherName", additionalResp.FatherName);
                sessionUtility.SetSession("BoardingId", additionalResp.BoardingId);
                sessionUtility.SetSession("BoardingDob", additionalResp.DateOfBirth);
                sessionUtility.SetSession("PanImageURL", additionalResp.PanPhoto);
                GetCroppedPANPhoto();
                if (status=="SUCCESS")
                {
                    string response = Convert.ToString(fc["response"]);
                    sessionUtility.SetSession("Aadhar_Details", response);
                    var aadharDetails = JsonConvert.DeserializeObject<AadharOTPRes>(response);
                    ViewData["Status"] = status;
                    ViewData["AadharOTPRes"] = aadharDetails;
                }
                else
                {
                    ViewData["Status"] = status;
                    ViewData["AadharOTPRes"] = null;
                }
            }
            catch (Exception)
            {

            }
            return View("Index");
        }

        [HttpPost]

        public IActionResult SubmitAddressProof(IFormCollection fc)
        {
            try
            {
                var sessionUtility = new SessionUtility();
                var aadharData = sessionUtility.GetStringSession("Aadhar_Details");
                var aadharDetails = JsonConvert.DeserializeObject<AadharOTPRes>(aadharData);
                aadharDetails.original_kyc_info.mobile = string.IsNullOrEmpty(aadharDetails.original_kyc_info.mobile) ? sessionUtility.GetStringSession("BoardedMobile") : aadharDetails.original_kyc_info.mobile;
                var getValue = GetAllValues();
                InsertResidentialInfo();
                return Json(new { success = true, responseData = aadharDetails.original_kyc_info });
            }
            catch (Exception)
            {

            }
            return Json(new { success = false });
        }
        [HttpPost]
        public IActionResult BasicDetails(IFormCollection fc)
        {
            try
            {
                var fc1 = fc["personalDetails"];
                var dict = HttpUtility.ParseQueryString(fc1);
                string json = JsonConvert.SerializeObject(dict.Cast<string>().ToDictionary(k => k, v => dict[v]));
                var personalDetails = JsonConvert.DeserializeObject<PersonalDetailsInput>(json);

                var fc2 = fc["addressDetails"];
                var dict1 = HttpUtility.ParseQueryString(fc2);
                string json1 = JsonConvert.SerializeObject(dict1.Cast<string>().ToDictionary(k => k, v => dict1[v]));
                var addressDetails = JsonConvert.DeserializeObject<AddressInput>(json1);
                var allValues = new AllBasicDetailsInput();
                allValues.personalDetails = personalDetails;
                allValues.addressDetails = addressDetails;
                new SessionUtility().SetSession("AllBasicDetails", JsonConvert.SerializeObject(allValues));
            }
            catch (Exception)
            {

            }
            return Json(new { success = true });
        }
        [HttpPost]
        public IActionResult BankDetails(IFormCollection fc)
        {
            try
            {
                var bankName = Convert.ToString(fc["ddlBank"]);
                var bankAccount = Convert.ToString(fc["bankaccount"]);
                string bankIFSCCode = Convert.ToString(fc["bankifsccode"]);
                var sessionUtility = new SessionUtility();
                var allBasicDetails = JsonConvert.DeserializeObject<AllBasicDetailsInput>(sessionUtility.GetStringSession("AllBasicDetails"));
                var mainDetails = JsonConvert.DeserializeObject<SetAllValue>(sessionUtility.GetStringSession("MainDetails"));
                mainDetails.BankName =  bankName;
                mainDetails.BankAccount = bankAccount;
                mainDetails.BankIFSCCode = bankIFSCCode;
                sessionUtility.SetSession("MainDetails", JsonConvert.SerializeObject(mainDetails));
                mainDetails.BankName = bankName.Split("~")[1];
                var overviewDtls = new OverviewDetails();
                allBasicDetails.personalDetails.Gender = allBasicDetails.personalDetails.Gender.Split("~")[1];
                allBasicDetails.personalDetails.MaritialStatus = allBasicDetails.personalDetails.MaritialStatus.Split("~")[1];
                overviewDtls.allDetails = allBasicDetails;
                overviewDtls.allValue = mainDetails;
                return PartialView("AllDetails", overviewDtls);
            }
            catch (Exception)
            {

            }
            return PartialView("AllDetails");
        }

        [HttpPost]
        public IActionResult GetBankList()
        {
            string errorMessage = string.Empty;
            try
            {
                var listParams = new List<KeyValuePair<string, string>>();
                listParams.Add(new KeyValuePair<string, string>("bankId", "0"));
                var response = new CallService().GetResponse<List<BankResponse>>("getMasterBanks", listParams, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return Json(response);
                }

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return Json(null);
        }

        [HttpPost]
        public IActionResult UploadCancelledCheque()
        {
            return Json(null);
        }

        public string DecryptData(string message, string accessKey, bool isHash = true)
        {
            string decryptDataMessage = null;
            try
            {
                byte[] accessKeyArray;
                byte[] toDecryptArray = Convert.FromBase64String(message);
                if (isHash)
                {
                    using (MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider())
                    {
                        accessKeyArray = mD5CryptoServiceProvider.ComputeHash(UTF8Encoding.UTF8.GetBytes(accessKey));
                        mD5CryptoServiceProvider.Clear();
                    }
                }
                else { accessKeyArray = UTF8Encoding.UTF8.GetBytes(accessKey); }

                using (TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider())
                {
                    tripleDESCryptoServiceProvider.Key = accessKeyArray;
                    tripleDESCryptoServiceProvider.Mode = CipherMode.ECB;
                    tripleDESCryptoServiceProvider.Padding = PaddingMode.PKCS7;

                    ICryptoTransform iCryptoTransform = tripleDESCryptoServiceProvider.CreateDecryptor();
                    byte[] resultDecryptArray = iCryptoTransform.TransformFinalBlock(toDecryptArray, 0, toDecryptArray.Length);
                    tripleDESCryptoServiceProvider.Clear();

                    decryptDataMessage = UTF8Encoding.UTF8.GetString(resultDecryptArray);
                }
            }
            catch { }
            return decryptDataMessage;
        }

        public List<KeyValuePair<string, string>> GetMatchTextPercentage(string imagesUrl, Object fieldInput, ref string errorMessage)
        {
            try
            {
                var listImage = new List<string>();
                var listKeyValuePair = new List<KeyValuePair<string, string>>();
                listImage.Add(imagesUrl);
                string errorMsg = string.Empty;
                var req = new
                {
                    essentials = new
                    {
                        images = listImage,
                        fields = fieldInput
                    }
                };
                var nameMatchResp = new CallService().PostResponse<TextMatchRes>("getSignzyTextMatch", req, ref errorMsg);
                if (string.IsNullOrEmpty(errorMsg))
                {
                    string totalPercent = Convert.ToString((Convert.ToDecimal(nameMatchResp.result.field1.score) + Convert.ToDecimal(nameMatchResp.result.field2.score) + Convert.ToDecimal(nameMatchResp.result.fieldN.score)) * 100);
                    InsertCustomerKyc(2, null, null, null, 1, JsonConvert.SerializeObject(nameMatchResp), totalPercent, "Pan Card Matching Response");
                    listKeyValuePair.Add(new KeyValuePair<string, string>("PANNumber", Convert.ToString(Convert.ToDecimal(nameMatchResp.result.field1.score) * 100)));
                    listKeyValuePair.Add(new KeyValuePair<string, string>("FatherName", Convert.ToString(Convert.ToDecimal(nameMatchResp.result.field2.score) * 100)));
                    listKeyValuePair.Add(new KeyValuePair<string, string>("Name", Convert.ToString(Convert.ToDecimal(nameMatchResp.result.fieldN.score) * 100)));
                    return listKeyValuePair;
                }
                else
                {
                    errorMessage = errorMsg;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return null;
        }


        public NameMatchResult GetNameMatchDetails(string name1, string name2, ref string errorMessage)
        {
            try
            {
                string errorMsg = string.Empty;
                var req = new
                {
                    task = "nameMatch",
                    essentials = new
                    {
                        nameBlock = new
                        {
                            name1,
                            name2
                        }
                    }
                };
                var nameMatchResp = new CallService().PostResponse<NameMatchResponse>("getSignzyNameMatch", req, ref errorMsg);
                if (string.IsNullOrEmpty(errorMsg))
                {
                    InsertCustomerKyc(7, null, null, null, 1, JsonConvert.SerializeObject(nameMatchResp), null, "PAN CARD DL Name Matching Response");
                    return nameMatchResp.result;
                }
                else
                {
                    errorMessage = errorMsg;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return null;
        }


        public FaceMatchResult GetSignzyFaceMatch(string firstImage, string secondImage, ref string errorMessage)
        {
            try
            {
                var req = new
                {
                    essentials = new
                    {
                        firstImage = firstImage,
                        secondImage = secondImage,
                        threshold = "0.5",
                        alignHorizontally = "yes"
                    }
                };
                var response = new CallService().PostResponse<FaceMatchResp>("getsignzyFaceMatch", req, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    InsertCustomerKyc(9, null, null, firstImage, 1, JsonConvert.SerializeObject(response), null, "PAN CARD DL Face Matching Response");
                    return response.result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return null;
        }

        public void GetCroppedPANPhoto()
        {
            try
            {
                var listParams = new List<KeyValuePair<string, string>>();
                string errorMessage = string.Empty;
                listParams.Add(new KeyValuePair<string, string>("getImages", new SessionUtility().GetStringSession("PanImageURL")));
                var response = new CallService().GetResponse<FacePanRes>("getSignzyPanFaceExtraction", listParams, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    new SessionUtility().SetSession("PANCroppedUrl", response.response.result.cropped);
                    InsertCustomerKyc(14, "Selfi", "", response.response.result.cropped, 1, null, null, null);
                }
            }
            catch (Exception)
            {

            }
        }
        public SetAllValue GetAllValues()
        {
            try
            {
                var sessionUtility = new SessionUtility();
                var setValues = new SetAllValue();
                setValues.FullName = sessionUtility.GetStringSession("BoardingName");
                setValues.FatherName = sessionUtility.GetStringSession("BoardingFatherName");
                setValues.DateOfBirth = sessionUtility.GetStringSession("BoardingDob");
                setValues.MobileNumber = sessionUtility.GetStringSession("BoardedMobile");
                var aadharData = sessionUtility.GetStringSession("Aadhar_Details");
                var aadharDetails = JsonConvert.DeserializeObject<AadharOTPRes>(aadharData);
                setValues.FullName = aadharDetails.original_kyc_info.name;
                setValues.Address = aadharDetails.original_kyc_info.address;
                setValues.City = aadharDetails.original_kyc_info.dist;
                setValues.State = aadharDetails.original_kyc_info.state;
                new SessionUtility().SetSession("MainDetails", JsonConvert.SerializeObject(setValues));
                return setValues;
            }
            catch (Exception)
            {

            }
            return null;
        }
        public void InsertCustomerKyc(int kycProofId, string kycProofValue1, string kycProofValue2, string kycProofImagePath, int kycStatus, string responseObject, string matchingCriteria, string kycRemarks)
        {
            try
            {
                var req = new
                {
                    kycId = 0,
                    OnboardingId = Convert.ToInt32(new SessionUtility().GetStringSession("BoardingId")),
                    kycProofId = kycProofId,
                    kycProofValue1 = kycProofValue1,
                    kycProofValue2 = kycProofValue2,
                    kycProofImagePath = kycProofImagePath,
                    submittedBy = 1,
                    kycStatus = kycStatus,
                    responseObject = responseObject,
                    matchingCriteria = matchingCriteria,
                    kycRemarks = kycRemarks,
                    ext1 = ""
                };
                string errorMessage = string.Empty;
                var response = new CallService().PostResponse<string>("putDetailsCustomerKYC", req, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {

                }
            }
            catch (Exception)
            {

            }
        }
        [HttpPost]
        public IActionResult VideoVerification()
        {
            try
            {
                var req = new
                {
                    task = "url",
                    essentials = new
                    {
                        matchImage = new List<string> { new SessionUtility().GetStringSession("PANCroppedUrl") },
                        customVideoRecordTime = "6",
                        hideTopLogo = "false",
                        hideBottomLogo = "false",
                        callbackUrl = Startup.AppSetting["AadharVideoVerifyUrl"],
                        redirectUrl = Startup.AppSetting["AadharVideoVerifyUrl"],
                        idCardVerification = "false",
                        customizations = new { }
                    }
                };
                string errorMessage = string.Empty;
                var response = new CallService().PostResponse<VideoVerificationResponse>("getSignzyVideoVerification", req, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    if (response.result != null)
                    {
                        new SessionUtility().SetSession("VideoToken", response.result.token);
                    }
                    return Json(new { success = true, responseData = response });
                }
                return Json(new { success = false, responseData = "" });
            }
            catch (Exception)
            {

            }
            return Json(new { success = false });
        }

        [HttpGet]
        public IActionResult GetVideoVerification()
        {
            try
            {                
                var sessionUtility = new SessionUtility();
                string tokenId = sessionUtility.GetStringSession("VideoToken");
                var listParam = new List<KeyValuePair<string, string>>();
                listParam.Add(new KeyValuePair<string, string>("task", tokenId));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<FinalVideoVerifyResp>("getSignzyGetVideoVerification", listParam, ref errorMessage);
                response = new CallService().GetResponse<FinalVideoVerifyResp>("getSignzyGetVideoVerification", listParam, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    response.PanImage = new SessionUtility().GetStringSession("PANCroppedUrl");
                    var allBasicDetails = JsonConvert.DeserializeObject<AllBasicDetailsInput>(sessionUtility.GetStringSession("AllBasicDetails"));
                    var mainDetails = JsonConvert.DeserializeObject<SetAllValue>(sessionUtility.GetStringSession("MainDetails"));
                    var panDOB = new DateTime();
                    int dob = 0;
                    if (DateTime.TryParse(mainDetails.DateOfBirth, out panDOB))
                    {
                        dob = Convert.ToInt32(Convert.ToDateTime(mainDetails.DateOfBirth).ToString("yyyyMMdd"));
                    }
                    if (sessionUtility.GetStringSession("isInserted") == null)
                    {
                        var areaOutletDetails = GetAddressByPinCode(allBasicDetails.basicInput.firmPinCode);
                        var req = new
                        {
                            onboardingId = Convert.ToInt64(sessionUtility.GetStringSession("BoardingId")),
                            dob = dob,
                            emailId = allBasicDetails.personalDetails.EmailAddress,
                            genderId = Convert.ToInt32(allBasicDetails.personalDetails.Gender.Split('~')[0]),
                            maritalStatusId = Convert.ToInt32(allBasicDetails.personalDetails.MaritialStatus.Split('~')[0]),
                            casteCategory = allBasicDetails.personalDetails.Caste,
                            qualification = allBasicDetails.personalDetails.EducationalQualification,
                            serviceProviderId = 1,
                            physicalStatus = allBasicDetails.personalDetails.IsPhysicallyDisabled,
                            occupationType = allBasicDetails.personalDetails.OccupationType,
                            occupationDuration = "",
                            entityType = allBasicDetails.personalDetails.EntityType,
                            districtId = GetAddressByPinCode(allBasicDetails.addressDetails.pincode).districtId,
                            areaId =Convert.ToInt32(allBasicDetails.addressDetails.areaId),
                            pinCode = Convert.ToInt32(allBasicDetails.addressDetails.pincode),
                            landmark = allBasicDetails.addressDetails.landmark,
                            address = allBasicDetails.addressDetails.address,
                            outletName = allBasicDetails.basicInput.firmname,
                            outletCategoryId = 1,
                            outletDistrictId = areaOutletDetails.districtId,
                            outletAreaId = areaOutletDetails.areaId,
                            outletPinCode = Convert.ToInt32(allBasicDetails.basicInput.firmPinCode),
                            outletLandmark = allBasicDetails.basicInput.firmLandmark,
                            outletAddress = allBasicDetails.basicInput.firmaddress,
                            bankAccountTypeId = 1,
                            bankId = Convert.ToInt32(mainDetails.BankName.Split('~')[0]),
                            accountName = mainDetails.BankName,
                            accountNumber = mainDetails.BankAccount,
                            ifscCode = mainDetails.BankIFSCCode,
                            customerKycStatusId = 2,
                            updateBy = 0
                        };
                        string errorMessage1 = string.Empty;
                        var insertKyc = new CallService().PostResponse<string>("putDetailsCustomerOnbBoardingKyc", req, ref errorMessage1);
                        if (string.IsNullOrEmpty(errorMessage1))
                        {
                            InsertCustomerKyc(10, null, null, null, 1, JsonConvert.SerializeObject(response), null, "Video Verifyication");
                        }
                        sessionUtility.SetSession("isInserted", "true");
                    }

                    return View("Response", response);
                }
                return View("Response");
            }
            catch (Exception)
            {

            }
            return View("Response");
        }

        public AreaByPinCodeRes GetAddressByPinCode(string PinCode)
        {
            var response =new List<AreaByPinCodeRes>();
            try
            {
                if (PinCode.Length == 6)
                {
                    var listParam = new List<KeyValuePair<string, string>>();
                    listParam.Add(new KeyValuePair<string, string>("pinCode", PinCode));
                    string errorMessage = string.Empty;
                    response = new CallService().GetResponse<List<AreaByPinCodeRes>>("getMasterAreasByPinCode", listParam, ref errorMessage);
                    return response[0];
                }
                else
                {
                    response.Add(new AreaByPinCodeRes { areaId = 0 });
                    return response[0];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                response.Add(new AreaByPinCodeRes { areaId = 0 });
                return response[0];
            }
        }

        #region INSERT METHOD BASIC DETAILS
        public bool InsertBasicInfo()
        {
            try
            {
                var sessionUtility = new SessionUtility();
                var allBasicDetails = JsonConvert.DeserializeObject<AllBasicDetailsInput>(sessionUtility.GetStringSession("AllBasicDetails"));
                var req = new
                {
                    onboardingId = Convert.ToInt64(sessionUtility.GetStringSession("BoardingId")),
                    emailId = allBasicDetails.personalDetails.EmailAddress,
                    genderId = Convert.ToInt32(allBasicDetails.personalDetails.Gender.Split('~')[0]),
                    maritalStatusId = Convert.ToInt32(allBasicDetails.personalDetails.MaritialStatus.Split('~')[0]),
                    casteCategory = Convert.ToString(allBasicDetails.personalDetails.Caste),
                    serviceProviderId = Convert.ToInt32(allBasicDetails.personalDetails.MobileOperator.Split('~')[0]),
                    physicalStatus = allBasicDetails.personalDetails.IsPhysicallyDisabled,
                    occupationType = allBasicDetails.personalDetails.OccupationType,
                    entityType = allBasicDetails.personalDetails.EntityType,
                    alternateNumber = "0",
                    submittedBy = 0
                };
                string errorMessage = string.Empty;
                var response = new CallService().PostResponse<string>("putDetailsKycBasicInfoReq", req, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return true;
                }
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool InsertResidentialInfo()
        {
            try
            {
                var sessionUtility = new SessionUtility();
                var aadharData = sessionUtility.GetStringSession("Aadhar_Details");
                var aadharDetails = JsonConvert.DeserializeObject<AadharOTPRes>(aadharData);
                var residential = GetAddressByPinCode(aadharDetails.original_kyc_info.pc);
                var req = new
                {
                    onboardingId = Convert.ToInt64(sessionUtility.GetStringSession("BoardingId")),
                    residentialDistrictId = residential.districtId,
                    residentialAreaId = Convert.ToInt32(residential.areaId),
                    residentialPinCode = Convert.ToInt32(aadharDetails.original_kyc_info.pc),
                    residentialLandmark = aadharDetails.original_kyc_info.landmark,
                    residentialAddress = aadharDetails.original_kyc_info.address,
                    submittedBy = 0,
                };
                string errorMessage = string.Empty;
                var response = new CallService().PostResponse<string>("putDetailsKycResidentialInfo", req, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return true;
                }
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool InsertKycOutletInfo()
        {
            try
            {
                var sessionUtility = new SessionUtility();
                var allBasicDetails = JsonConvert.DeserializeObject<AllBasicDetailsInput>(sessionUtility.GetStringSession("AllBasicDetails"));
                var pinAddress = GetAddressByPinCode(allBasicDetails.basicInput.firmPinCode);
                var req = new
                {
                    onboardingId = Convert.ToInt64(sessionUtility.GetStringSession("BoardingId")),
                    outletName = allBasicDetails.basicInput.firmname,
                    outletCategoryId = 1,
                    outletDistrictId = Convert.ToInt32(pinAddress.districtId),
                    outletAreaId = Convert.ToInt32(allBasicDetails.basicInput.firmAreaId),
                    outletPinCode = Convert.ToInt32(allBasicDetails.basicInput.firmPinCode),
                    outletLandmark = allBasicDetails.basicInput.firmLandmark,
                    outletAddress = allBasicDetails.basicInput.firmaddress,
                    occupationDuration = "1",
                    outletLatitude = 0,
                    outletLongitude = 0,
                    submittedBy = 0,
                };
                string errorMessage = string.Empty;
                var response = new CallService().PostResponse<string>("putDetailsKycOutletInfo", req, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return true;
                }
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool InsertPANInfo()
        {
            try
            {
                var sessionUtility = new SessionUtility();
                string panDOB = sessionUtility.GetStringSession("BoardingDob");
                var dateTime = new DateTime();
                int dob = 0;
                if (DateTime.TryParse(panDOB, out dateTime))
                {
                    dob = Convert.ToInt32(Convert.ToDateTime(panDOB).ToString("yyyyMMdd"));
                }
                else
                {
                    if (DateTime.TryParseExact(panDOB, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                    {
                        DateTime dt = DateTime.ParseExact(panDOB, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                        dob = Convert.ToInt32(dt.ToString("yyyyMMdd"));
                    }
                    else if (DateTime.TryParseExact(panDOB, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                    {
                        DateTime dt = DateTime.ParseExact(panDOB, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        dob = Convert.ToInt32(dt.ToString("yyyyMMdd"));
                    }
                }
                var req = new
                {
                    onboardingId = Convert.ToInt64(sessionUtility.GetStringSession("BoardingId")),
                    nameonPancard = sessionUtility.GetStringSession("BoardingName"),
                    fatherName = sessionUtility.GetStringSession("BoardingFatherName"),
                    dob = dob,
                    imagePath = sessionUtility.GetStringSession("PanImageURL"),
                    submittedBy = 0
                };
                string errorMessage = string.Empty;
                var response = new CallService().PostResponse<string>("putDetailsKycPanOcrInfo", req, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return true;
                }
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool InsertBankInfo()
        {
            try
            {
                var sessionUtility = new SessionUtility();
                var mainDetails = JsonConvert.DeserializeObject<SetAllValue>(sessionUtility.GetStringSession("MainDetails"));
                var req = new
                {
                    onboardingId = Convert.ToInt64(sessionUtility.GetStringSession("BoardingId")),
                    bankAccountTypeId = 1,
                    bankId = Convert.ToInt32(mainDetails.BankName.Split("~")[0]),
                    accountName = mainDetails.AccountHolderName,
                    accountNumber = mainDetails.BankAccount,
                    ifscCode = mainDetails.BankIFSCCode,
                    imagePath = "NA",
                    submittedBy = 0
                };
                string errorMessage = string.Empty;
                var response = new CallService().PostResponse<string>("putDetailsKycBankInfo", req, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return true;
                }
            }
            catch (Exception)
            {

            }
            return false;
        }

        #endregion

        #region UPDATE METHOD BASIC DETAILS
        public bool UpdateBasicInfo(int basicInfoId)
        {
            try
            {
                var sessionUtility = new SessionUtility();
                var allBasicDetails = JsonConvert.DeserializeObject<AllBasicDetailsInput>(sessionUtility.GetStringSession("AllBasicDetails"));
                var req = new
                {
                    basicInformationId = basicInfoId,
                    onboardingId = Convert.ToInt64(sessionUtility.GetStringSession("BoardingId")),
                    emailId = allBasicDetails.personalDetails.EmailAddress,
                    genderId = Convert.ToInt32(allBasicDetails.personalDetails.Gender.Split('~')[0]),
                    maritalStatusId = Convert.ToInt32(allBasicDetails.personalDetails.MaritialStatus.Split('~')[0]),
                    casteCategory = Convert.ToString(allBasicDetails.personalDetails.Caste),
                    serviceProviderId = Convert.ToInt32(allBasicDetails.personalDetails.MobileOperator.Split('~')[0]),
                    physicalStatus = allBasicDetails.personalDetails.IsPhysicallyDisabled,
                    occupationType = allBasicDetails.personalDetails.OccupationType,
                    entityType = allBasicDetails.personalDetails.EntityType,
                    alternateNumber = "0",
                    kycStatus = 1,
                    approvedBy = 0,
                    remarks = 1,
                    basicInfo1 = "",
                    basicInfo2 = ""
                };
                string errorMessage = string.Empty;
                var response = new CallService().PostResponse<string>("updateDetailsKycBasicInfo", req, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return true;
                }
            }
            catch (Exception)
            {

            }
            return false;
        }
        public bool UpdateResidentInfo(int residentialInfoId)
        {
            try
            {
                var sessionUtility = new SessionUtility();
                var allBasicDetails = JsonConvert.DeserializeObject<AllBasicDetailsInput>(sessionUtility.GetStringSession("AllBasicDetails"));
                var residential = GetAddressByPinCode(allBasicDetails.addressDetails.pincode);
                var req = new
                {
                    residentialInformationId = residentialInfoId,
                    onboardingId = Convert.ToInt64(sessionUtility.GetStringSession("BoardingId")),
                    residentialDistrictId = residential.districtId,
                    residentialAreaId = Convert.ToInt32(allBasicDetails.addressDetails.areaId),
                    residentialPinCode = Convert.ToInt32(allBasicDetails.addressDetails.pincode),
                    residentialLandmark = allBasicDetails.addressDetails.landmark,
                    residentialAddress = allBasicDetails.addressDetails.address,
                    kycStatus = 1,
                    approvedBy = 0,
                    remarks = 1,
                    residentialInfo1 = "0",
                    residentialInfo2 = "0"
                };
                string errorMessage = string.Empty;
                var response = new CallService().PostResponse<string>("updateDetailsKycResidentialInfo", req, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return true;
                }
            }
            catch (Exception)
            {

            }
            return false;
        }
        public bool UpdateOutletInfo(int outletInformationId)
        {
            try
            {
                var sessionUtility = new SessionUtility();
                var allBasicDetails = JsonConvert.DeserializeObject<AllBasicDetailsInput>(sessionUtility.GetStringSession("AllBasicDetails"));
                var pinAddress = GetAddressByPinCode(allBasicDetails.basicInput.firmPinCode);
                var req = new
                {
                    outletInformationId = outletInformationId,
                    onboardingId = Convert.ToInt64(sessionUtility.GetStringSession("BoardingId")),
                    outletName = allBasicDetails.basicInput.firmname,
                    outletCategoryId = 1,
                    outletDistrictId = Convert.ToInt32(pinAddress.districtId),
                    outletAreaId = Convert.ToInt32(allBasicDetails.basicInput.firmAreaId),
                    outletPinCode = Convert.ToInt32(allBasicDetails.basicInput.firmPinCode),
                    outletLandmark = allBasicDetails.basicInput.firmLandmark,
                    outletAddress = allBasicDetails.basicInput.firmaddress,
                    occupationDuration = "1",
                    outletLatitude = 0,
                    outletLongitude = 0,
                    kycStatus = 1,
                    approvedBy = 0,
                    remarks = 1,
                    outletInfo1 = "",
                    OutletInfo2 = ""
                };

                string errorMessage = string.Empty;
                var response = new CallService().PostResponse<string>("updateDetailsKycOutletInfo", req, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return true;
                }
            }
            catch (Exception)
            {

            }
            return false;
        }
        public bool UpdatePANInfo()
        {
            try
            {
                var sessionUtility = new SessionUtility();
                string panDOB = sessionUtility.GetStringSession("BoardingDob");
                var dateTime = new DateTime();
                int dob = 0;
                if (DateTime.TryParse(panDOB, out dateTime))
                {
                    dob = Convert.ToInt32(Convert.ToDateTime(panDOB).ToString("yyyyMMdd"));
                }
                else
                {
                    if (DateTime.TryParseExact(panDOB, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                    {
                        DateTime dt = DateTime.ParseExact(panDOB, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                        dob = Convert.ToInt32(dt.ToString("yyyyMMdd"));
                    }

                }
                var req = new
                {
                    onboardingId = Convert.ToInt64(sessionUtility.GetStringSession("BoardingId")),
                    nameonPancard = sessionUtility.GetStringSession("BoardingName"),
                    fatherName = sessionUtility.GetStringSession("BoardingFatherName"),
                    dob = dob,
                    imagePath = sessionUtility.GetStringSession("PanImageURL"),
                    submittedBy = 0
                };
                string errorMessage = string.Empty;
                var response = new CallService().PostResponse<string>("updateDetailsKycBasicInfo", req, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return true;
                }
            }
            catch (Exception)
            {

            }
            return false;
        }
        public bool UpdateBankInfo(int bankInfoId)
        {
            try
            {
                var sessionUtility = new SessionUtility();
                var allBasicDetails = JsonConvert.DeserializeObject<AllBasicDetailsInput>(sessionUtility.GetStringSession("AllBasicDetails"));
                var mainDetails = JsonConvert.DeserializeObject<SetAllValue>(sessionUtility.GetStringSession("MainDetails"));

                var req = new
                {
                    bankInformationId = bankInfoId,
                    onboardingId = Convert.ToInt64(sessionUtility.GetStringSession("BoardingId")),
                    bankAccountTypeId = 1,
                    bankId = Convert.ToInt32(mainDetails.BankName.Split('~')[0]),
                    accountName = mainDetails.AccountHolderName,
                    accountNumber = mainDetails.BankAccount,
                    ifscCode = mainDetails.BankIFSCCode,
                    imagePath = "",
                    kycStatus = 1,
                    approvedBy = 0,
                    remarks = 1
                };
                string errorMessage = string.Empty;
                var response = new CallService().PostResponse<string>("updateDetailsKycBankInfo", req, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return true;
                }
            }
            catch (Exception)
            {

            }
            return false;
        }
        #endregion
    }
}
