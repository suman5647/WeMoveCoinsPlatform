using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMC.Data;
using RestSharp;
using Newtonsoft.Json;
using WMC.Logic.Models;

namespace WMC.Logic
{
    public class BitGoAccess
    {
        string url;
        string currencyUrl;
        RestClient restClient;
        public BitGoAccessSettings bitGoAccessSettings;

        public BitGoAccess(BitGoAccessSettings bitGoAccessSettings, string cryptoCurrency)
        {
            string baseUrl;
            this.bitGoAccessSettings = bitGoAccessSettings;
            var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
            Currency bitGoCurrency = DataUnitOfWork.Currencies.Get(currency => currency.Code == cryptoCurrency).FirstOrDefault();
            if (bitGoCurrency == null)
            {
                throw new Exception($"Currency {cryptoCurrency} not found in currency table in GetBalance");
            }
            if (bitGoAccessSettings.Environment != "Prod")
            {
                var bitGoCurrencySettings = JsonConvert.DeserializeObject<dynamic>(bitGoCurrency.BitgoSettings);
                cryptoCurrency = bitGoCurrencySettings.TestCurrency;
            }
            baseUrl = bitGoAccessSettings.Url ?? "http://localhost:3080";

            url = baseUrl + "/api/v2";
            currencyUrl = baseUrl + "/api/v2/" + cryptoCurrency.ToLower();
            restClient = new RestClient(url);
            restClient.AddDefaultHeader("Authorization", "Bearer " + bitGoAccessSettings.AccessCode);
            if (bitGoAccessSettings.SessionExpiry == null || (bitGoAccessSettings.SessionExpiry.Value - DateTime.UtcNow).TotalMinutes <= 2)
            {
                Unlock();
                SettingsManager.GetDefault().Update("BitGoAccessCode", JsonConvert.SerializeObject(bitGoAccessSettings)).GetJsonData<BitGoAccessSettings>();
                DataUnitOfWork.Commit();
            }
        }

        /// <summary>
        /// Create bitgo access and validate/assert
        /// </summary>
        /// <param name="bitGoAccessSettings"></param>
        /// <param name="cryptoCurrency"></param>
        /// <param name="bitgoClient"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static bool TryGetAndValidate(BitGoAccessSettings bitGoAccessSettings, string cryptoCurrency, out BitGoAccess bitgoClient, long? orderId = null)
        {
            bitgoClient = new BitGoAccess(bitGoAccessSettings, cryptoCurrency);
            return bitgoClient.ValidateSession(orderId);
        }

        /// <summary>
        /// Unlock and validate session
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool RefreshSession(long? orderId)
        {
            if (bitGoAccessSettings.SessionExpiry == null || (bitGoAccessSettings.SessionExpiry.Value - DateTime.UtcNow).TotalMinutes <= 2)
            {
                var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
                Unlock();
                SettingsManager.GetDefault().Update("BitGoAccessCode", JsonConvert.SerializeObject(bitGoAccessSettings)).GetJsonData<BitGoAccessSettings>();
                DataUnitOfWork.Commit();
            }

            return ValidateSession(orderId);
        }

        public bool ValidateSession(long? orderId)
        {
            var sessionJSON = this.Session();
            if (sessionJSON.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                AuditLog.log("BitGo session is Unauthorized. Use correct AccessCode or reset expired AccessCode.",
                    (int)Data.Enums.AuditLogStatus.BitGo, (int)Data.Enums.AuditTrailLevel.Warn, orderId);
                return false;
            }

            dynamic sessionObject = SimpleJson.DeserializeObject(sessionJSON.Content);
            DateTime dateTimeObj;
            if (DateTime.TryParse(sessionObject.session.expires.ToString(), out dateTimeObj) && DateTime.Compare(dateTimeObj, DateTime.Now) < 0)
            {
                AuditLog.log("BitGo session is locked.", (int)Data.Enums.AuditLogStatus.BitGo, (int)Data.Enums.AuditTrailLevel.Debug, orderId);
                return false;
            }

            return true;
        }

        public BitGoWallet GetWallet(string walletId)
        {
            var request = new RestRequest();
            restClient.BaseUrl = new Uri(currencyUrl + "/wallet/" + walletId);
            var response = restClient.Get(request);
            BitGoWallet wallet = JsonConvert.DeserializeObject<BitGoWallet>(response.Content);
            return wallet;
        }

