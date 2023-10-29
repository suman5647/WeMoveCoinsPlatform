using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Linq;
using WMC.Logic.Models;

namespace WMC.Logic
{
    public class QuickPayService
    {
        // TODO: mark variables as private?
        string url = "https://api.quickpay.net";
        string userAPIKey;
        string paymentMethods;
        bool isFramed;
        RestClient restClient;
        public QuickPaySettings[] quickPaySettings;

        public QuickPayService(long siteId)
        {
            string url;
            url = this.url;
            restClient = new RestClient(url);
            //store here all site keys 
            var quickPaySettings = SettingsManager.GetDefault().Get("QuickPayDetails").GetJsonData<QuickPaySettings[]>();
            if (quickPaySettings == null)
            {
                AuditLog.log("QuickPayDetails is not defined in the database.", (long)Data.Enums.AuditLogStatus.ApplicationError, (long)Data.Enums.AuditTrailLevel.Debug);
                throw new Exception("QuickPayDetails is not defined in the database.");
            }
            var currentQuickDetails = quickPaySettings.Where(detail => detail.SiteId == siteId).FirstOrDefault<QuickPaySettings>();
            if (currentQuickDetails == null)
            {
                AuditLog.log("Configuration not found for siteId : " + siteId, (long)Data.Enums.AuditLogStatus.ApplicationError, (long)Data.Enums.AuditTrailLevel.Debug);
                throw new Exception("Configuration not found for siteId : " + siteId);
            }
            userAPIKey = currentQuickDetails.UserAPIKey;
            paymentMethods = currentQuickDetails.PaymentMethod;
            isFramed = currentQuickDetails.isFramed;
            restClient.Authenticator = new HttpBasicAuthenticator(string.Empty, userAPIKey);
            restClient.AddDefaultHeader("Accept-Version", "v10");
        }

