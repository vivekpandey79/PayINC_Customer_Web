using System;
using System.Collections.Generic;
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
                            sessionUtilty.SetSession("BoardingName", reponse1[0].FirstName + " " + reponse1[0].LastName);
                            sessionUtilty.SetSession("BoardingDob", Convert.ToString(reponse1[0].Dob));
                            sessionUtilty.SetSession("BoardedMobile", Convert.ToString(reponse1[0].CustomerNumber));
                            sessionUtilty.SetSession("BoardingId", Convert.ToString(reponse1[0].OnboardingId));
                            ViewData["BoardedName"] = reponse1[0].FirstName + " " + reponse1[0].LastName;
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
            try
            {

            }
            catch (Exception)
            {

            }
            return Json(new { success = true });
        }
        
        [HttpPost]
        public IActionResult GetAadharOTP()
        {
            try
            {
                var listParam = new List<KeyValuePair<string, string>>();
                listParam.Add(new KeyValuePair<string, string>("redirecturl", ""));
                listParam.Add(new KeyValuePair<string, string>("otp", ""));
                listParam.Add(new KeyValuePair<string, string>("mobile", ""));
                string errorMessage = string.Empty;
                var response = new CallService().GetResponse<string>("getKLinitWebViKyc", listParam, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {

                }
            }
            catch (Exception)
            {

            }
            return Json(new { success=true });
        }

        [HttpPost]

        public IActionResult SubmitAddressProof(IFormCollection fc)
        {
            try
            {
                var getValue = GetAllValues();
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
                var sessionUtility = new SessionUtility();
                var allBasicDetails = JsonConvert.DeserializeObject<AllBasicDetailsInput>(sessionUtility.GetStringSession("AllBasicDetails"));
                var mainDetails = JsonConvert.DeserializeObject<SetAllValue>(sessionUtility.GetStringSession("MainDetails"));
                var overviewDtls = new OverviewDetails();
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
                setValues.Address = sessionUtility.GetStringSession("dlAddress");
                setValues.City = sessionUtility.GetStringSession("dlCity");
                setValues.State = sessionUtility.GetStringSession("dlState");

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
        public IActionResult VideoVerification()
        {
            try
            {
                var req = new
                {
                    task = "videoVerfication",
                    essentials = new
                    {
                        matchImage = new List<string> { },
                        customVideoRecordTime = "",
                        hideTopLogo = "",
                        hideBottomLogo = "",
                        callbackUrl = "",
                        redirectUrl = "",
                        idCardVerification = "",
                        customizations = new { }
                    }
                };
                string errorMessage = string.Empty;
                var response = new CallService().PostResponse<string>("getSignzyVideoVerification", req, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {

                }
                return Json(new { success = true });
            }
            catch (Exception)
            {

            }
            return Json(new { success = true });
        }
    }
}
