using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
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
    public class ManualFormController : Controller
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
            return Json(new { success=true});
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
                                fileUniqueId = "PAN_CARD_" + System.DateTime.Now.Day + System.DateTime.Now.Minute +"_"+new Random().Next(1000, 9999),
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
                                        return Json(new { success = false, errorMessage="Invalid PAN CARD" });
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
                                    InsertCustomerKyc(1, responseParam.panNumber, null, savedImageUrl, 1, JsonConvert.SerializeObject(panExtractResponse),null, "Pan Card Response");
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
                    if (Request.Form.Files["file1"]!=null)
                    { 
                        using (var ms = new MemoryStream())
                        {
                            Request.Form.Files["file1"].CopyTo(ms);
                            var ext=Path.GetExtension(Request.Form.Files["file1"].FileName);
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
                        sessionUtility.SetSession("frontVoter",JsonConvert.SerializeObject(frontVoterParam));
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
                    if (sessionUtility.GetStringSession("frontVoter")!=null && sessionUtility.GetStringSession("backVoter") != null)
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
                                        if (dlFetchResponse != null)
                                        {
                                            if (dlFetchResponse.response.result != null)
                                            {
                                                
                                                addressDetails = new
                                                {
                                                    address = dlFetchResponse.response.result.detailsOfDrivingLicence.splitAddress.addressLine,
                                                    city = dlFetchResponse.response.result.detailsOfDrivingLicence.splitAddress.city[0],
                                                    state = dlFetchResponse.response.result.detailsOfDrivingLicence.splitAddress.state[0]
                                                };
                                                var sessionUtility = new SessionUtility();
                                                sessionUtility.SetSession("dlAddress", dlFetchResponse.response.result.detailsOfDrivingLicence.splitAddress.addressLine);
                                                sessionUtility.SetSession("dlCity", dlFetchResponse.response.result.detailsOfDrivingLicence.splitAddress.city[0]);
                                                sessionUtility.SetSession("dlState", dlFetchResponse.response.result.detailsOfDrivingLicence.splitAddress.state[0][0]);
                                                GetCroppedPANPhoto();
                                                //string faceErrorMsg = string.Empty;
                                                ////faceMatchRes = GetSignzyFaceMatch(dlFetchResponse.response.result.detailsOfDrivingLicence.photo, new SessionUtility().GetStringSession("PANCroppedUrl"), ref faceErrorMsg);
                                                //if (!string.IsNullOrEmpty(faceErrorMsg))
                                                //{
                                                //    faceMatchRes = null;
                                                //}
                                                //else
                                                //{
                                                //    faceMatchRes.imgURL1 = new SessionUtility().GetStringSession("PANCroppedUrl");
                                                //    faceMatchRes.imgURL2 = dlFetchResponse.response.result.detailsOfDrivingLicence.photo;
                                                //}
                                            }
                                        }
                                    }
                                    else
                                    {
                                        addressDetails = null;
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
                                faceMatchCriteria=""
                            };
                            return Json(new { success = true, responseData = responseParam });
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
        public IActionResult SubmitAddressProof(IFormCollection fc)
        {
            try
            {
               var getValue=GetAllValues();
               return Json(new { success = true,responseData=getValue });
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
                new SessionUtility().SetSession("AllBasicDetails",JsonConvert.SerializeObject(allValues));
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
                return PartialView("AllDetails",overviewDtls);
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
                listParams.Add(new KeyValuePair<string, string>("bankId","0"));
                var response = new CallService().GetResponse<List<BankResponse>>("getMasterBanks", listParams,ref errorMessage);
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

        public List<KeyValuePair<string,string>> GetMatchTextPercentage(string imagesUrl, Object fieldInput , ref string errorMessage)
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
                        images= listImage,
                        fields =fieldInput
                    }
                };
                var nameMatchResp = new CallService().PostResponse<TextMatchRes>("getSignzyTextMatch", req, ref errorMsg);
                if (string.IsNullOrEmpty(errorMsg))
                {
                    string totalPercent =Convert.ToString((Convert.ToDecimal(nameMatchResp.result.field1.score) + Convert.ToDecimal(nameMatchResp.result.field2.score) + Convert.ToDecimal(nameMatchResp.result.fieldN.score)) * 100);
                    InsertCustomerKyc(2, null, null, null, 1, JsonConvert.SerializeObject(nameMatchResp), totalPercent, "Pan Card Matching Response");
                    listKeyValuePair.Add(new KeyValuePair<string, string>("PANNumber", Convert.ToString(Convert.ToDecimal(nameMatchResp.result.field1.score)*100)));
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
                    task="nameMatch",
                    essentials = new
                    {
                        nameBlock=new {
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
                var req = new {
                    essentials = new {
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

                new SessionUtility().SetSession("MainDetails",JsonConvert.SerializeObject(setValues));
                return setValues;
            }
            catch (Exception)
            {

            }
            return null;
        }


        public void InsertCustomerKyc(int kycProofId, string kycProofValue1,string kycProofValue2, string kycProofImagePath,int kycStatus, string responseObject, string matchingCriteria, string kycRemarks)
        {
            try
            {
                var req = new {
                    kycId=0,
                    OnboardingId=Convert.ToInt32(new SessionUtility().GetStringSession("BoardingId")),
                    kycProofId= kycProofId,
                    kycProofValue1= kycProofValue1,
                    kycProofValue2= kycProofValue2,
                    kycProofImagePath= kycProofImagePath,
                    submittedBy=1,
                    kycStatus= kycStatus,
                    responseObject= responseObject,
                    matchingCriteria= matchingCriteria,
                    kycRemarks= kycRemarks,
                    ext1=""
                };
                string errorMessage = string.Empty;
                var response = new CallService().PostResponse<string>("putDetailsCustomerKYC", req,ref errorMessage);
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
                var req = new {
                    task= "url",
                    essentials=new {
                        matchImage=new List<string> { new SessionUtility().GetStringSession("PANCroppedUrl") },
                        customVideoRecordTime="6",
                        hideTopLogo="false",
                        hideBottomLogo="false",
                        callbackUrl= Startup.AppSetting["ManualVideoVerifyUrl"],
                        redirectUrl= Startup.AppSetting["ManualVideoVerifyUrl"],
                        idCardVerification="false",
                        customizations =new { }
                    }
                };
                string errorMessage = string.Empty;
                var response = new CallService().PostResponse<VideoVerificationResponse>("getSignzyVideoVerification", req, ref errorMessage);
                if (string.IsNullOrEmpty(errorMessage))
                {
                    if (response.result!=null)
                    {
                        new SessionUtility().SetSession("VideoToken",response.result.token);
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
                if (string.IsNullOrEmpty(errorMessage))
                {
                    response.PanImage = new SessionUtility().GetStringSession("PANCroppedUrl");
                    return View("Response", response);
                }
                return View("Response");
            }
            catch (Exception)
            {

            }
            return View("Response");
        }
    }
}