        public IRestResponse GetPayment(string payment_id)
        {
            var restRequest = new RestRequest();
            restClient.BaseUrl = new Uri(url + "/payments/" + payment_id);
            var response = restClient.Get(restRequest);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                AuditLog.log("Error while getting the payment. Request Content: " +
                   "PaymentId:" + payment_id + " " +
                   "Response Content: " + JsonConvert.SerializeObject(response),
                   (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Error);
                return response;
            }
            AuditLog.log("Get payment Response. Reponse Content: " + response.Content,
                                   (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Debug);
            return response;
        }

        public IRestResponse CreatePayment(long orderId, long orderNumber, string currency, string text_on_statement)
        {
            var restRequest = new RestRequest();
            restClient.BaseUrl = new Uri(url + "/payments/");
            var paymentParameters = new QuickPayRecipients(orderNumber, currency, text_on_statement);
            restRequest.AddJsonBody(paymentParameters);
            var response = restClient.Post(restRequest);
            if (response.StatusCode != System.Net.HttpStatusCode.Created)
            {
                AuditLog.log("Error while creating the payment. Request Content: " +
                   "orderId:" + orderId + " " +
                   "Response Content: " + JsonConvert.SerializeObject(response),
                   (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Error, orderId);
                return response;
            }
            AuditLog.log("Create payment Response. Response Content: " + response.Content,
                                   (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Debug, orderId);
            return response;
        }

        public IRestResponse CreatePaymentLink(long payment_id, long amount, long orderId)
        {
            var restRequest = new RestRequest();

            restClient.BaseUrl = new Uri(url + "/payments/" + payment_id + "/link");
            bool framed = isFramed ? isFramed : false;
            framed = true;
            string continue_url = "";
            string cancel_url = "";
            var paymentParameters = new QuickPayPaymentLink(amount, framed, paymentMethods, continue_url, cancel_url);
            restRequest.AddJsonBody(paymentParameters);
            var response = restClient.Put(restRequest);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                AuditLog.log("Error while creating the payment link. Request Content: " +
                   "paymentId:" + payment_id + " " +
                   "Response Content: " + JsonConvert.SerializeObject(response),
                   (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Error, orderId);
                return response;
            }
            AuditLog.log("Create payment link Response. Response Content: " + response.Content,
                                   (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Debug, orderId);
            return response;
        }

        public IRestResponse CreatePaymentLink(long payment_id, long amount, string paymentMethods, long orderId)
        {
            var restRequest = new RestRequest();

            restClient.BaseUrl = new Uri(url + "/payments/" + payment_id + "/link");
            bool framed = isFramed ? isFramed : false;
            framed = true;
            string continue_url = "";
            string cancel_url = "";
            var paymentParameters = new QuickPayPaymentLink(amount, framed, paymentMethods, continue_url, cancel_url);
            restRequest.AddJsonBody(paymentParameters);
            var response = restClient.Put(restRequest);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                AuditLog.log("Error while creating the payment link. Request Content: " +
                   "paymentId:" + payment_id + " " +
                   "Response Content: " + JsonConvert.SerializeObject(response),
                   (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Error);
                return response;
            }
            AuditLog.log("Create payment link Response. Response Content: " + response.Content,
                                   (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Debug, orderId);
            return response;
        }
        public IRestResponse AuthorizePayment(long payment_id, long amount)
        {
            var restRequest = new RestRequest();
            restClient.BaseUrl = new Uri(url + "/payments/" + payment_id + "/authorize");
            var paymentParameters = new QuickPayCapturePayment(amount);
            restRequest.AddJsonBody(paymentParameters);
            var response = restClient.Post(restRequest);
            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                AuditLog.log("Error while authorizing the payment. Request Content: " +
                     "paymentId:" + payment_id + " " +
                     "Response Content: " + JsonConvert.SerializeObject(response),
                     (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Error);
                return response;
            }
            AuditLog.log("Authorize payment Response. Response Content: " + response.Content,
                                       (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Debug);
            return response;
        }

        public IRestResponse CapturePayment(long payment_id, long amount)
        {
            var restRequest = new RestRequest();
            restClient.BaseUrl = new Uri(url + "/payments/" + payment_id + "/capture");
            var paymentParameters = new QuickPayCapturePayment(amount);
            restRequest.AddJsonBody(paymentParameters);
            var response = restClient.Post(restRequest);
            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                AuditLog.log("Error while capturing the payment. Request Content: " +
                     "paymentId:" + payment_id + " " +
                     "Response Content: " + JsonConvert.SerializeObject(response),
                     (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Error);
                return response;
            }
            AuditLog.log("Capture payment Response. Response Content: " + response.Content,
                                       (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Debug);
            return response;
        }

        public IRestResponse CancelPayment(long payment_id)
        {
            var restRequest = new RestRequest();
            restClient.BaseUrl = new Uri(url + "/payments/" + payment_id + "/cancel");
            var response = restClient.Post(restRequest);
            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                AuditLog.log("Error while canceling payment. Request Content: " +
                     "paymentId:" + payment_id + " " +
                     "Response Content: " + JsonConvert.SerializeObject(response),
                     (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Error);
                return response;
            }
            AuditLog.log("Cancel payment Response. Response Content: " + response.Content,
                                       (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Debug);
            return response;
        }

        public IRestResponse RefundPayment(long payment_id, long amount)
        {
            var restRequest = new RestRequest();
            restClient.BaseUrl = new Uri(url + "/payments/" + payment_id + "/refund");
            var paymentParameters = new QuickPayCapturePayment(amount);
            restRequest.AddJsonBody(paymentParameters);
            var response = restClient.Post(restRequest);
            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                AuditLog.log("Error while refunding payment. Request Content: " +
                     "paymentId:" + payment_id + " " +
                     "Response Content: " + JsonConvert.SerializeObject(response),
                     (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Error);
                return response;
            }
            AuditLog.log("Refund payment Response. Response Content: " + response.Content,
                                       (int)Data.Enums.AuditLogStatus.QuickPay, (int)Data.Enums.AuditTrailLevel.Debug);
            return response;
        }
        //interface
    }
}
