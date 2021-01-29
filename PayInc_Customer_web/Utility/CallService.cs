using Newtonsoft.Json;
using PayInc_Customer_web.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace PayInc_Customer_web.Utility
{
    public class CallService
    {
        #region CALL HTTP POST RS
        private string HttpPostRS(string serviceUrl, string payLoad, ref string error)
        {
            string response = null;
            int timedout = 0;
            //UserDetails
            try
            {
                timedout = Convert.ToInt32("300000");
                var client = new RestClient(Startup.AppSetting["WebApiUrl"] + serviceUrl);

                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var request = new RestRequest(Method.POST);
                request.Timeout = timedout;
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", payLoad, ParameterType.RequestBody);
                IRestResponse iRestResponse = client.Execute(request);

                if (iRestResponse.StatusCode == HttpStatusCode.Accepted || iRestResponse.StatusCode == HttpStatusCode.OK)
                {
                    response = iRestResponse.Content;
                }
                else
                {
                    error = iRestResponse.Content;
                }
            }
            catch (Exception ex)
            {
                error = "Exception: " + ex.Message;
            }
            finally { }

            return response;
        }

        private string HttpPostWithParams(string serviceUrl, List<KeyValuePair<string, string>> parameter, string payLoad, ref string error)
        {
            string response = null;
            int timedout = 0;
            //UserDetails
            try
            {
                timedout = Convert.ToInt32("300000");
                var client = new RestClient(Startup.AppSetting["WebApiUrl"] + serviceUrl);

                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var request = new RestRequest(Method.POST);
                request.Timeout = timedout;
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", payLoad, ParameterType.RequestBody);
                for (int i = 0; i < parameter.Count; i++)
                {
                    request.AddParameter(parameter[i].Key, parameter[i].Value, ParameterType.QueryString);
                }
                IRestResponse iRestResponse = client.Execute(request);

                if (iRestResponse.StatusCode == HttpStatusCode.Accepted || iRestResponse.StatusCode == HttpStatusCode.OK)
                {
                    response = iRestResponse.Content;
                }
                else
                {
                    error = iRestResponse.Content;
                }
            }
            catch (Exception ex)
            {
                error = "Exception: " + ex.Message;
            }
            finally { }

            return response;
        }

        private string HttpPostImage(string serviceUrl, string payLoad, ref string error)
        {
            string response = null;
            int timedout = 0;
            //UserDetails
            try
            {
                timedout = Convert.ToInt32("300000");
                var client = new RestClient(Startup.AppSetting["FileUploadUrl"] + serviceUrl);

                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var request = new RestRequest(Method.POST);
                request.Timeout = timedout;
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", payLoad, ParameterType.RequestBody);
                IRestResponse iRestResponse = client.Execute(request);

                if (iRestResponse.StatusCode == HttpStatusCode.Accepted || iRestResponse.StatusCode == HttpStatusCode.OK)
                {
                    response = iRestResponse.Content;
                }
                else
                {
                    error = iRestResponse.Content;
                }
            }
            catch (Exception ex)
            {
                error = "Exception: " + ex.Message;
            }
            finally { }

            return response;
        }
        private string HttpGetRS(string serviceUrl, List<KeyValuePair<string, string>> parameter, ref string error)
        {
            string response = null;
            int timedout = 0;
            //UserDetails
            try
            {
                timedout = Convert.ToInt32("300000");
                var client = new RestClient(Startup.AppSetting["WebApiUrl"] + serviceUrl);

                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var request = new RestRequest(Method.GET);
                request.Timeout = timedout;
                request.AddHeader("content-type", "application/json");
                for (int i = 0; i < parameter.Count; i++)
                {
                    request.AddParameter(parameter[i].Key, parameter[i].Value, ParameterType.QueryString);
                }
                
                IRestResponse iRestResponse = client.Execute(request);

                if (iRestResponse.StatusCode == HttpStatusCode.Accepted || iRestResponse.StatusCode == HttpStatusCode.OK)
                {
                    response = iRestResponse.Content;
                }
                else
                {
                    error = iRestResponse.Content;
                }
            }
            catch (Exception ex)
            {
                error = "Exception: " + ex.Message;
            }
            finally { }

            return response;
        }
        #endregion
        public T PostResponse<T>(string methodName, object requestParam, ref string errorMessage)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var response = HttpPostRS(methodName, Newtonsoft.Json.JsonConvert.SerializeObject(requestParam), ref errorMessage);
                if (response == null) { return default(T); }
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(Convert.ToString(response));
                if (apiResponse == null) { return default(T); }
                if (apiResponse.errorCode != 200)
                {
                    errorMessage = apiResponse.message;
                    return default(T);
                }
                if (apiResponse.status!=true)
                {
                    errorMessage = apiResponse.message;
                    return default(T);
                }
                return Convert.ToString(apiResponse.response) == null ? default(T) : JsonConvert.DeserializeObject<T>(Convert.ToString(apiResponse.response));
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return default(T);
            }
        }

        public T PostTransaction<T>(string methodName, object requestParam, ref string errorMessage)
        {
            //ApiResponse apiResponse = new ApiResponse();
            try
            {
                var response = HttpPostRS(methodName, Newtonsoft.Json.JsonConvert.SerializeObject(requestParam), ref errorMessage);
                if (response == null) { return default(T); }
                var apiResponse = JsonConvert.DeserializeObject<T>(Convert.ToString(response));
                if (apiResponse == null) { return default(T); }
                //if (apiResponse.status != true)
                //{
                //    errorMessage = apiResponse.message;
                //    return default(T);
                //}
                return (apiResponse);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return default(T);
            }
        }

        public T PostWithParams<T>(string methodName, List<KeyValuePair<string, string>> parameter, object requestParam, ref string errorMessage)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var response = HttpPostWithParams(methodName, parameter,Newtonsoft.Json.JsonConvert.SerializeObject(requestParam), ref errorMessage);
                if (response == null) { return default(T); }
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(Convert.ToString(response));
                if (apiResponse == null) { return default(T); }
                if (apiResponse.errorCode != 200)
                {
                    errorMessage = apiResponse.message;
                    return default(T);
                }
                if (apiResponse.status != true)
                {
                    errorMessage = apiResponse.message;
                    return default(T);
                }
                return Convert.ToString(apiResponse.response) == null ? default(T) : JsonConvert.DeserializeObject<T>(Convert.ToString(apiResponse.response));
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return default(T);
            }
        }


        public T GetResponse<T>(string methodName, List<KeyValuePair<string,string>> keyValues, ref string errorMessage)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var response = HttpGetRS(methodName, keyValues, ref errorMessage);
                if (response == null) { return default(T); }
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(Convert.ToString(response));
                if (apiResponse == null) { return default(T); }
                if (apiResponse.errorCode != 200 )
                {
                    //if (apiResponse.errorCode != 1027)
                    //{
                        errorMessage = apiResponse.message;
                        return default(T);
                    //}
                    
                }
                if (apiResponse.status != true)
                {
                    errorMessage = apiResponse.message;
                    return default(T);
                }
                return Convert.ToString(apiResponse.response) == null ? default(T) : JsonConvert.DeserializeObject<T>(Convert.ToString(apiResponse.response));
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return default(T);
            }
        }

        public string PostImage(string methodName, object requestParam, ref string errorMessage)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var response = HttpPostImage(methodName, Newtonsoft.Json.JsonConvert.SerializeObject(requestParam), ref errorMessage);
                return response;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return null;
            }
        }
    }

    public class CallHelpDeskService
    {
        #region CALL HTTP POST RS
        private string HttpPostRS(string serviceUrl, string payLoad, ref string error)
        {
            string response = null;
            int timedout = 0;
            //UserDetails
            try
            {
                timedout = Convert.ToInt32("300000");
                var client = new RestClient(Startup.AppSetting["HelpDeskUrl"] + serviceUrl);

                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var request = new RestRequest(Method.POST);
                request.Timeout = timedout;
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", payLoad, ParameterType.RequestBody);
                IRestResponse iRestResponse = client.Execute(request);

                if (iRestResponse.StatusCode == HttpStatusCode.Accepted || iRestResponse.StatusCode == HttpStatusCode.OK)
                {
                    response = iRestResponse.Content;
                }
                else
                {
                    error = iRestResponse.Content;
                }
            }
            catch (Exception ex)
            {
                error = "Exception: " + ex.Message;
            }
            finally { }

            return response;
        }
        private string HttpGetRS(string serviceUrl, List<KeyValuePair<string, string>> parameter, ref string error)
        {
            string response = null;
            int timedout = 0;
            //UserDetails
            try
            {
                timedout = Convert.ToInt32("300000");
                var client = new RestClient(Startup.AppSetting["HelpDeskUrl"] + serviceUrl);

                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var request = new RestRequest(Method.GET);
                request.Timeout = timedout;
                request.AddHeader("content-type", "application/json");
                for (int i = 0; i < parameter.Count; i++)
                {
                    request.AddParameter(parameter[i].Key, parameter[i].Value, ParameterType.QueryString);
                }

                IRestResponse iRestResponse = client.Execute(request);

                if (iRestResponse.StatusCode == HttpStatusCode.Accepted || iRestResponse.StatusCode == HttpStatusCode.OK)
                {
                    response = iRestResponse.Content;
                }
                else
                {
                    error = iRestResponse.Content;
                }
            }
            catch (Exception ex)
            {
                error = "Exception: " + ex.Message;
            }
            finally { }

            return response;
        }
        #endregion
        public T PostResponse<T>(string methodName, object requestParam, ref string errorMessage)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var response = HttpPostRS(methodName, Newtonsoft.Json.JsonConvert.SerializeObject(requestParam), ref errorMessage);
                if (response == null) { return default(T); }
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(Convert.ToString(response));
                if (apiResponse == null) { return default(T); }
                if (apiResponse.errorCode != 200)
                {
                    errorMessage = apiResponse.message;
                    return default(T);
                }
                if (apiResponse.status != true)
                {
                    errorMessage = apiResponse.message;
                    return default(T);
                }
                return Convert.ToString(apiResponse.response) == null ? default(T) : JsonConvert.DeserializeObject<T>(Convert.ToString(apiResponse.response));
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return default(T);
            }
        }
        public T GetResponse<T>(string methodName, List<KeyValuePair<string, string>> keyValues, ref string errorMessage)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var response = HttpGetRS(methodName, keyValues, ref errorMessage);
                if (response == null) { return default(T); }
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(Convert.ToString(response));
                if (apiResponse == null) { return default(T); }
                if (apiResponse.errorCode != 200)
                {
                    //if (apiResponse.errorCode != 1027)
                    //{
                    errorMessage = apiResponse.message;
                    return default(T);
                    //}

                }
                if (apiResponse.status != true)
                {
                    errorMessage = apiResponse.message;
                    return default(T);
                }
                return Convert.ToString(apiResponse.response) == null ? default(T) : JsonConvert.DeserializeObject<T>(Convert.ToString(apiResponse.response));
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return default(T);
            }
        }

    }
}
