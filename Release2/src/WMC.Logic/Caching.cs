using System;
using System.Runtime.Caching;

namespace WMC.Logic
{
    public class MemCaching : ICacheObject
    {
        /// <summary>
        /// A generic method for getting and setting objects to the memory cache.
        /// </summary>
        /// <typeparam name="T">The type of the object to be returned.</typeparam>
        /// <param name="cacheItemName">The name to be used when storing this object in the cache.</param>
        /// <param name="cacheTimeInMinutes">How long to cache this object for.</param>
        /// <param name="objectSettingFunction">A parameterless function to call if the object isn't in the cache and you need to set it.</param>
        /// <returns>An object of the type you asked for</returns>
        public T GetObjectFromCache<T>(string cacheItemName, int cacheTimeInMinutes, Func<T> objectSettingFunction)
        {
            ObjectCache cache = MemoryCache.Default;
            var cachedObject = (T)cache[cacheItemName];
            if (cachedObject == null)
            {
                CacheItemPolicy policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheTimeInMinutes);
                cachedObject = objectSettingFunction();
                cache.Set(cacheItemName, cachedObject, policy);
            }
            if (cachedObject is ICloneCacheObject<T>)
            {
                return (cachedObject as ICloneCacheObject<T>).Clone();
            }
            else
            {
                return cachedObject;
            }
        }

        public T UpdateCacheObject<T>(string cacheItemName, int cacheTimeInMinutes, T objectSettingValue)
        {
            ObjectCache cache = MemoryCache.Default;
            var cachedObject = objectSettingValue;
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheTimeInMinutes);
            if (cache[cacheItemName] != null)
            {
                cache.Remove(cacheItemName);
            }
            if (cachedObject is ICloneCacheObject<T>)
            {
                cache.Set(cacheItemName, (cachedObject as ICloneCacheObject<T>).Clone(), policy);
            }
            else
            {
                cache.Set(cacheItemName, cachedObject, policy);
            }

            return cachedObject;
        }
    }
}