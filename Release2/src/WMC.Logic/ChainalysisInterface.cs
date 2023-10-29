using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using WMC.Data;
using System.Linq;
using System;
using WMC.Logic.Models;

namespace WMC.Logic
{
    public class ChainalysisInterface
    {
        public static WithDrawalAddressAndScore GetWithdrawalAddress(string userId, string withdrawalAddress)
        {
            try
            {
                ChainAnalysisSettings chainalysisSettings = SettingsManager.GetDefault().Get("ChainalysisSettings", true).GetJsonData<ChainAnalysisSettings>();

                if (chainalysisSettings.URL.ToLower().Contains("test"))
                    return new WithDrawalAddressAndScore { score = "!red", address = withdrawalAddress };

                var client = new RestClient(chainalysisSettings.URL);
                //var client = new RestClient(string.Format("{0}{1}/addresses/withdrawal", chainalysisSettings.URL, userId));
                var request = new RestRequest("api/risk/user/{userId}/addresses/withdrawal", Method.POST);
                //client.Authenticator = new HttpBasicAuthenticator(chainalysisSettings.UserName, chainalysisSettings.Password);
                request.AddHeader("Token", chainalysisSettings.Token);
                request.AddUrlSegment("userId", userId);
                request.AddJsonBody(new { address = withdrawalAddress });
                var response = client.Execute(request);
                return JsonConvert.DeserializeObject<WithDrawalAddressAndScore>(response.Content);
            }
            catch (Exception ex)
            {
                AuditLog.log(string.Format("Unable get Withdrawal Address Score from Chainalysis for userid:{0} and withdrawalAddress:{1}.\r\nResponse content:{1}\r\nError: {2}", userId , withdrawalAddress, ex),
                    (int)Data.Enums.AuditLogStatus.Chainalysis, (int)Data.Enums.AuditTrailLevel.Error);
                throw ex;
            }
        }

        public static Output GetOutputs(string userId)
        {
            IRestResponse response = null;
            try
            {
                ChainAnalysisSettings chainalysisSettings = SettingsManager.GetDefault().Get("ChainalysisSettings", true).GetJsonData<ChainAnalysisSettings>();
                var client = new RestClient(chainalysisSettings.URL);
                var request = new RestRequest("api/risk/user/{userId}/outputs/sent", Method.GET);
                //client.Authenticator = new HttpBasicAuthenticator(chainalysisSettings.UserName, chainalysisSettings.Password);
                request.AddHeader("Token", chainalysisSettings.Token);
                request.AddUrlSegment("userId", userId);
                response = client.Execute(request);
                return JsonConvert.DeserializeObject<Output>(response.Content);
            }
            catch (Exception ex)
            {
                AuditLog.log(string.Format("Unable get output from Chainalysis for userid:{0}.\r\nResponse content:{1}\r\nError: {2}", userId, response.Content, ex),
                    (int)Data.Enums.AuditLogStatus.Chainalysis, (int)Data.Enums.AuditTrailLevel.Error);
                throw ex;
            }
        }

        //public static Output GetOutputsTest(string userId)
        //{
        //    var dc = new MonniData();
        //    var client = new RestClient("https://test.chainalysis.com/");
        //    var request = new RestRequest("api/risk/user/{userId}/outputs/sent", Method.GET);
        //    //client.Authenticator = new HttpBasicAuthenticator(chainalysisSettings.UserName, chainalysisSettings.Password);
        //    request.AddHeader("Token", "574bdd89b0233aa536ef9d746aca5d039ffd7c2ca8deda0ba4beda8af7f4ee4b");
        //    request.AddUrlSegment("userId", userId);
        //    var response = client.Execute(request);
        //    return JsonConvert.DeserializeObject<Output>(response.Content);
        //}

        public static Output PostOutputs(string userId, string output)
        {
            IRestResponse response = null;
            try
            {
                ChainAnalysisSettings chainalysisSettings = SettingsManager.GetDefault().Get("ChainalysisSettings", true).GetJsonData<ChainAnalysisSettings>();
                var client = new RestClient(chainalysisSettings.URL);
                var request = new RestRequest("api/risk/user/{userId}/outputs/sent", Method.POST);
                //client.Authenticator = new HttpBasicAuthenticator(chainalysisSettings.UserName, chainalysisSettings.Password);
                request.AddHeader("Token", chainalysisSettings.Token);
                request.AddUrlSegment("userId", userId);
                request.AddJsonBody(new { output = output });
                response = client.Execute(request);
                return JsonConvert.DeserializeObject<Output>(response.Content);
            }
            catch (Exception ex)
            {
                AuditLog.log(string.Format("Unable send output to Chainalysis for userid:{0}.\r\nResponse content:{1}\r\nOutput:{2}\r\nError: {3}", userId, response.Content, output, ex),
                    (int)Data.Enums.AuditLogStatus.Chainalysis, (int)Data.Enums.AuditTrailLevel.Error);
                throw ex;
            }
        }

        public static RecieveOutput ReceivedOutputs(string userId, string output)
        {
            IRestResponse response = null;
            try
            {
                ChainAnalysisSettings chainalysisSettings = SettingsManager.GetDefault().Get("ChainalysisSettings", true).GetJsonData<ChainAnalysisSettings>();
                var client = new RestClient(chainalysisSettings.URL);
                var request = new RestRequest("api/risk/user/{userId}/outputs/received", Method.POST);
                request.AddHeader("Token", chainalysisSettings.Token);
                request.AddUrlSegment("userId", userId);
                request.AddJsonBody(new { output = output });
                response = client.Execute(request);
                AuditLog.log("Response content from ReceivedOutputs(" + userId + "," + output + "):" + response.Content, (int)Data.Enums.AuditLogStatus.TrustLogic, (int)Data.Enums.AuditTrailLevel.Info);
                return JsonConvert.DeserializeObject<RecieveOutput>(response.Content);
            }
            catch (Exception ex)
            {
                AuditLog.log(string.Format("Unable send output to Chainalysis:ReceivedOutputs for userid:{0}.\r\nResponse content:{1}\r\nOutputs:{2}\r\nError: {3}", userId, response.Content, output, ex),
                    (int)Data.Enums.AuditLogStatus.Chainalysis, (int)Data.Enums.AuditTrailLevel.Error);
                throw ex;
            }
        }
    }

    public class WithDrawalAddressAndScore
    {
        public string address { get; set; }
        public string score { get; set; }
    }

    public class WithDrawal
    {
        public int total { get; set; }
        public int limit { get; set; }
        public int offset { get; set; }
        public List<WithDrawalAddressAndScore> data { get; set; }
    }

    public class UserInfo
    {
        public string URL { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
    }

    public class OutputData
    {
        public string output { get; set; }
        public string status { get; set; }
    }

    public class Output
    {
        public int limit { get; set; }
        public int offset { get; set; }
        public List<OutputData> data { get; set; }
    }
    public class RecieveOutput
    {
        public string Score { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
    }
}