        public IRestResponse Session()
        {
            var request = new RestRequest();
            restClient.BaseUrl = new Uri(url + "/user/session");
            return restClient.Get(request);
        }

        public BitGoTransaction GetTransaction(string walletId, string txHash)
        {
            restClient.BaseUrl = new Uri(currencyUrl + "/wallet/" + walletId + "/tx/" + txHash);
            var response = restClient.Get(new RestRequest());
            var transaction = new BitGoTransaction();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                transaction = SimpleJson.DeserializeObject<BitGoTransaction>(response.Content);
            return transaction;
        }

        public IRestResponse GetWalletByAddress(string address)
        {
            restClient.BaseUrl = new Uri(currencyUrl + "/wallet/address/" + address);
            return restClient.Get(new RestRequest());
        }

        public string CreateWalletAddress(string walletId)
        {
            restClient.BaseUrl = new Uri(currencyUrl + "/wallet/" + walletId + "/address");
            var response = restClient.Post(new RestRequest());
            var address = new BitGoCreateAddress();
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception("Error occured while creating address");
            address = SimpleJson.DeserializeObject<BitGoCreateAddress>(response.Content);
            return address.address;
        }

        public IRestResponse CreateTransaction(string walletId, Dictionary<string, long> recepients)
        {
            restClient.BaseUrl = new Uri(currencyUrl + "/wallet/" + walletId + "/tx/build");
            var listRecepients = new List<Recipients>();
            foreach (var item in recepients)
                listRecepients.Add(new Recipients(item.Key, item.Value));
            var buildTxInput = new BuildTxInput(listRecepients);
            var restRequest = new RestRequest();
            restRequest.AddJsonBody(buildTxInput);
            var response = restClient.Post(restRequest);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception("Error occured while creating transaction");
            return response;
        }

        public IRestResponse SendCoins(string walletId, string address, long amount, string walletPassphrase, int minConfirms)
        {
            restClient.BaseUrl = new Uri(currencyUrl + "/wallet/" + walletId + "/sendcoins");
            var sendCoin = new SendCoinInput(address, amount, walletPassphrase, minConfirms);
            var restRequest = new RestRequest();
            restRequest.AddJsonBody(sendCoin);
            //TODO LATER;
            //restRequest.Timeout = ;
            var response = restClient.Post(restRequest);
            AuditLog.log("Bitgo SendCoins Response: " + JsonConvert.SerializeObject(response), (int)Data.Enums.AuditLogStatus.BitGo, (int)Data.Enums.AuditTrailLevel.Info);
            //if (response.StatusCode != System.Net.HttpStatusCode.OK)
            //    throw new Exception("Error occured while creating transaction");

            return response;
        }

        public IRestResponse GetWalletTransfer(string walletId, string txId)
        {
            restClient.BaseUrl = new Uri(currencyUrl + "/wallet/" + walletId + "/transfer/" + txId);
            var restRequest = new RestRequest();
            var response = restClient.Get(restRequest);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception("Error occured while creating transaction");
            return response;
        }

        public IRestResponse Unlock()
        {
            if (this.bitGoAccessSettings.TOTPKey == null)
                throw new Exception();
            var bytes = Base32Encoding.ToBytes(this.bitGoAccessSettings.TOTPKey);
            var totp = new TOTP(bytes);
            restClient.BaseUrl = new Uri(url + "/user/unlock");
            while (!(totp.RemainingSeconds() >= 2))
            {
                System.Threading.Thread.Sleep(totp.RemainingSeconds() * 1000);
            }
            var sendCoin = new UnlockInput() { otp = totp.ComputeTotp() };
            var restRequest = new RestRequest();
            restRequest.AddJsonBody(sendCoin);
            var response = restClient.Post(restRequest);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception("Error occured while unlocking session");
            dynamic responseObj = JsonConvert.DeserializeObject(response.Content);
            DateTime dateTimeObj;
            if (!DateTime.TryParse(responseObj.session.unlock.expires.ToString(), out dateTimeObj))
            {
                dateTimeObj = DateTime.UtcNow;
            }
            this.bitGoAccessSettings.SessionExpiry = dateTimeObj;
            return response;
        }
    }
}
