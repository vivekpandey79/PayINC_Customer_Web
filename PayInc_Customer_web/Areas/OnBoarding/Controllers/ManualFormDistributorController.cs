using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PayInc_Customer_web.Areas.OnBoarding.Models;
using PayInc_Customer_web.Models;
using PayInc_Customer_web.Utility;
using System.Web;

namespace PayInc_Customer_web.Areas.OnBoarding.Controllers
{
    [Area("OnBoarding")]
    public class ManualFormDistributorController : Controller
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
                            sessionUtilty.SetSession("BoardingName", reponse1[0].FirstName + " " + reponse1[0].MiddleName + " " + reponse1[0].LastName);
                            sessionUtilty.SetSession("BoardingDob", Convert.ToString(reponse1[0].Dob));
                            sessionUtilty.SetSession("BoardedMobile", Convert.ToString(reponse1[0].CustomerNumber));
                            sessionUtilty.SetSession("BoardingId", Convert.ToString(reponse1[0].OnboardingId));
                            ViewData["BoardedName"] = reponse1[0].FirstName + " " + reponse1[0].MiddleName + " " + reponse1[0].LastName;
                            ViewData["BoardedMobile"] = reponse1[0].CustomerNumber;
                            var onBoardingData = GetCustomerOnboarding(Convert.ToString(reponse1[0].OnboardingId));
                            if (onBoardingData != null)
                            {
                                sessionUtilty.SetSession("ExistingData", JsonConvert.SerializeObject(onBoardingData));
                                if (onBoardingData.detailsKycBankInfo != null)
                                {
                                    if (onBoardingData.detailsKycBankInfo[0].customerKYCStatus != "APPROVED")
                                    {
                                        ViewData["StepNum"] = "5";
                                    }
                                    else
                                    {
                                        ViewData["StepNum"] = "6";
                                    }

                                }
                                else if (onBoardingData.detailsKycResidentialInfo != null)
                                {
                                    if (onBoardingData.detailsKycResidentialInfo[0].customerKYCStatus != "APPROVED")
                                    {
                                        ViewData["StepNum"] = "3";
                                    }
                                    else
                                    {
                                        ViewData["StepNum"] = "5";
                                    }
                                }
                                else if (onBoardingData.detailsKycPanOcrInfo != null)
                                {
                                    if (onBoardingData.detailsKycPanOcrInfo[0].customerKYCStatus != "APPROVED")
                                    {
                                        ViewData["StepNum"] = "2";
                                    }
                                    else
                                    {
                                        ViewData["StepNum"] = "3";
                                    }
                                }
                            }
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

