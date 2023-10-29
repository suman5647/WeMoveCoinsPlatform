using BitGo;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using WMC.Data;
using WMC.Logic.Models;

namespace WMC.Logic
{
    public class BitGoUtil
    {
        //public static BitGoFeeResponse GetFee()
        //{
        //    var restClient = new RestClient("https://www.bitgo.com/api/v1/tx/fee");
        //    var response = restClient.Get(new RestRequest());
        //    return SimpleJson.DeserializeObject<BitGoFeeResponse>(response.Content);
        //}

        public static MinersFee GetEstimateFee(BitGoAccessSettings bitGoAccessSettings, string walletId, Dictionary<string, long> recepients, string cryptoCurrency)
        {
            try
            {
                //var bitGoClient = new BitGo.BitGoClient(environment == "Prod" ? BitGoNetwork.Main : BitGoNetwork.Test, accessCode);
                //var walletService = bitGoClient.Wallets[walletId];
                //var unsignedTransactionTask = walletService.CreateTransactionAsync(recepients);
                //unsignedTransactionTask.Wait();
                //return unsignedTransactionTask.Result.Fee;
                var bitGoClient = new BitGoAccess(bitGoAccessSettings, cryptoCurrency);
                var response = bitGoClient.CreateTransaction(walletId, recepients);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return JsonConvert.DeserializeObject<MinersFee>(JsonConvert.SerializeObject(JsonConvert.DeserializeObject<dynamic>(response.Content).feeInfo));
                else
                    throw new Exception("Error occured while calculating fee");

            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetEstimateFee(" + bitGoAccessSettings.Environment + "," + bitGoAccessSettings.AccessCode + "," + walletId + "," + recepients + ")" + ex.Message + "\r\n" + ex.StackTrace, ex);
            }
        }

        public static long GetBalance(string cryptoCurrency)
        {
            try
            {
                var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
                // var appSettings = DataUnitOfWork.AppSettings.Get();
                // var bitGoAccessCodeJSON = appSettings.FirstOrDefault(q => q.ConfigKey == "BitGoAccessCode").ConfigValue; //Get BitGoAccessCode value
                // TODO: select
                Currency currency = DataUnitOfWork.Currencies.Get(currencies => currencies.Code == cryptoCurrency).FirstOrDefault();
                if (currency == null)
                {
                    throw new Exception($"Currency {cryptoCurrency} not found in currency table in GetBalance");
                }

                var bitGosettingsJSON = currency.BitgoSettings;
                BitGoAccessSettings bitGoAccessSettings = SettingsManager.GetDefault().Get("BitGoAccessCode", true).GetJsonData<BitGoAccessSettings>(); // JsonConvert.DeserializeObject<BitGoAccessSettings>(bitGoAccessCodeJSON);
                // TODO: try not to user dynamic
                dynamic bitgoSetting = JsonConvert.DeserializeObject(bitGosettingsJSON);

                return GetBalance(bitGoAccessSettings, bitgoSetting.DefaultWalletId.ToString() as string, cryptoCurrency);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetBalance()\r\n" + ex.ToMessageAndCompleteStacktrace(), ex);
            }
        }

