using RestSharp;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using WMC.Data;
using Newtonsoft.Json;
using System;
using WMC.Logic.Models;

namespace WMC.Logic
{
    public class TrustPayService
    {
        public CheckOutResponse PreAuthorization(string currency, decimal amount, string orderNumber, string descriptor, long siteId)
        {
            try
            {
                var details = GetTrustPayDetails(siteId);
                var client = new RestClient("https://" + (details.IsProd ? "" : "test.") + "oppwa.com/v1/checkouts");
                var request = new RestRequest { Method = Method.POST };
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("authentication.userId", details.UserId);
                request.AddParameter("authentication.entityId", details.EntityId);
                request.AddParameter("authentication.password", details.Password);
                if(descriptor != null) request.AddParameter("descriptor", descriptor);
                request.AddParameter("customParameters[SHOPPER_OrderNumber]", orderNumber);
                request.AddParameter("amount", amount);
                request.AddParameter("currency", currency);
                request.AddParameter("paymentType", "PA");
                var response = client.Execute(request);
                return JsonConvert.DeserializeObject<CheckOutResponse>(response.Content);
            }
            catch (Exception ex)
            {
                throw new Exception("Error at TrustPay PreAuthorization.", ex);
            }
        }

        public PaymentStatus PaymentStatus(string checkOutId, long siteId)
        {
            try
            {
                var details = GetTrustPayDetails(siteId);
                string data = "authentication.userId=" + details.UserId +
                    "&authentication.password=" + details.Password +
                    "&authentication.entityId=" + details.EntityId;
                var client = new RestClient("https://" + (details.IsProd ? "" : "test.") + string.Format("oppwa.com/v1/checkouts/{0}/payment?", checkOutId) + data);
                var request = new RestRequest { Method = Method.GET };
                var response = client.Execute(request);
                return JsonConvert.DeserializeObject<PaymentStatus>(response.Content);
            }
            catch (Exception ex)
            {
                throw new Exception("Error at TrustPay PaymentStatus.", ex);
            }
        }

        public Payment CapturePayment(string currency, string amount, string paymentId, long siteId)
        {
            try
            {
                var details = GetTrustPayDetails(siteId);
                var client = new RestClient("https://" + (details.IsProd ? "" : "test.") + "oppwa.com/v1/payments/" + paymentId);
                var request = new RestRequest { Method = Method.POST };
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("authentication.userId", details.UserId);
                request.AddParameter("authentication.entityId", details.EntityId);
                request.AddParameter("authentication.password", details.Password);
                request.AddParameter("amount", amount);
                request.AddParameter("currency", currency);
                request.AddParameter("paymentType", "CP");
                var response = client.Execute(request);
                return JsonConvert.DeserializeObject<Payment>(response.Content);
            }
            catch (Exception ex)
            {
                throw new Exception("Error at TrustPay CapturePayment.", ex);
            }
        }

        public Payment RefundPayment(string currency, string amount, string paymentId, int siteId)
        {
            try
            {
                var details = GetTrustPayDetails(siteId);
                var client = new RestClient("https://" + (details.IsProd ? "" : "test.") + "oppwa.com/v1/payments/" + paymentId);
                var request = new RestRequest { Method = Method.POST };
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("authentication.userId", details.UserId);
                request.AddParameter("authentication.entityId", details.EntityId);
                request.AddParameter("authentication.password", details.Password);
                request.AddParameter("amount", amount);
                request.AddParameter("currency", currency);
                request.AddParameter("paymentType", "RF");
                var response = client.Execute(request);
                return JsonConvert.DeserializeObject<Payment>(response.Content);
            }
            catch (Exception ex)
            {
                throw new Exception("Error at TrustPay RefundPayment.", ex);
            }
        }

        public Payment ReversalPayment(string paymentId, long siteId)
        {
            try
            {
                var details = GetTrustPayDetails(siteId);
                var client = new RestClient("https://" + (details.IsProd ? "" : "test.") + "oppwa.com/v1/payments/" + paymentId);
                var request = new RestRequest { Method = Method.POST };
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("authentication.userId", details.UserId);
                request.AddParameter("authentication.entityId", details.EntityId);
                request.AddParameter("authentication.password", details.Password);
                request.AddParameter("paymentType", "RV");
                var response = client.Execute(request);
                return JsonConvert.DeserializeObject<Payment>(response.Content);
            }
            catch (Exception ex)
            {
                throw new Exception("Error at TrustPay ReversalPayment.", ex);
            }
        }

        public class TrustPaySettings
        {
            public long SiteId { get; set; }
            public string UserId { get; set; }
            public string EntityId { get; set; }
            public string Password { get; set; }
            public bool IsProd { get; set; }
        }

        public TrustPaySettings GetTrustPayDetails(long siteId)
        {
            var dc = new MonniData();
            bool isProd;
            var trustPayTestOrProd = SettingsManager.GetDefault().Get("TrustPayTestOrProd").Value;
            var trustPayDetails = SettingsManager.GetDefault().Get("TrustPayTestDetails").GetJsonData<TrustPaySettings[]>();
            if (trustPayTestOrProd == null)
            {
                AuditLog.log("TrustPayTestOrProd is not defined in the database.", (long)Data.Enums.AuditLogStatus.ApplicationError, (long)Data.Enums.AuditTrailLevel.Debug);
                throw new Exception("TrustPayTestOrProd is not defined in the database.");
            }
            if (trustPayTestOrProd == "Prod")
            {
                isProd = true;
                trustPayDetails = SettingsManager.GetDefault().Get("TrustPayProdDetails").GetJsonData<TrustPaySettings[]>();
                if (trustPayDetails == null)
                {
                    AuditLog.log("TrustPayProdDetails is not defined in the database.", (long)Data.Enums.AuditLogStatus.ApplicationError, (long)Data.Enums.AuditTrailLevel.Debug);
                    throw new Exception("TrustPayProdDetails is not defined in the database.");
                }
            }
            else if ((trustPayTestOrProd == "Test"))
            {
                isProd = false;
                trustPayDetails = SettingsManager.GetDefault().Get("TrustPayTestDetails").GetJsonData<TrustPaySettings[]>();
                if (trustPayDetails == null)
                {
                    AuditLog.log("TrustPayTestDetails is not defined in the database.", (long)Data.Enums.AuditLogStatus.ApplicationError, (long)Data.Enums.AuditTrailLevel.Debug);
                    throw new Exception("TrustPayTestDetails is not defined in the database.");
                }
            }
            else
            {
                AuditLog.log("trustPayTestOrProd.ConfigValue is having wrong value in the database.", (long)Data.Enums.AuditLogStatus.ApplicationError, (long)Data.Enums.AuditTrailLevel.Debug);
                throw new Exception("trustPayTestOrProd.ConfigValue is having wrong value in the database.");
            }
            var result = trustPayDetails.Where(detail => detail.SiteId == siteId).FirstOrDefault<TrustPaySettings>();
            if (result == null)
            {
                AuditLog.log("Configuration not found for siteId : " + siteId, (long)Data.Enums.AuditLogStatus.ApplicationError, (long)Data.Enums.AuditTrailLevel.Debug);
                throw new Exception("Configuration not found for siteId : " + siteId);
            }
            result.IsProd = isProd;
            return result;
        }
    }
}