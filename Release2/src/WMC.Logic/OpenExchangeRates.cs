using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using WMC.Data;
using WMC.Logic;

namespace WMC.Utilities
{
    public class OpenExchangeRates
    {
        public static decimal GetBTCExchangeRate(string currency, Dictionary<string, decimal> ratesPerCurrency, decimal btceurRate, string cryptoCurrency)
        {
            var rate = 0.0M;
            var rates = ratesPerCurrency;
            //var rates = application["LatestExchangeRates"] as Dictionary<string, decimal>;
            if (!rates.TryGetValue(currency, out rate))
            {
                AuditLog.log("Unable to get Exchange rates.", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                throw new Exception("Unable to get exchange rates. Please contact the administrator.");
            }

            var eurRate = 0.0M;
            if (!rates.TryGetValue(cryptoCurrency, out eurRate))
            {
                AuditLog.log("Unable to get BTC exchange rates.", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                throw new Exception("Unable to get BTC exchange rates. Please contact the administrator.");
            }

            return rate / eurRate;
        }

        public static decimal GetEURExchangeRate(string currency, Dictionary<string, decimal> ratesPerCurrency = null)
        {
            var rate = 0.0M;
            var rates = new Dictionary<string, decimal>();
            if (ratesPerCurrency == null)
                rates = GetLatestExchangeRates().Rates;
            else
                rates = ratesPerCurrency;

            if (!rates.TryGetValue(currency, out rate))
            {
                AuditLog.log("Unable to get Exchange rates in GetEURExchangeRate().", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                throw new Exception("Unable to get exchange rates. Please contact the administrator.");
            }

            var btcRate = 0.0M;
            if (!rates.TryGetValue("EUR", out btcRate))
            {
                AuditLog.log("Unable to get BTC exchange rates in GetEURExchangeRate().", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                throw new Exception("Unable to get BTC exchange rates. Please contact the administrator.");
            }

            return rate / btcRate;
        }

        public const string OPENEXCHANGE_RATE_CACHE_KEY = "TEMP_Cache_openexchangerates";
        public const string BASE_Currency = "EUR";
        public static ExchangeRate GetLatestExchangeRates()
        {
            try
            {
                var dc = new MonniData();
                var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
                // todo retrier
                var dayString = string.Format("{0}-{1}-{2} {3}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour);
                var appSettingsData = dc.AppSettings.FirstOrDefault(x => x.ConfigKey == OPENEXCHANGE_RATE_CACHE_KEY);
                string exchangeRates = string.Empty;
                ExchangeRate result = null;
                if (appSettingsData != null && string.Equals(appSettingsData.ConfigDescription, dayString, StringComparison.InvariantCultureIgnoreCase))
                {
                    exchangeRates = appSettingsData.ConfigValue;
                    result = JsonConvert.DeserializeObject<ExchangeRate>(exchangeRates);
                }
                else
                {
                    var appSettings = dc.AppSettings.ToList();
                    var openExchangeRatesAPIId = SettingsManager.GetDefault().Get("OpenExchangeRatesAPIId").Value;
                    var latest = "https://openexchangerates.org/api/latest.json?app_id=" + openExchangeRatesAPIId + "&base=" + BASE_Currency;
                    var client = new RestClient(latest);
                    var request = new RestRequest(Method.GET);
                    var response = client.Execute(request);
                    exchangeRates = response.Content;
                    result = JsonConvert.DeserializeObject<ExchangeRate>(exchangeRates);

                    if (result.ChangeBaseCurrency(BASE_Currency))
                    {
                        exchangeRates = JsonConvert.SerializeObject(result);
                    }

                    if (appSettingsData == null)
                    {
                        dc.AppSettings.Add(new AppSetting() { ConfigKey = OPENEXCHANGE_RATE_CACHE_KEY, ConfigValue = exchangeRates, ConfigDescription = dayString });
                        dc.SaveChanges();
                    }
                    else
                    {
                        appSettingsData.ConfigValue = exchangeRates;
                        appSettingsData.ConfigDescription = dayString;
                        dc.SaveChanges();
                    }
                }

                // TODO: Select
                var currencies = DataUnitOfWork.Currencies.Get(curr => curr.CurrencyTypeId == (int)Data.Enums.CurrencyTypes.Digital && curr.IsActive == true).ToList();
                foreach (var item in currencies)
                {
                    decimal rate;
                    if (!result.Rates.TryGetValue(item.Code, out rate))
                    {
                        var usdRate = CryptoCurrencyExchangeRate(item.Code, BASE_Currency);
                        rate = 1 / (decimal)usdRate.ask;
                        result.Rates.Add(item.Code, rate);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                AuditLog.log("Unable to get Open exchange rates in GetLatestExchangeRates().", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                throw new Exception("Unable to get Open Exchange Rates. Please contact the administrator.");
            }
        }

        private static CryptoExchange CryptoCurrencyExchangeRate(string cryptoCurrency, string baseCurrency = "EUR")
        {
            var restClient = new RestClient("https://www.bitgo.com/api/v2/" + cryptoCurrency.ToLower() + "/market/latest");
            var request = new RestRequest();
            var response = restClient.Get(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var deserializedResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
                return JsonConvert.DeserializeObject<CryptoExchange>(JsonConvert.SerializeObject(deserializedResponse.latest.currencies[baseCurrency]));
            }
            else return null;
        }
    }

    public class ExchangeRate
    {
        [JsonProperty("disclaimer")]
        public string Disclaimer { get; set; }

        [JsonProperty("license")]
        public string Licence { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("base")]
        public string Base { get; set; }

        [JsonProperty("rates")]
        public Dictionary<string, decimal> Rates { get; set; }

        internal bool ChangeBaseCurrency(string newCurrencySynbol = "EUR")
        {
            if (this.Base == newCurrencySynbol) return false;

            string _base = this.Base;
            decimal cur_eur_rate = Rates[newCurrencySynbol];

            decimal new_eur_rate = 1;
            decimal base_rate = new_eur_rate / cur_eur_rate;

            // change base currency
            this.Base = newCurrencySynbol;
            // change base currency rate
            Rates[_base] = base_rate;
            // change new currency
            Rates[newCurrencySynbol] = new_eur_rate;
            foreach (var item in Rates)
            {
                if(item.Key != newCurrencySynbol || item.Key != _base)
                {
                    Rates[newCurrencySynbol] = base_rate * item.Value;
                }
            }

            return true;
        }
    }

    public class CryptoExchange
    {
        public double __invalid_name__24h_avg { get; set; }
        public double total_vol { get; set; }
        public int timestamp { get; set; }
        public double last { get; set; }
        public double bid { get; set; }
        public double ask { get; set; }
        public long cacheTime { get; set; }
        public double monthlyLow { get; set; }
        public double monthlyHigh { get; set; }
        public double prevDayLow { get; set; }
        public double prevDayHigh { get; set; }
        public double lastHourLow { get; set; }
        public double lastHourHigh { get; set; }
    }
}