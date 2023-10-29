using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WMC.Data;
using WMC.Data.Enums;
using WMC.Logic;

namespace WMC.Utilities
{
    public class KrakenExchange
    {
        public static Dictionary<string, decimal?> GetBTCEURRate()
        {
            try
            {
                var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
                var digitalCurrencies = DataUnitOfWork.Currencies.Get(currency => currency.CurrencyTypeId == (int)(CurrencyTypes.Digital) && currency.IsActive == true);
                var currencyKrakenCodeLookUp = new Dictionary<string, string>();
                string currencyEuroPairs = "";
                foreach (var item in digitalCurrencies)
                {
                    var bitgoSetting = JsonConvert.DeserializeObject<dynamic>(item.BitgoSettings);
                    currencyEuroPairs += bitgoSetting.KrakenEurPairCode;
                    if (item != digitalCurrencies.Last())
                        currencyEuroPairs += ",";
                    currencyKrakenCodeLookUp.Add(bitgoSetting.KrakenEurPairCode.ToString(), item.Code);
                }
                var client = new RestClient("https://api.kraken.com/0/public/Ticker?pair=" + currencyEuroPairs);
                var request = new RestRequest(Method.GET);
                var response = client.Execute(request);
                dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(response.Content);
                Dictionary<string, KrakenExchangeRate> rate = JsonConvert.DeserializeObject<Dictionary<string, KrakenExchangeRate>>(JsonConvert.SerializeObject(jsonData.result));
                Dictionary<string, decimal?> result = new Dictionary<string, decimal?>();
                foreach (var item in rate.Keys)
                {
                    var priceValue = 0.0M;
                    KrakenExchangeRate rateValue;
                    rate.TryGetValue(item, out rateValue);
                    string key;
                    currencyKrakenCodeLookUp.TryGetValue(item, out key);
                    if (decimal.TryParse(rateValue.a[0], NumberStyles.Number, new CultureInfo("en-US"), out priceValue))
                        result.Add(key, priceValue);
                    else result.Add(key, null);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured in GetBTCEURRate().", ex);
            }
        }

        public static decimal GetBTCEURPrice(decimal eurAmount, string cryptoCurrency)
        {
            try
            {
                var approxBTCAmount = eurAmount * 0.00067m;
                var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
                // TODO: select
                var iteration = DataUnitOfWork.Currencies.Get(currency => currency.CurrencyTypeId == (int)(CurrencyTypes.Digital) && currency.Code.ToLower() == cryptoCurrency.ToLower()).FirstOrDefault();
                var bitgoSetting = JsonConvert.DeserializeObject<dynamic>(iteration.BitgoSettings);
                var currencyEuroPair = bitgoSetting.KrakenEurPairCode;
                var client = new RestClient("https://api.kraken.com/0/public/Depth?pair=" + currencyEuroPair);
                var request = new RestRequest(Method.GET);
                var response = client.Execute(request);
                dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(response.Content);
                Dictionary<string, KrakenExchangeDepth> currencyPairDepth = JsonConvert.DeserializeObject<Dictionary<String, KrakenExchangeDepth>>(JsonConvert.SerializeObject(jsonData.result));
                KrakenExchangeDepth currencyDepth;
                currencyPairDepth.TryGetValue(currencyEuroPair.ToString(), out currencyDepth);
                var totalBTCVolume = 0m;
                var price = 0m;
                var highestprice = 0m;
                foreach (var item in currencyDepth.asks)
                {
                    totalBTCVolume += decimal.Parse(item[1]);
                    var tempPrice = decimal.Parse(item[0]);
                    if (highestprice < tempPrice)
                        highestprice = tempPrice;
                    if (totalBTCVolume >= approxBTCAmount)
                    {
                        price = tempPrice;
                        break;
                    }
                }
                //AuditLog.log("eurAmount:" + eurAmount.ToString() + " price:" + price.ToString() + " highestprice:" + highestprice.ToString(),
                //    (int)Data.Enums.AuditLogStatus.OrderBook, (int)Data.Enums.AuditTrailLevel.Info);
                return price == 0m ? highestprice : price;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error occured in GetBTCEURPrice({0})", eurAmount), ex);
            }
        }
    }

    public class KrakenExchangeRate
    {
        public List<string> a { get; set; }
        public List<string> b { get; set; }
        public List<string> c { get; set; }
        public List<string> v { get; set; }
        public List<string> p { get; set; }
        public List<int> t { get; set; }
        public List<string> l { get; set; }
        public List<string> h { get; set; }
        public string o { get; set; }
    }

    public class KrakenExchangeDepth
    {
        public List<List<string>> asks { get; set; }
        public List<List<string>> bids { get; set; }
    }
}