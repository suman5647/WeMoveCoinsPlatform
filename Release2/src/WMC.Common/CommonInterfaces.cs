using System;

namespace WMC.Logic
{
    public interface ICacheObject
    {
        T GetObjectFromCache<T>(string cacheItemName, int cacheTimeInMinutes, Func<T> objectSettingFunction);
        T UpdateCacheObject<T>(string cacheItemName, int cacheTimeInMinutes, T objectSettingValue);
    }
    public interface ICloneCacheObject<T>
    {
        T Clone();
    }
}
