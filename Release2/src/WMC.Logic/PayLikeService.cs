using Newtonsoft.Json;
using System;
using System.Linq;
using WMC.Data;
using WMC.Logic;

namespace WMC.Utilities
{
    public class PayLikeService
    {
        public static dynamic GetPayLikeDetails(long id)
        {
            var dc = new MonniData();
            var payLikeTestOrProd = SettingsManager.GetDefault().Get("PayLikeTestOrProd").Value;
            var payLikeDetails = SettingsManager.GetDefault().Get("PayLikeTestDetails").GetJsonData<PayLikeDetails[]>();
            if (payLikeTestOrProd == null)
            {
                AuditLog.log("PayLikeTestOrProd is not defined in the database.", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug);
                throw new Exception("PayLikeTestOrProd is not defined in the database.");
            }
            if (payLikeTestOrProd == "Prod")
            {
                 payLikeDetails = SettingsManager.GetDefault().Get("PayLikeProdDetails").GetJsonData<PayLikeDetails[]>();
                if (payLikeDetails == null)
                {
                    AuditLog.log("PayLikeProdDetails is not defined in the database.", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug);
                    throw new Exception("PayLikeProdDetails is not defined in the database.");
                }
            }
            else if ((payLikeTestOrProd == "Test"))
            {
                payLikeDetails = SettingsManager.GetDefault().Get("PayLikeTestDetails").GetJsonData<PayLikeDetails[]>();
                if (payLikeDetails == null)
                {
                    AuditLog.log("PayLikeTestDetails is not defined in the database.", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug);
                    throw new Exception("PayLikeTestDetails is not defined in the database.");
                }
            }
            foreach (dynamic item in payLikeDetails)
            {
                if (item.SiteId == id)
                {
                    return item;
                }
            }
            AuditLog.log("PayLikeTestDetails is not defined in the database.", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug);
            throw new Exception("PayLikeTestDetails is not defined in the database.");
        }

        public static dynamic GetPayLikeDetails(long siteId, long currencyId)
        {
            var dc = new MonniData();
            var payLikeDetails = dc.Currencies.Where(q => q.Id == currencyId).Select(q => q.PayLikeDetails).FirstOrDefault();
            if (payLikeDetails == null)
            {
                AuditLog.log("PayLike Details is not defined in the Currency table.", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug);
                throw new Exception("PayLike Details is not defined in the Currency table.");
            }

            foreach (dynamic item in JsonConvert.DeserializeObject(payLikeDetails) as Newtonsoft.Json.Linq.JArray)
                if (item.SiteId == siteId)
                    return item;
            AuditLog.log("PayLike Details is not defined in the Currency table.", (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Debug);
            throw new Exception("PayLike Details is not defined in the Currency table.");
        }
    }


    public class PayLikeDetails
    {
        public string SiteId { get; set; }
        public string PublicKey { get; set; }
        public string AppKey { get; set; }
    }
}