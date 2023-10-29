using NBitcoin;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;

namespace BitGoSharp
{
    public class BitGoClient
    {
        public static dynamic Constants { get; set; }

        string url;
        RestClient restClient;
        public List<string> Log { get; set; }
        public BitGoClient(string env, string accessToken)
        {
            string baseUrl;
            if (env == "Prod")
                baseUrl = "https://www.bitgo.com";
            else
                baseUrl = "https://test.bitgo.com";
            url = baseUrl + "/api/v1";
            restClient = new RestClient(url);
            restClient.AddDefaultHeader("Authorization", "Bearer " + accessToken);
            FetchConstants();
            if (Constants == null)
                Constants = new Constants
                {
                    maxFee = 0.1e8,
                    maxFeeRate = 100000,
                    minFeeRate = 2000,
                    fallbackFeeRate = 20000,
                    minOutputSize = 2730,
                    bitgoEthAddress = "0x0f47ea803926926f299b7f1afc8460888d850f47"
                };
            if (env == "Prod")
                Constants.network = Network.Main;
            else
                Constants.network = Network.TestNet;
        }

        public Wallet GetWallet(string walletId)
        {
            restClient.BaseUrl = new Uri(url);
            return new Wallet(restClient, this, walletId);
        }

        public IRestResponse Unlock(string otp, int? duration = 600)
        {
            var request = new RestRequest();
            if (duration.HasValue)
                request.AddJsonBody(new { otp = otp, duration = duration });
            else
                request.AddJsonBody(new { otp = otp });
            restClient.BaseUrl = new Uri(url + "/user/unlock");
            return restClient.Post(request);
        }

        public IRestResponse SendOTP(bool? forceSMS = true)
        {
            var request = new RestRequest();
            if (forceSMS.HasValue)
                request.AddJsonBody(new { forceSMS = forceSMS });
            restClient.BaseUrl = new Uri(url + "/user/sendotp");
            return restClient.Post(request);
        }

        public IRestResponse Session()
        {
            var request = new RestRequest();
            restClient.BaseUrl = new Uri(url + "/user/session");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            return restClient.Get(request);
        }

        public IRestResponse EstimateFee(dynamic numBlocks, string maxFee, string version)
        {
            var request = new RestRequest();
            request.AddQueryParameter("numBlocks", numBlocks);
            request.AddQueryParameter("maxFee", maxFee);
            request.AddQueryParameter("version", version);
            restClient.BaseUrl = new Uri(url + "/tx/fee");
            return restClient.Get(request);
        }

        public IRestResponse GetBitGoFeeAddress()
        {
            restClient.BaseUrl = new Uri(url + "/billing/address");
            return restClient.Post(new RestRequest());
        }

        public IRestResponse GetUnspentsForSingleKey(string feeSingleKeySourceAddress, string feeTarget)
        {
            restClient.BaseUrl = new Uri(url + "/address/" + feeSingleKeySourceAddress + "/unspents?target=" + feeTarget);
            return restClient.Get(new RestRequest());
        }

        public void FetchConstants()
        {
            try
            {
                restClient.BaseUrl = new Uri(url + "/client/constants");
                dynamic response = SimpleJson.DeserializeObject(restClient.Get(new RestRequest()).Content);
                Constants = SimpleJson.DeserializeObject<Constants>(response.constants.ToString());
                //Constants.MaxFee = response.constants.maxFee;
                //Constants.MaxFeeRate = response.constants.maxFeeRate;
                //Constants.MinFeeRate = response.constants.minFeeRate;
                //Constants.FallbackFeeRate = response.constants.fallbackFeeRate;
                //Constants.MinOutputSize = response.constants.minOutputSize;
                //Constants.BitgoEthAddress = response.constants.bitgoEthAddress;
            }
            catch (Exception ex)
            {
            }
        }
    }

    public class Constants
    {
        public double maxFee;
        public decimal maxFeeRate;
        public decimal minFeeRate;
        public decimal fallbackFeeRate;
        public decimal minOutputSize;
        public string bitgoEthAddress;
        public Network network;
    }
}