                                    var panDOB = new DateTime();
                                    if (DateTime.TryParse(panExtractResponse.response.result.dob, out panDOB))
                                    {
                                        panExtractResponse.response.result.dob = Convert.ToDateTime(panExtractResponse.response.result.dob).ToString("dd-MM-yyyy");
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
                                            FullName = nameMatchResp[2].Value,
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
            try
            {
                var sessionUtility = new SessionUtility();
                if (sessionUtility.GetStringSession("ExistingData") != null)
                {
                    var onBoardedData = JsonConvert.DeserializeObject<OnBoardingCustomer>(sessionUtility.GetStringSession("ExistingData"));
                    if (onBoardedData.detailsKycPanOcrInfo != null)
                    {
                        sessionUtility.SetSession("PanImageURL", onBoardedData.detailsKycPanOcrInfo[0].imagePath);
                        sessionUtility.SetSession("BoardingDob", onBoardedData.detailsKycPanOcrInfo[0].dateOfBirth);

                    }
                }
                InsertPANInfo();
            }
            catch (Exception)
            {

            }
            return Json(new { success = true });
        }
        [HttpPost]
        public IActionResult UploadVoterID()
        {
            try
            {
                var frontVoterParam = new Object();
                var backVoterParam = new Object();
                var sessionUtility = new SessionUtility();
                if (Request.Form.Files != null)
                {
                    if (Request.Form.Files["file1"] != null)
                    {
                        using (var ms = new MemoryStream())
                        {
                            Request.Form.Files["file1"].CopyTo(ms);
                            var ext = Path.GetExtension(Request.Form.Files["file1"].FileName);
                            var fileBytes = ms.ToArray();
                            string strBase64 = Convert.ToBase64String(fileBytes);
                            frontVoterParam = new
                            {
                                fileCategory = "VOTERID_FRONT",
                                fileUniqueId = "VOTERID_FRONT" + System.DateTime.Now.Day + System.DateTime.Now.Minute + new Random().Next(1000, 9999),
                                fileExtension = ext,
                                base64String = strBase64
                            };
                        }
                        sessionUtility.SetSession("frontVoter", JsonConvert.SerializeObject(frontVoterParam));
                    }
                    if (Request.Form.Files["file2"] != null)
                    {
                        using (var ms = new MemoryStream())
                        {
                            Request.Form.Files["file2"].CopyTo(ms);
                            var ext = Path.GetExtension(Request.Form.Files["file2"].FileName);
                            var fileBytes = ms.ToArray();
                            string strBase64 = Convert.ToBase64String(fileBytes);
                            backVoterParam = new
                            {
                                fileCategory = "VOTERID_BACK",
                                fileUniqueId = "VOTERID_BACK" + new Random().Next(1000, 9999),
                                fileExtension = ext,
                                base64String = strBase64
                            };
                        }
                        sessionUtility.SetSession("backVoter", JsonConvert.SerializeObject(backVoterParam));
                    }
                    if (sessionUtility.GetStringSession("frontVoter") != null && sessionUtility.GetStringSession("backVoter") != null)
                    {
                        string errorMessage = string.Empty;
                        var response = new CallService().PostImage("saveImageBase64", frontVoterParam, ref errorMessage);
                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            response = JsonConvert.DeserializeObject<string>(response);
                            var listParams = new List<KeyValuePair<string, string>>();
                            listParams.Add(new KeyValuePair<string, string>("getImages", response));
                            string errorPanMsg = string.Empty;
                            var panExtractResponse = new CallService().GetResponse<string>("getsignzyVoterIdExtraction", listParams, ref errorPanMsg);
                            if (!string.IsNullOrEmpty(panExtractResponse))
                            {
                                var panDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.OnBoardingResp>(panExtractResponse);
                                var responseParam = new
                                {
                                    name = panDetails.response.result.name,
                                    fatherName = panDetails.response.result.fatherName,
                                    dob = panDetails.response.result.dob,
                                    panNumber = panDetails.response.result.number,
                                    imageUrl = "http://images.payinc.in/tpskyc/PAN/PAN_CARD_8242.jpg"
                                };
                                // return Json(new { success = true, responseData = responseParam });
                            }
                        }
                        string errorMessage1 = string.Empty;
                        var response1 = new CallService().PostImage("saveImageBase64", backVoterParam, ref errorMessage);
                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            response1 = JsonConvert.DeserializeObject<string>(response1);
                            var listParams = new List<KeyValuePair<string, string>>();
                            listParams.Add(new KeyValuePair<string, string>("getImages", response1));
                            string errorPanMsg = string.Empty;
                            var panExtractResponse = new CallService().GetResponse<string>("getsignzyVoterIdExtraction", listParams, ref errorPanMsg);
                            if (!string.IsNullOrEmpty(panExtractResponse))
                            {
                                var panDetails = JsonConvert.DeserializeObject<Models.OnBoardingResp>(panExtractResponse);
                                var panDOB = new DateTime();
                                if (DateTime.TryParseExact(panDetails.response.result.dob, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out panDOB))
                                {

                                }
                                var responseParam = new
                                {
                                    name = panDetails.response.result.name,
                                    fatherName = panDetails.response.result.fatherName,
                                    dob = panDOB,
                                    panNumber = panDetails.response.result.number,
                                    imageUrl = "http://images.payinc.in/tpskyc/PAN/PAN_CARD_8242.jpg"
                                };


                                return Json(new { success = true, responseData = responseParam });
                            }
                        }
                    }

                }
            }
            catch (Exception)
            {

            }
            return Json(new { success = true });
        }
        [HttpPost]
        public IActionResult UploadDL(IFormCollection fc)
        {
            try
            {
                var dlNumber = fc["dlnumber"];
                var dldob = fc["dldob"];
                var dlnumber1 = Request.Form["dlnumber"];

                if (Request.Form.Files != null)
                {
                    if (Request.Form.Files.Count < 1)
                    {
                        return Json(new { success = false, errorMessage = "Please select both back and front voter id" });
                    }
                    if (string.IsNullOrEmpty(dlNumber) && string.IsNullOrEmpty(dldob))
                    {
                        return Json(new { success = false, errorMessage = "Please enter DL Number and Date of Birth" });
                    }
                    if (string.IsNullOrEmpty(dldob))
                    {
                        //if (Convert.ToDateTime(dldob).ToString("dd/MM/yyyy")!=new SessionUtility().GetStringSession("BoardingDob"))
                        //{
                        //    return Json(new { success = false, errorMessage = "Please enter valid Date of Birth" });
                        //}
                        return Json(new { success = false, errorMessage = "Please enter valid Date of Birth" });
                    }
                    var dlFile = Request.Form.Files[0];
                    var filePath = Path.GetTempFileName();

                    var paramater = new Object();
                    if (dlFile.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            dlFile.CopyTo(ms);
                            var fileBytes = ms.ToArray();
                            string strBase64 = Convert.ToBase64String(fileBytes);
                            var ext = Path.GetExtension(dlFile.FileName);
                            paramater = new
                            {
                                fileCategory = "DL",
                                fileUniqueId = "DL_" + new Random().Next(1000, 9999),
                                fileExtension = ext,
                                base64String = strBase64
                            };
                        }
                    }

                    string errorMessage = string.Empty;
                    var savedImageUrl = new CallService().PostImage("saveImageBase64", paramater, ref errorMessage);
                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        savedImageUrl = JsonConvert.DeserializeObject<string>(savedImageUrl);
                        var addressDetails = new Object();
                        var faceMatchRes = new FaceMatchResult();
                        var listParams = new List<KeyValuePair<string, string>>();
                        listParams.Add(new KeyValuePair<string, string>("imagesUrl", savedImageUrl));
                        string errorMsg = string.Empty;
                        var dlExtractResponse = new CallService().GetResponse<Models.OnBoardingResp>("getSignzyDLExtraction", listParams, ref errorMsg);
                        if (string.IsNullOrEmpty(errorMsg))
                        {
                            InsertCustomerKyc(3, dlExtractResponse.response.result.number, null, savedImageUrl, 1, JsonConvert.SerializeObject(dlExtractResponse), null, "DL Response");
                            var nameResult = new NameMatchResult(); string nameTextError = string.Empty;
                            nameResult = GetNameMatchDetails(dlExtractResponse.response.result.name, new SessionUtility().GetStringSession("BoardingName"), ref nameTextError);

                            if (string.IsNullOrEmpty(nameTextError))
                            {
                                nameResult.name1 = dlExtractResponse.response.result.name;//DL Name
                                nameResult.name2 = new SessionUtility().GetStringSession("BoardingName");//PAN Name
                                if (Convert.ToDecimal(nameResult.name1_vs_name2_matchScore) >= Convert.ToDecimal(0.5))
                                {
                                    #region  Driving License Fetch By DL Number
                                    var listParamDL = new List<KeyValuePair<string, string>>();
                                    listParamDL.Add(new KeyValuePair<string, string>("drivingNumber", dlNumber));
                                    listParamDL.Add(new KeyValuePair<string, string>("dob", Convert.ToDateTime(dldob).ToString("dd/MM/yyyy").Replace('-', '/')));
                                    var dlFetchResponse = new CallService().GetResponse<Models.DLFetchResponse>("getsignzyDLNumberBasedSearch", listParamDL, ref errorMsg);
                                    if (string.IsNullOrEmpty(errorMsg))
                                    {
                                        var sessionUtility = new SessionUtility();
                                        sessionUtility.SetSession("BoardingDob", dldob);

                                        if (dlFetchResponse != null)
                                        {
                                            if (dlFetchResponse.response.result != null)
                                            {
                                                string dlState = string.Empty;
                                                if (dlFetchResponse.response.result.detailsOfDrivingLicence.splitAddress.state.Count > 0)
                                                {
                                                    if (dlFetchResponse.response.result.detailsOfDrivingLicence.splitAddress.state[0].Count > 0)
                                                    {
                                                        dlState = dlFetchResponse.response.result.detailsOfDrivingLicence.splitAddress.state[0][0];
                                                    }
                                                    else
                                                    {
                                                        dlState = "";
                                                    }

                                                }
                                                else
                                                {
                                                    dlState = "";
                                                }
                                                string pinCode = string.Empty;
                                                if (dlFetchResponse.response.result.detailsOfDrivingLicence.addressList != null)
                                                {
                                                    if (dlFetchResponse.response.result.detailsOfDrivingLicence.addressList.Count > 0)
                                                    {
                                                        if (dlFetchResponse.response.result.detailsOfDrivingLicence.addressList[0].splitAddress != null)
                                                        {
                                                            pinCode = dlFetchResponse.response.result.detailsOfDrivingLicence.addressList[0].splitAddress.pincode;
                                                        }
                                                    }
                                                }
                                                addressDetails = new
                                                {
                                                    address = dlFetchResponse.response.result.detailsOfDrivingLicence.splitAddress.addressLine == "" ? "" : dlFetchResponse.response.result.detailsOfDrivingLicence.splitAddress.addressLine,
                                                    city = dlFetchResponse.response.result.detailsOfDrivingLicence.splitAddress.city.Count > 0 ? dlFetchResponse.response.result.detailsOfDrivingLicence.splitAddress.city[0] : "",
                                                    state = dlState,
                                                    pincode = pinCode
                                                };

                                                sessionUtility.SetSession("dlAddress", dlFetchResponse.response.result.detailsOfDrivingLicence.splitAddress.addressLine == "" ? "" : dlFetchResponse.response.result.detailsOfDrivingLicence.splitAddress.addressLine);
                                                sessionUtility.SetSession("dlCity", dlFetchResponse.response.result.detailsOfDrivingLicence.splitAddress.city.Count > 0 ? dlFetchResponse.response.result.detailsOfDrivingLicence.splitAddress.city[0] : "");
                                                sessionUtility.SetSession("dlState", dlState);
                                                sessionUtility.SetSession("dlPinCode", pinCode);
                                                GetCroppedPANPhoto();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        addressDetails = new
                                        {
                                            address = "",
                                            city = "",
                                            state = "",
                                            pincode = ""
                                        };
                                        GetCroppedPANPhoto();

                                    }
                                    #endregion
                                }
                            }
                            var responseParam = new
                            {
                                name = dlExtractResponse.response.result.name,
                                dob = dlExtractResponse.response.result.dob,
                                dlNumber = dlExtractResponse.response.result.number,
                                address = addressDetails,
                                nameMatchCriteria = nameResult,
                                //faceMatchCriteria = faceMatchRes
                                faceMatchCriteria = ""
                            };
                            return Json(new { success = true, responseData = responseParam });
                        }
                    }


                }
            }
            catch (Exception)
            {
                return Json(new { success = false, errorMessage = "Something went wrong." });
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult SubmitAddressProof(IFormCollection fc)
        {
            try
            {
                string dlAddress = Convert.ToString(fc["dlAddress"]);
                string dlCity = Convert.ToString(fc["dlCity"]);
                string dlState = Convert.ToString(fc["dlState"]);
                string dlPinCode = Convert.ToString(fc["dlPinCode"]);

                var getValue = GetAllValues();
                if (string.IsNullOrEmpty(getValue.Address))
                {
                    getValue.Address = dlAddress;
                }
                if (string.IsNullOrEmpty(getValue.City))
                {
                    getValue.City = dlCity;
                }
                if (string.IsNullOrEmpty(getValue.State))
                {
                    getValue.State = dlState;
                }
                if (string.IsNullOrEmpty(getValue.PinCode))
                {
                    getValue.PinCode = dlPinCode;
                }
                return Json(new { success = true, responseData = getValue });
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
                var sessionUtility = new SessionUtility();
                if (sessionUtility.GetStringSession("ExistingData") != null)
                {
                    #region ReSubmitting form or after page refresh data
                    var onBoardedData = JsonConvert.DeserializeObject<OnBoardingCustomer>(sessionUtility.GetStringSession("ExistingData"));
                    var basicInfo = new BasicInput();
                    var addressInfo = new AddressInput();
                    var personalInfo = new PersonalDetailsInput();
                    if (onBoardedData.detailsKycOutletInfo != null)
                    {
                        basicInfo = new BasicInput()
                        {
                            firmname = onBoardedData.detailsKycOutletInfo[0].outletName,
                            firmstate = onBoardedData.detailsKycOutletInfo[0].stateName,
                            firmcity = onBoardedData.detailsKycOutletInfo[0].districtName,
                            firmdist = onBoardedData.detailsKycOutletInfo[0].outletDistrictId + "~" + onBoardedData.detailsKycOutletInfo[0].districtName,
                            firmaddress = onBoardedData.detailsKycOutletInfo[0].outletAddress,
                            firmAreaId = onBoardedData.detailsKycOutletInfo[0].outletAreaId + "~" + onBoardedData.detailsKycOutletInfo[0].area,
                            firmLandmark = onBoardedData.detailsKycOutletInfo[0].outletLandmark,
                            firmPinCode = Convert.ToString(onBoardedData.detailsKycOutletInfo[0].outletPinCode)
                        };
                    }
                    else if (onBoardedData.detailsKycResidentialInfo != null)
                    {
                        addressInfo = new AddressInput()
                        {
                            address = onBoardedData.detailsKycResidentialInfo[0].residentialAddress,
                            areaId = onBoardedData.detailsKycResidentialInfo[0].residentialAreaId + "~" + onBoardedData.detailsKycResidentialInfo[0].area,
                            city = onBoardedData.detailsKycResidentialInfo[0].districtName,
                            district = onBoardedData.detailsKycResidentialInfo[0].residentialDistrictId + "~" + onBoardedData.detailsKycResidentialInfo[0].districtName,
                            landmark = onBoardedData.detailsKycResidentialInfo[0].residentialLandmark,
                            pincode = onBoardedData.detailsKycResidentialInfo[0].residentialPinCode.ToString(),
                            state = onBoardedData.detailsKycResidentialInfo[0].stateName
                        };
                    }
                    else if (onBoardedData.detailsKycBasicInfo != null)
                    {
                        personalInfo = new PersonalDetailsInput()
                        {
                            EmailAddress = onBoardedData.detailsKycBasicInfo[0].emailId
                        };
                    }
                    var allInfo = new AllBasicDetailsInput();
                    allInfo.personalDetails = personalInfo;
                    allInfo.addressDetails = addressInfo;
                    allInfo.basicInput = basicInfo;
                    new SessionUtility().SetSession("AllBasicDetails", JsonConvert.SerializeObject(allInfo));
                    if (onBoardedData.detailsKycBankInfo != null)
                    {
                        var bankDt = new
                        {
                            bankId = onBoardedData.detailsKycBankInfo[0].bankId + "~" + onBoardedData.detailsKycBankInfo[0].bankName,
                            accountname = onBoardedData.detailsKycBankInfo[0].accountName,
                            bankaccount = onBoardedData.detailsKycBankInfo[0].accountNumber,
                            ifsccode = onBoardedData.detailsKycBankInfo[0].ifscCode
                        };
                        return Json(new { success = true, bankData = bankDt });
                    }

                    #endregion
                }
                else
                {
                    #region Form submittion 
                    var fc1 = fc["personalDetails"];
                    var dict = HttpUtility.ParseQueryString(fc1);
                    string json = JsonConvert.SerializeObject(dict.Cast<string>().ToDictionary(k => k, v => dict[v]));
                    var personalDetails = JsonConvert.DeserializeObject<PersonalDetailsInput>(json);

                    var fc2 = fc["addressDetails"];
                    var dict1 = HttpUtility.ParseQueryString(fc2);
                    string json1 = JsonConvert.SerializeObject(dict1.Cast<string>().ToDictionary(k => k, v => dict1[v]));
                    var addressDetails = JsonConvert.DeserializeObject<AddressInput>(json1);

                    var fc3 = fc["basicDetails"];//outletdetails
                    var dict2 = HttpUtility.ParseQueryString(fc3);
                    string json2 = JsonConvert.SerializeObject(dict2.Cast<string>().ToDictionary(k => k, v => dict2[v]));
                    var basicDetails = JsonConvert.DeserializeObject<BasicInput>(json2);
                    var allValues = new AllBasicDetailsInput();
                    allValues.personalDetails = personalDetails;
                    allValues.addressDetails = addressDetails;
                    allValues.basicInput = basicDetails;
                    new SessionUtility().SetSession("AllBasicDetails", JsonConvert.SerializeObject(allValues));
                    bool isInserted = InsertBasicInfo();
                    bool isOutletInserted = InsertKycOutletInfo();
                    bool isResidentialInserted = InsertResidentialInfo();
                    #endregion
                }
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
                var sessionUtility = new SessionUtility();
                if (sessionUtility.GetStringSession("ExistingData") != null)
                {
                    #region ReSubmittion
                    var onBoardedData = JsonConvert.DeserializeObject<OnBoardingCustomer>(sessionUtility.GetStringSession("ExistingData"));
                    var mainDetails = new SetAllValue();
                    mainDetails.BankName = onBoardedData.detailsKycBankInfo[0].bankId + "~" + onBoardedData.detailsKycBankInfo[0].bankName;
                    mainDetails.BankAccount = onBoardedData.detailsKycBankInfo[0].accountNumber;
                    mainDetails.BankIFSCCode = onBoardedData.detailsKycBankInfo[0].ifscCode;
                    mainDetails.AccountHolderName = onBoardedData.detailsKycBankInfo[0].accountName;
                    mainDetails.FullName = onBoardedData.detailsCustomerOnboarding[0].firstName + " " + onBoardedData.detailsCustomerOnboarding[0].lastName;
                    mainDetails.FatherName = onBoardedData.detailsCustomerOnboarding[0].fatherName;
                    mainDetails.DateOfBirth = onBoardedData.detailsCustomerOnboarding[0].dob;
                    mainDetails.MobileNumber = onBoardedData.detailsCustomerOnboarding[0].customerNumber.ToString();
                    sessionUtility.SetSession("MainDetails", JsonConvert.SerializeObject(mainDetails));
                    mainDetails.BankName = onBoardedData.detailsKycBankInfo[0].bankName;
                    var allBasicDetails = new AllBasicDetailsInput();
                    var personalDetails = new PersonalDetailsInput()
                    {
                        EmailAddress = onBoardedData.detailsKycBasicInfo[0].emailId,
                        Caste = onBoardedData.detailsKycBasicInfo[0].casteCategory,
                        EducationalQualification = "NA",
                        EntityType = onBoardedData.detailsKycBasicInfo[0].entityType,
                        IsPhysicallyDisabled = onBoardedData.detailsKycBasicInfo[0].physicalStatus,
                        MaritialStatus = onBoardedData.detailsKycBasicInfo[0].maritalStatusName,
                        MobileOperator = onBoardedData.detailsKycBasicInfo[0].serviceProviderName
                    };
                    var addressDetails = new AddressInput()
                    {
                        address = onBoardedData.detailsKycResidentialInfo[0].residentialAddress,
                        city = onBoardedData.detailsKycResidentialInfo[0].districtName,
                        district = onBoardedData.detailsKycResidentialInfo[0].districtName,
                        landmark = onBoardedData.detailsKycResidentialInfo[0].residentialLandmark,
                        pincode = Convert.ToString(onBoardedData.detailsKycResidentialInfo[0].residentialPinCode),
                        state = onBoardedData.detailsKycResidentialInfo[0].stateName,
                        areaId = onBoardedData.detailsKycResidentialInfo[0].residentialAreaId + "~" + onBoardedData.detailsKycResidentialInfo[0].area
                    };
                    var basicInput = new BasicInput()
                    {
                        firmname = onBoardedData.detailsKycOutletInfo[0].outletName,
                        firmaddress = onBoardedData.detailsKycOutletInfo[0].outletAddress,
                        firmstate = onBoardedData.detailsKycOutletInfo[0].stateName,
                        firmcity = onBoardedData.detailsKycOutletInfo[0].districtName,
                        firmdist = onBoardedData.detailsKycOutletInfo[0].outletDistrictId + "~" + onBoardedData.detailsKycOutletInfo[0].districtName,
                        firmAreaId = onBoardedData.detailsKycOutletInfo[0].outletAreaId + "~" + onBoardedData.detailsKycOutletInfo[0].area,
                        firmLandmark = onBoardedData.detailsKycOutletInfo[0].outletLandmark,
                        firmPinCode = Convert.ToString(onBoardedData.detailsKycOutletInfo[0].outletPinCode)
                    };
                    allBasicDetails.personalDetails = personalDetails;
                    allBasicDetails.addressDetails = addressDetails;
                    allBasicDetails.basicInput = basicInput;
                    sessionUtility.SetSession("AllBasicDetails", JsonConvert.SerializeObject(allBasicDetails));
                    allBasicDetails.basicInput.firmAreaId = onBoardedData.detailsKycOutletInfo[0].area;
                    allBasicDetails.basicInput.firmdist = onBoardedData.detailsKycOutletInfo[0].districtName;
                    allBasicDetails.addressDetails.areaId = onBoardedData.detailsKycResidentialInfo[0].area;
                    var overviewDtls = new OverviewDetails();
                    overviewDtls.allDetails = allBasicDetails;
                    overviewDtls.allValue = mainDetails;
                    return PartialView("AllDetails", overviewDtls);
                    #endregion
                }
                else
                {
                    #region Bank form Submission
                    var bankName = Convert.ToString(fc["ddlBank"]);
                    var accountHolderName = Convert.ToString(fc["accountname"]);
                    var bankAccount = Convert.ToString(fc["bankaccount"]);
                    string bankIFSCCode = Convert.ToString(fc["bankifsccode"]);
                    var allBasicDetails = JsonConvert.DeserializeObject<AllBasicDetailsInput>(sessionUtility.GetStringSession("AllBasicDetails"));
                    var mainDetails = JsonConvert.DeserializeObject<SetAllValue>(sessionUtility.GetStringSession("MainDetails"));
                    mainDetails.BankName = bankName;
                    mainDetails.BankAccount = bankAccount;
                    mainDetails.BankIFSCCode = bankIFSCCode;
                    mainDetails.AccountHolderName = accountHolderName;
                    sessionUtility.SetSession("MainDetails", JsonConvert.SerializeObject(mainDetails));
                    mainDetails.BankName = bankName.Split('~')[1];
                    allBasicDetails.personalDetails.Gender = allBasicDetails.personalDetails.Gender.Split("~")[1];
                    allBasicDetails.personalDetails.MaritialStatus = allBasicDetails.personalDetails.MaritialStatus.Split("~")[1];
                    var overviewDtls = new OverviewDetails();
                    overviewDtls.allDetails = allBasicDetails;
                    overviewDtls.allValue = mainDetails;
                    bool isInserted = InsertBankInfo();
                    return PartialView("AllDetails", overviewDtls);
                    #endregion
                }
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
            var sessionUtility = new SessionUtility();
            try
            {
                var listParams = new List<KeyValuePair<string, string>>();
                string errorMessage = string.Empty;
                listParams.Add(new KeyValuePair<string, string>("getImages", sessionUtility.GetStringSession("PanImageURL")));
                var response = new CallService().GetResponse<FacePanRes>("getSignzyPanFaceExtraction", listParams, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    sessionUtility.SetSession("PANCroppedUrl", response.response.result.cropped);
                }
                else
                {
                    sessionUtility.SetSession("PANCroppedUrl", sessionUtility.GetStringSession("PanImageURL"));
                }
            }
            catch (Exception)
            {
                sessionUtility.SetSession("PANCroppedUrl", sessionUtility.GetStringSession("PanImageURL"));
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
                setValues.Address = sessionUtility.GetStringSession("dlAddress");
                setValues.City = sessionUtility.GetStringSession("dlCity");
                setValues.State = sessionUtility.GetStringSession("dlState");
                setValues.PinCode = sessionUtility.GetStringSession("dlPinCode");

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
                var sessionUtility = new SessionUtility();
                if (sessionUtility.GetStringSession("ExistingData") != null)
                {
                    var onBoardedData = JsonConvert.DeserializeObject<OnBoardingCustomer>(sessionUtility.GetStringSession("ExistingData"));
                    if (onBoardedData.detailsKycPanOcrInfo != null)
                    {
                        sessionUtility.SetSession("PANCroppedUrl", onBoardedData.detailsKycPanOcrInfo[0].imagePath);
                    }
                }
                var req = new
                {
                    task = "url",
                    essentials = new
                    {
                        matchImage = new List<string> { sessionUtility.GetStringSession("PANCroppedUrl") },
                        customVideoRecordTime = "6",
                        hideTopLogo = "false",
                        hideBottomLogo = "false",
                        callbackUrl = Startup.AppSetting["ManualVideoVerifyUrl"],
                        redirectUrl = Startup.AppSetting["ManualVideoVerifyUrl"],
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
                System.Threading.Thread.Sleep(1000);
                var sessionUtility = new SessionUtility();
                string tokenId = sessionUtility.GetStringSession("VideoToken");
                var listParam = new List<KeyValuePair<string, string>>();
                listParam.Add(new KeyValuePair<string, string>("task", tokenId));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<FinalVideoVerifyResp>("getSignzyGetVideoVerification", listParam, ref errorMessage);
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
                    else
                    {
                        if (DateTime.TryParseExact(mainDetails.DateOfBirth, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out panDOB))
                        {
                            DateTime dt = DateTime.ParseExact(mainDetails.DateOfBirth, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                            dob = Convert.ToInt32(dt.ToString("yyyyMMdd"));
                        }

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
                            areaId = Convert.ToInt32(allBasicDetails.addressDetails.areaId),
                            pinCode = Convert.ToInt32(allBasicDetails.addressDetails.pincode),
                            landmark = allBasicDetails.addressDetails.landmark,
                            address = allBasicDetails.addressDetails.address,
                            outletName = allBasicDetails.basicInput.firmname,
                            outletCategoryId = 1,
                            outletDistrictId = areaOutletDetails.districtId,
                            outletAreaId = Convert.ToInt32(allBasicDetails.basicInput.firmAreaId),
                            outletPinCode = Convert.ToInt32(allBasicDetails.basicInput.firmPinCode),
                            outletLandmark = allBasicDetails.basicInput.firmLandmark,
                            outletAddress = allBasicDetails.basicInput.firmaddress,
                            bankAccountTypeId = 1,
                            bankId = Convert.ToInt32(mainDetails.BankName.Split("~")[0]),
                            accountName = mainDetails.AccountHolderName,
                            accountNumber = mainDetails.BankAccount,
                            ifscCode = mainDetails.BankIFSCCode,
                            customerKycStatusId = 2,
                            updateBy = 0
                        };
                        string errorMessage1 = string.Empty;
                        var insertKyc = new CallService().PostResponse<string>("putDetailsCustomerOnbBoardingKyc", req, ref errorMessage1);
                        if (string.IsNullOrEmpty(errorMessage1))
                        {
                            sessionUtility.RemoveSession("MainDetails");
                            sessionUtility.RemoveSession("AllBasicDetails");
                            sessionUtility.SetSession("isInserted", "true");
                            InsertCustomerKyc(10, null, null, null, 1, JsonConvert.SerializeObject(response), null, "Video Verification");
                        }

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
        [HttpPost]
        public JsonResult GetAreaPinCode(string pincode)
        {
            try
            {
                if (pincode.Length == 6)
                {
                    var listParam = new List<KeyValuePair<string, string>>();
                    listParam.Add(new KeyValuePair<string, string>("pinCode", pincode));
                    string errorMessage = string.Empty;
                    var response = new CallService().GetResponse<List<AreaByPinCodeRes>>("getMasterAreasByPinCode", listParam, ref errorMessage);
                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        return Json(new { success = true, responseData = response });
                    }
                    else
                    {
                        return Json(new { success = false });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Json(new { success = false });
        }
        public AreaByPinCodeRes GetAddressByPinCode(string PinCode)
        {
            var response = new List<AreaByPinCodeRes>();
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
                var allBasicDetails = JsonConvert.DeserializeObject<AllBasicDetailsInput>(sessionUtility.GetStringSession("AllBasicDetails"));
                var residential = GetAddressByPinCode(allBasicDetails.addressDetails.pincode);
                var req = new
                {
                    onboardingId = Convert.ToInt64(sessionUtility.GetStringSession("BoardingId")),
                    residentialDistrictId = residential.districtId,
                    residentialAreaId = Convert.ToInt32(allBasicDetails.addressDetails.areaId),
                    residentialPinCode = Convert.ToInt32(allBasicDetails.addressDetails.pincode),
                    residentialLandmark = allBasicDetails.addressDetails.landmark,
                    residentialAddress = allBasicDetails.addressDetails.address,
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
        public bool UpdateBasicInfo()
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
        public bool UpdateResidentInfo()
        {
            try
            {
                var sessionUtility = new SessionUtility();
                var allBasicDetails = JsonConvert.DeserializeObject<AllBasicDetailsInput>(sessionUtility.GetStringSession("AllBasicDetails"));
                var residential = GetAddressByPinCode(allBasicDetails.addressDetails.pincode);
                var req = new
                {
                    onboardingId = Convert.ToInt64(sessionUtility.GetStringSession("BoardingId")),
                    residentialDistrictId = residential.districtId,
                    residentialAreaId = Convert.ToInt32(allBasicDetails.addressDetails.areaId),
                    residentialPinCode = Convert.ToInt32(allBasicDetails.addressDetails.pincode),
                    residentialLandmark = allBasicDetails.addressDetails.landmark,
                    residentialAddress = allBasicDetails.addressDetails.address,
                    submittedBy = 0,
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
        public bool UpdateOutletInfo()
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
        public bool UpdateBankInfo()
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
        #endregion
        public OnBoardingCustomer GetCustomerOnboarding(string onBoardingId)
        {
            try
            {
                var listParam = new List<KeyValuePair<string, string>>();
                listParam.Add(new KeyValuePair<string, string>("onboardingId", onBoardingId));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<OnBoardingCustomer>("getCustomerOnboardingDetailsInfo", listParam, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    return response;
                }
            }
            catch (Exception)
            {

            }
            return null;
        }
    }
}