        public static long GetBalance(BitGoAccessSettings bitGoAccessSettings, string walletId, string cryptoCurrency)
        {
            try
            {
                var bitgoClient = new BitGoAccess(bitGoAccessSettings, cryptoCurrency);
                var sessionJSON = bitgoClient.Session();
                if (sessionJSON.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    throw new Exception("BitGo session is Unauthorized. Use correct AccessCode or reset expired AccessCode.");
                // TODO: try not to user dynamic
                dynamic sessionObject = SimpleJson.DeserializeObject(sessionJSON.Content);
                DateTime dateTimeObj;
                if (DateTime.TryParse(sessionObject.session.expires.ToString(), out dateTimeObj) && DateTime.Compare(dateTimeObj, DateTime.Now) < 0)
                    throw new Exception("BitGo session is locked.");
                var wallet = bitgoClient.GetWallet(walletId);
                return wallet.balance;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetBalance(" + bitGoAccessSettings.Environment + "," + bitGoAccessSettings.AccessCode + "," + walletId + ")\r\n" + ex.ToMessageAndCompleteStacktrace(), ex);
            }
        }

        public static Tuple<List<Tuple<string, decimal>>, DateTime, int, decimal> GetTransactionDetails(string hash, string cryptoCurrency)
        {
            try
            {
                var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
                var appSettings = DataUnitOfWork.AppSettings.Get();
                // var bitGoAccessCodeJSON = appSettings.FirstOrDefault(q => q.ConfigKey == "BitGoAccessCode").ConfigValue; //Get BitGoAccessCode value
                Currency currency = DataUnitOfWork.Currencies.Get(currencies => currencies.Code == cryptoCurrency).FirstOrDefault();
                if (currency == null)
                {
                    throw new Exception($"Currency {cryptoCurrency} not found in currency table in GetBalance");
                }
                var bitGosettingsJSON = currency.BitgoSettings;
                BitGoAccessSettings bitGoAccessSettings = SettingsManager.GetDefault().Get("BitGoAccessCode", true).GetJsonData<BitGoAccessSettings>(); // JsonConvert.DeserializeObject<BitGoAccessSettings>(bitGoAccessCodeJSON);
                dynamic bitgoSetting = JsonConvert.DeserializeObject(bitGosettingsJSON);

                return GetTransactionDetails(bitGoAccessSettings, bitgoSetting.DefaultWalletId.ToString() as string, hash, cryptoCurrency);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetTransactionDetails(" + hash + ")" + ex.Message + "\r\n" + ex.StackTrace, ex);
            }
        }

        public static Tuple<List<Tuple<string, decimal>>, DateTime, int, decimal> GetTransactionDetails(BitGoAccessSettings bitGoAccessSettings, string walletId, string hash, string cryptoCurrency)
        {
            try
            {
                var bitgoClient = new BitGoAccess(bitGoAccessSettings, cryptoCurrency);
                var sessionJSON = bitgoClient.Session();
                if (sessionJSON.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    throw new Exception("BitGo session is Unauthorized. Use correct AccessCode or reset expired AccessCode.");
                dynamic sessionObject = SimpleJson.DeserializeObject(sessionJSON.Content);
                DateTime dateTimeObj;
                if (DateTime.TryParse(sessionObject.session.expires.ToString(), out dateTimeObj) && DateTime.Compare(dateTimeObj, DateTime.Now) < 0)
                    throw new Exception("BitGo session is locked.");
                var wallet = bitgoClient.GetWallet(walletId);
                var transactionJson = bitgoClient.GetWalletTransfer(walletId, hash).Content;
                var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
                DataUnitOfWork.AuditTrails.Add(new AuditTrail { Message = "In GetTransactionDetails(): BitGoTransaction Response:" + transactionJson, Status = 2, Created = DateTime.Now, });
                DataUnitOfWork.Commit();
                var transaction = JsonConvert.DeserializeObject<dynamic>(transactionJson);



                //var newAddressAndValueList = transaction.outputs.Where(q => q.isMine == true).
                //    Select(q => new Tuple<string, double>(q.account, q.value)).ToList();
                var newAddressAndValueList = new List<Tuple<string, decimal>>();
                foreach (var item in transaction.outputs)
                {
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                    if (item.wallet == walletId)
                        newAddressAndValueList.Add(new Tuple<string, decimal>(item.address.ToString(), Convert.ToDecimal(item.value.ToString())));
                }
                DateTime date;
                DateTime.TryParse(transaction.date.ToString(), out date);
                return new Tuple<List<Tuple<string, decimal>>, DateTime, int, decimal>(newAddressAndValueList, date, (int)transaction.confirmations, Convert.ToDecimal(transaction.feeString.ToString()));
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetTransactionDetails(" + bitGoAccessSettings.Environment + "," + bitGoAccessSettings.AccessCode + "," + walletId + "," + hash + ")" + ex.Message + "\r\n" + ex.StackTrace, ex);
            }
        }

        public static bool ValidateAddress(string address, string cryptoCurrency)
        {
            try
            {
                return GetWalletByAddress(address, cryptoCurrency);
            }
            catch (Exception ex)
            {
                AuditLog.log(ex.Message, (int)Data.Enums.AuditLogStatus.BitGo, (int)Data.Enums.AuditTrailLevel.Error);
                return false;
            }
        }
        public static dynamic GetWalletByAddress(string address, string cryptoCurrency)
        {
            var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
            var appSettings = DataUnitOfWork.AppSettings.Get();
            // var bitGoAccessCodeJSON = appSettings.FirstOrDefault(q => q.ConfigKey == "BitGoAccessCode").ConfigValue; //Get BitGoAccessCode value
            Currency currency = DataUnitOfWork.Currencies.Get(currencies => currencies.Code == cryptoCurrency).FirstOrDefault();
            if (currency == null)
            {
                throw new Exception($"Currency {cryptoCurrency} not found in currency table in GetBalance");
            }
            var bitGosettingsJSON = currency.BitgoSettings;
            BitGoAccessSettings bitGoAccessSettings = SettingsManager.GetDefault().Get("BitGoAccessCode", true).GetJsonData<BitGoAccessSettings>(); //  JsonConvert.DeserializeObject<BitGoAccessSettings>(bitGoAccessCodeJSON);
            dynamic bitgoSetting = JsonConvert.DeserializeObject(bitGosettingsJSON);

            var bitgoClient = new BitGoAccess(bitGoAccessSettings, cryptoCurrency);
            var sessionJSON = bitgoClient.Session();
            if (sessionJSON.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new Exception("BitGo session is Unauthorized. Use correct AccessCode or reset expired AccessCode.");
            dynamic sessionObject = SimpleJson.DeserializeObject(sessionJSON.Content);
            DateTime dateTimeObj;
            if (DateTime.TryParse(sessionObject.session.expires.ToString(), out dateTimeObj) && DateTime.Compare(dateTimeObj, DateTime.Now) < 0)
                throw new Exception("BitGo session is locked.");
            var result = bitgoClient.GetWalletByAddress(address);
            dynamic responseContent = JsonConvert.DeserializeObject<dynamic>(result.Content);
            AuditLog.log("Input : currency : " + cryptoCurrency + ", address : " + address + ", Output : " + JsonConvert.SerializeObject(result.Content), (int)Data.Enums.AuditLogStatus.BitGo, (int)Data.Enums.AuditTrailLevel.Info);
            if (responseContent.name.ToString() == "Invalid")
                return false;
            else
                return true;
        }

    }

    public class BitGoCurrencySettings
    {
        public string DefaultWalletId { get; set; }
        public decimal DefaultAmount { get; set; }
        public string TestCurrency { get; set; }
        public string KrakenCode { get; set; }
        public string KrakenEurPairCode { get; set; }
        public MinersFee MinersFee { get; set; }
        public string PassPhrase { get; set; }
        public long TxUnit { get; set; }
        public List<SellMessageLanguageResource> SellMessageLangRes { get; set; }
    }

    public class MinersFee
    {
        public int size { get; set; }
        public int feeRate { get; set; }
        public decimal fee { get; set; }
        public int payGoFee { get; set; }
        public string payGoFeeString { get; set; }
    }

    public class SellMessageLanguageResource
    {
        public decimal MinAmountInEUR { get; set; }
        public decimal MaxAmountInEUR { get; set; }
        public string LanguageResource { get; set; }
    }
}
