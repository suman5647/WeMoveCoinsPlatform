using Newtonsoft.Json;

namespace WMC.Logic
{
    public interface ISettingsManager
    {
        SettingsValue Get(string key, bool force = false);
        SettingsValue Update(string key, string value, string vtype = "");
    }
    public class SettingsValue
    {
        public virtual string Key { get; set; }
        public virtual string Value { get; set; }

        public virtual T GetJsonData<T>(string vtype = "")
        {
            return JsonConvert.DeserializeObject<T>(Value);
        }

        public virtual string UpdateJsonData<T>(T valueObject, string vtype = "")
        {
            this.Value = JsonConvert.SerializeObject(valueObject);
            return this.Value;
        }
    }
}
