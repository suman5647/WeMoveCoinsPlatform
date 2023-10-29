using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RestSharp;
using Twilio;
using WMC.Data;
using WMC.Logic;

namespace WMC.Web
{
    public interface IMessageService
    {
        bool IsTest();
        bool IsConfigured();
        string GetCallerIdentity(string countryCodeAndPhoneNumber);
        object SendMessage(string to, string message, bool? alphaSupport, params object[] messageParams);
    }

    public class TwilioService : IMessageService
    {
        public const string KEY_TWILIO_TEST_OR_PROD = "TwilioTestOrProd";
        public const string KEY_TWILIO_TEST_SETTINGS = "TwilioTestDetails";
        public const string KEY_TWILIO_PROD_SETTINGS = "TwilioProdDetails";

        // private ISettingsManager settingsManager;
        private bool configured = false;
        private TwilioSettings settings = null;

        public static IMessageService GetDefault(long siteId)
        {
            return new TwilioService(SettingsManager.GetDefault(), siteId);
        }

        public TwilioService(ISettingsManager settingsManager, long siteId)
        {
            // this.settingsManager = settingsManager;
            // var 

            var testOrProdSetting = settingsManager.Get(KEY_TWILIO_TEST_OR_PROD);
            if (testOrProdSetting != null)
            {
                configured = true;
                bool isTest = testOrProdSetting.Value.Equals("Test", StringComparison.InvariantCultureIgnoreCase);
                var twilioSettingsConfig = settingsManager.Get((isTest ? KEY_TWILIO_TEST_SETTINGS : KEY_TWILIO_PROD_SETTINGS), true);
                var twilioSettings = twilioSettingsConfig.GetJsonData<TwilioSettings[]>();
                foreach (var setting in twilioSettings)
                {
                    if (setting.SiteId == siteId)
                    {
                        this.settings = setting;
                        this.settings.IsTest = isTest;
                    }
                }
            }
        }

        public bool IsTest()
        {
            return settings == null ? true : settings.IsTest;
        }

        public bool IsConfigured()
        {
            return configured;
        }

        public Message SendVerificationCode(string to, string customerName, long code, string message, long orderId, string trn_amount, string trn_currency)
        {
            var dc = new MonniData();
            // TODO: select fields - mandatory (order/user)
            var order = dc.Orders.Where(q => q.Id == orderId).FirstOrDefault();
            var user = dc.Users.Where(q => q.Id == order.UserId).FirstOrDefault();
            var alphaSupport = user.Country.AlphaSupport;

            // Send a new outgoing SMS by POSTing to the Messages resource */
            var messageResult = SendMessage(to, message, alphaSupport, customerName, code, trn_amount, trn_currency, orderId);
            AuditLog.log(JsonConvert.SerializeObject(messageResult), (int)Data.Enums.AuditLogStatus.Twilio, (int)Data.Enums.AuditTrailLevel.Info, orderId);
            return messageResult as Message;
        }

        public object SendMessage(string to, string message, bool? alphaSupport, params object[] messageParams)
        {
            var from = alphaSupport.HasValue ? (alphaSupport.Value ? settings.From : settings.FromNumber) : settings.FromNumber;
            var messageText = string.Format(message, messageParams);
            // Send a new outgoing SMS by POSTing to the Messages resource */
            var messageResult = settings.IsTest ? new Message() { To = to, Body = messageText, AccountSid = settings.AccountSid, From = settings.FromNumber } : new TwilioRestClient(settings.AccountSid, settings.AuthToken).SendMessage(
            // var messageResult = new TwilioRestClient(settings.AccountSid, settings.AuthToken).SendMessage(
                       from, // "YYY-YYY-YYYY", // From number, must be an SMS-enabled Twilio number
                       to,             // To number, if using Sandbox see note above message content
                       messageText);
            return messageResult;
        }

        public string GetCallerIdentity(string countryCodeAndPhoneNumber)
        {
            try
            {
                return settings.IsTest ? countryCodeAndPhoneNumber : new TwilioRestClientProxy(settings.AccountSid, settings.AuthToken).GetCallerIdentity(countryCodeAndPhoneNumber);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetCallerIdentity(" + countryCodeAndPhoneNumber + ")", ex);
            }
        }

        public class TwilioSettings
        {
            [JsonIgnore]
            public bool IsTest { get; set; }
            public int SiteId { get; set; }
            public string From { get; set; }
            public string FromNumber { get; set; }
            public string AccountSid { get; set; }
            public string AuthToken { get; set; }
            public string Message { get; set; }
        }
    }

    public class TwilioRestClientProxy : TwilioRestClient
    {
        public TwilioRestClientProxy(string accountSid, string authToken) : this(accountSid, authToken, accountSid)
        {
        }

        public TwilioRestClientProxy(string accountSid, string authToken, string accountResourceSid) : base(accountSid, authToken, accountResourceSid, "v1", "https://lookups.twilio.com/")
        {
        }

        public string GetCallerIdentity(string countryCodeAndPhoneNumber)
        {
            var request = new RestRequest(Method.GET);
            request.Resource = "PhoneNumbers/{CountryCodeAndPhoneNumber}/?AddOns=provider_phone_reputation";
            request.AddUrlSegment("CountryCodeAndPhoneNumber", countryCodeAndPhoneNumber);
            return Execute(request).Content;
        }
    }
}