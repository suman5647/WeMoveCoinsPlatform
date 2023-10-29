using System.Linq;
using Newtonsoft.Json;
using WMC.Data;

namespace WMC.Logic
{

    public class SettingsManager : ISettingsManager
    {
        /// <summary>
        /// In minutes
        /// </summary>
        public const int CACHE_EXP = 60;
        public const string CACHE_PREFIX = "SETTINGS_";
        public const string CACHE_FORMAT = "{0}:{1}:";

        private ICacheObject cacheObj;

        public SettingsManager(ICacheObject cacheObj)
        {
            this.cacheObj = cacheObj;
        }

        public static ISettingsManager GetDefault()
        {
            return new SettingsManager(new MemCaching());
        }


        private static string GetCacheKey(string key)
        {
            return string.Format(CACHE_FORMAT, CACHE_PREFIX, key).ToUpper();
        }


        public SettingsValue Get(string key, bool force = false)
        {
            if (force)
            {
                return InternalGet(key);
            }
            else
            {
                return this.cacheObj.GetObjectFromCache(GetCacheKey(key), CACHE_EXP, () => InternalGet(key));
            }
        }

        private static SettingsValue InternalGet(string key)
        {
            var dc = new MonniData();
            var resVal = dc.AppSettings.FirstOrDefault(q => q.ConfigKey == key);
            if (resVal != null)
            {
                string _value = resVal.IsEncrypted ? SecurityUtil.Decrypt(resVal.ConfigValue) : resVal.ConfigValue;
                return new SettingsValue()
                {
                    Key = resVal.ConfigKey,
                    Value = _value
                };
            }

            return null;
        }

        public SettingsValue Update(string key, string value, string vtype = "")
        {
            var dc = new MonniData();
            var resVal = dc.AppSettings.FirstOrDefault(q => q.ConfigKey == key);
            if (resVal != null)
            {
                resVal.ConfigValue = resVal.IsEncrypted ? SecurityUtil.Encrypt(value) : value;
                dc.SaveChanges();
                cacheObj.UpdateCacheObject(key, CACHE_EXP, value);
            }

            return new SettingsValue() { Key = key, Value = value };
        }
    }

}

