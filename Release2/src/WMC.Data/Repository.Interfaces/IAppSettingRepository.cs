using WMC.Data.Interfaces;
using WMC.Data;

namespace WMC.Data.Repository.Interfaces
{
    public interface IAppSettingRepository : IRepository<AppSetting>
    {
        string GetValue(string key);
    }
}
