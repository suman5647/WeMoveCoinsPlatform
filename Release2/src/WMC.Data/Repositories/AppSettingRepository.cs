using System;
using System.Data.Entity;
using WMC.Data.Repository.Interfaces;
using System.Linq;

namespace WMC.Data.Repositories
{
    public class AppSettingRepository : DataRepository<AppSetting>, IAppSettingRepository
    {
        public AppSettingRepository(DbContext context)
            : base(context)
        {
        }

        public string GetValue(string key)
        {
            var appSetting = Data.FirstOrDefault(q => q.ConfigKey == key);
            if (appSetting == null)
                throw new Exception("Unable to find '" + key + "' key in AppSettings.");
            return appSetting.ConfigValue;
        }
    }
}