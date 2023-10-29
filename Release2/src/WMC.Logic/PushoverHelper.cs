using DojoNorthSoftware.Pushover;
using Newtonsoft.Json;
using System;
using System.Linq;
using WMC.Data;
using WMC.Logic;

namespace WMC.Utilities
{
    public class PushoverHelper
    {
        public static void SendNotification(string title, string message)
        {
            try
            {
                PushoverSettings pushoverSettings = SettingsManager.GetDefault().Get("PushoverSettings").GetJsonData<PushoverSettings>();
                Exception except;
                Pushover.SendNotification(pushoverSettings.AppToken, pushoverSettings.UserKey, title, message, out except);
                //throw except;
            }
            catch (Exception ex)
            {
                AuditLog.log("Unable to send Push notification.\r\nError: " + ex,
                       (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                throw ex;
            }
        }
    }

    public class PushoverSettings
    {
        public string AppToken { get; set; }
        public string UserKey { get; set; }
    }
}