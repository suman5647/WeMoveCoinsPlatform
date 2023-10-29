using System.Linq;
using Newtonsoft.Json;
using WMC.Data;

namespace WMC.Logic
{
    //public interface ISettingsManager
    //{
    //    SettingsValue Get(string key);
    //    SettingsValue Update(string key, string value);
    //}

    public class CurrencySettingsManager : ISettingsManager
    {
        /// <summary>
        /// In minutes
        /// </summary>
        public const int CACHE_EXP = 1;
        public const string CACHE_PREFIX = "CUR_SET_";
        public const string CACHE_FORMAT = "{0}:{1}:";

        private ICacheObject cacheObj;

        public CurrencySettingsManager(ICacheObject cacheObj)
        {
            this.cacheObj = cacheObj;
        }

        public static ISettingsManager GetDefault()
        {
            return new CurrencySettingsManager(new MemCaching());
        }


        private static string GetCacheKey(string key)
        {
            return string.Format(CACHE_FORMAT, CACHE_PREFIX, key).ToUpper();
        }


        public SettingsValue Get(string key, bool force = false)
        {
            return force ? InternalGet(key) : this.cacheObj.GetObjectFromCache(GetCacheKey(key), CACHE_EXP, () => InternalGet(key));
        }

        private static CurrencySettingsValue InternalGet(string key)
        {
            var dc = new MonniData();
            var resVal = dc.Currencies.Where(q => q.Code == key);
            if (resVal.Any())
            {
                return resVal.Select(t => new CurrencySettingsValue()
                {
                    Key = t.Code,
                    Value = t.Text,
                    Id = t.Id,
                    BitgoSettings = t.BitgoSettings,
                    CurrencyTypeId = t.CurrencyTypeId,
                    FXMarkUp = t.FXMarkUp,
                    IsActive = t.IsActive,
                    PayLikeCurrency = t.PayLikeCurrencyCode,
                    PayLikeDetails = t.PayLikeDetails,
                    YourPayCurrency = t.YourPayCurrencyCode,
                }).FirstOrDefault();
            }

            return null;
        }

        public SettingsValue Update(string key, string value, string vtype = "bitgo")
        {
            var dc = new MonniData();
            var resVal = dc.Currencies.FirstOrDefault(q => q.Code == key);
            if (resVal != null)
            {
                resVal.BitgoSettings = value;
                dc.SaveChanges();
                cacheObj.UpdateCacheObject(key, CACHE_EXP, value);
            }

            return new SettingsValue() { Key = key, Value = value };
        }
    }

    public class CurrencySettingsValue : SettingsValue
    {
        public long Id { get; set; }
        public long? CurrencyTypeId { get; set; }
        /// <summary>
        /// Code
        /// </summary>
        public override string Key { get; set; }
        /// <summary>
        /// Text
        /// </summary>
        public override string Value { get; set; }

        public string YourPayCurrency { get; set; }
        public string PayLikeCurrency { get; set; }
        public string PayLikeDetails { get; set; }
        public decimal? FXMarkUp { get; set; }
        public string BitgoSettings { get; set; }
        public bool IsActive { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vtype">paylike/bitgo, default: paylike</param>
        /// <returns></returns>
        public override T GetJsonData<T>(string vtype = "paylike")
        {
            return JsonConvert.DeserializeObject<T>(vtype == "paylike" ? PayLikeDetails : BitgoSettings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueObject"></param>
        /// <param name="vtype">paylike/bitgo, default: bitgo</param>
        /// <returns></returns>
        public override string UpdateJsonData<T>(T valueObject, string vtype = "bitgo")
        {
            if (vtype == "paylike")
            {
                this.PayLikeDetails = JsonConvert.SerializeObject(valueObject);
                return this.PayLikeDetails;
            }
            else
            {
                this.BitgoSettings = JsonConvert.SerializeObject(valueObject);
                return this.BitgoSettings;
            }
        }
    }
}