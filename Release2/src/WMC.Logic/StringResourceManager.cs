using System.Linq;
using WMC.Data;

namespace WMC.Logic
{
    public interface IResourceManager
    {
        ResourceValue Get(string key, string language);
        // IEnumerable<ResourceValue> Get(string key;
    }

    public class StringResourceManager : IResourceManager
    {
        /// <summary>
        /// In minutes
        /// </summary>
        public const int CACHE_EXP = 60;
        public const string CACHE_PREFIX = "STRING_RESOURCE_";
        /// <summary>
        /// PREFIX, Language, Key
        /// </summary>
        public const string CACHE_FORMAT = "{0}:{1}:{2}:";

        private ICacheObject cacheObj;

        private static string ResolveLanguage(string language)
        {
            if (!string.IsNullOrEmpty(language) && language.Equals("en", System.StringComparison.InvariantCultureIgnoreCase))
            {
                language = "";
            }

            return language;
        }
        private static string GetCacheKey(string key, string language)
        {
            return string.Format(CACHE_FORMAT, CACHE_PREFIX, language, key).ToUpper();
        }

        public StringResourceManager(ICacheObject cacheObj)
        {
            this.cacheObj = cacheObj;
        }
//#if DEBUG
//        static long hitCount = 0;
//        static long lookupCount = 0;
//#endif

        public ResourceValue Get(string key, string language)
        {
//#if DEBUG
//            var timeTrack = new System.Diagnostics.Stopwatch();
//            timeTrack.Start();
//            System.Threading.Interlocked.Increment(ref hitCount);
//#endif
            language = ResolveLanguage(language);
            var result = cacheObj.GetObjectFromCache(GetCacheKey(key, language).ToUpper(), CACHE_EXP, () => InternalGet(key, language));
//#if DEBUG
//            timeTrack.Stop();
//            System.Diagnostics.Debugger.Log(1, "STRRES", string.Format("STR_RES: Hits:{0}, DB Hits:{1}, Time taken:{2}ms.\n", hitCount, lookupCount, timeTrack.ElapsedMilliseconds));
//#endif
            return result;
        }

        private static ResourceValue InternalGet(string key, string language)
        {
            //#if DEBUG
            //            System.Threading.Interlocked.Increment(ref lookupCount);
            //            var timeTrack2 = new System.Diagnostics.Stopwatch();
            //            timeTrack2.Start();
            //#endif
            var dc = new MonniData();
            var resVal = dc.LanguageResources.Where(q => q.Key == key && q.Language == language);
            ResourceValue result = null;
            if (resVal.Any())
            {
                result = resVal.Select(t => new ResourceValue()
                {
                    Language = t.Language,
                    Key = t.Key,
                    Value = t.Value
                }).FirstOrDefault();
            }

            //#if DEBUG
            //            timeTrack2.Stop();
            //            System.Diagnostics.Debugger.Log(1, "STRRES", string.Format("STR_RES_DB: DB_Time taken:{0}ms.\n", timeTrack2.ElapsedMilliseconds));
            //#endif
            return result != null ? result : new ResourceValue() { Key = key, Language = language, Value = key };
        }
    }

    public class ResourceValue : ICloneCacheObject<ResourceValue>
    {
        public string Language { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public ResourceValue Clone()
        {
            return new ResourceValue() { Key = Key, Language = Language, Value = Value };
        }

        public string Format(params object[] args)
        {
            return string.Format(Value, args);
        }

    }
}
