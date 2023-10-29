using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WMC.Data.Enums;

namespace WMC.Logic.Models
{
    public class QuickPayResponse
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("merchant_id")]
        public int? MerchantId { get; set; }

        [JsonProperty("order_id")]
        public string OrderId { get; set; }

        [JsonProperty("accepted")]
        public bool? Accepted { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("text_on_statement")]
        public string TextOnStatement { get; set; }

        [JsonProperty("branding_id")]
        public object BrandingId { get; set; }

        [JsonProperty("variables")]
        public Variables Variables { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }

        [JsonProperty("link")]
        public Link Link { get; set; }

        [JsonProperty("shipping_address")]
        public object ShippingAddress { get; set; }

        [JsonProperty("invoice_address")]
        public object InvoiceAddress { get; set; }

        [JsonProperty("basket")]
        public List<object> Basket { get; set; }

        [JsonProperty("shipping")]
        public object Shipping { get; set; }

        [JsonProperty("operations")]
        public List<Operation> Operations { get; set; }

        [JsonProperty("test_mode")]
        public bool? TestMode { get; set; }

        [JsonProperty("acquirer")]
        public string Acquirer { get; set; }

        [JsonProperty("facilitator")]
        public object Facilitator { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("retented_at")]
        public object RetentedAt { get; set; }

        [JsonProperty("balance")]
        public int? Balance { get; set; }

        [JsonProperty("fee")]
        public object Fee { get; set; }

        [JsonProperty("deadline_at")]
        public object DeadlineAt { get; set; }
    }

    public class Variables
    {
    }

    public class Metadata
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("origin")]
        public string Origin { get; set; }

        [JsonProperty("brand")]
        public string Brand { get; set; }

        [JsonProperty("bin")]
        public string Bin { get; set; }

        [JsonProperty("corporate")]
        public bool Corporate { get; set; }

        [JsonProperty("last4")]
        public string Last4 { get; set; }

        [JsonProperty("exp_month")]
        public int? ExpMonth { get; set; }

        [JsonProperty("exp_year")]
        public int? ExpYear { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("is_3d_secure")]
        public bool Is3dSecure { get; set; }

        [JsonProperty("issued_to")]
        public object IssuedTo { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("number")]
        public object Number { get; set; }

        [JsonProperty("customer_ip")]
        public string CustomerIp { get; set; }

        [JsonProperty("customer_country")]
        public string CustomerCountry { get; set; }

        [JsonProperty("fraud_suspected")]
        public bool? FraudSuspected { get; set; }

        [JsonProperty("fraud_remarks")]
        public List<string> FraudRemarks { get; set; }

        [JsonProperty("fraud_reported")]
        public bool? FraudReported { get; set; }

        [JsonProperty("fraud_report_description")]
        public object FraudReportDescription { get; set; }

        [JsonProperty("fraud_reported_at")]
        public object FraudReportedAt { get; set; }

        [JsonProperty("nin_number")]
        public object NinNumber { get; set; }

        [JsonProperty("nin_country_code")]
        public object NinCountryCode { get; set; }

        [JsonProperty("nin_gender")]
        public object NinGender { get; set; }

        [JsonProperty("shopsystem_name")]
        public object ShopSystemName { get; set; }

        [JsonProperty("shopsystem_version")]
        public object ShopSystemVersion { get; set; }
    }

    public class BrandingConfig
    {
    }

    public class Link
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("agreement_id")]
        public int? AgreementId { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("amount")]
        public int? Amount { get; set; }

        [JsonProperty("continue_url")]
        public object ContinueUrl { get; set; }

        [JsonProperty("cancel_url")]
        public object CancelUrl { get; set; }

        [JsonProperty("callback_url")]
        public object CallbackUrl { get; set; }

        [JsonProperty("payment_methods")]
        public string PaymentMethods { get; set; }

        [JsonProperty("auto_fee")]
        public bool? AutoFee { get; set; }

        [JsonProperty("auto_capture")]
        public object AutoCapture { get; set; }

        [JsonProperty("branding_id")]
        public object BrandingId { get; set; }

        [JsonProperty("google_analytics_client_id")]
        public object GoogleAnalyticsClientId { get; set; }

        [JsonProperty("google_analytics_tracking_id")]
        public object GoogleAnalyticsTrackingId { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("acquirer")]
        public object Acquirer { get; set; }

        [JsonProperty("deadline")]
        public object Deadline { get; set; }

        [JsonProperty("framed")]
        public bool? Framed { get; set; }

        [JsonProperty("branding_config")]
        public BrandingConfig BrandingConfig { get; set; }

        [JsonProperty("invoice_address_selection")]
        public object InvoiceAddressSelection { get; set; }

        [JsonProperty("shipping_address_selection")]
        public object ShippingAddressSelection { get; set; }

        [JsonProperty("customer_email")]
        public object CustomerEmail { get; set; }
    }

    public class Data
    {
    }

    public class Operation
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("amount")]
        public int? Amount { get; set; }

        [JsonProperty("pending")]
        public bool? Pending { get; set; }

        [JsonProperty("qp_status_code")]
        public string QpStatusCode { get; set; }

        [JsonProperty("qp_status_msg")]
        public string QpStatusMsg { get; set; }

        [JsonProperty("aq_status_code")]
        public string AqStatusCode { get; set; }

        [JsonProperty("aq_status_msg")]
        public string AqStatusMsg { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }

        [JsonProperty("callback_url")]
        public object CallbackUrl { get; set; }

        [JsonProperty("callback_success")]
        public object CallbackSuccess { get; set; }

        [JsonProperty("callback_response_code")]
        public object CallbackResponseCode { get; set; }

        [JsonProperty("callback_duration")]
        public object CallbackDuration { get; set; }

        [JsonProperty("acquirer")]
        public string Acquirer { get; set; }

        [JsonProperty("__invalid_name__3d_secure_status")]
        public string InvalidName3dSecureStatus { get; set; }

        [JsonProperty("callback_at")]
        public object CallbackAt { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        public bool TypeMatch(QuickPayResponseConsts thatType)
        {
            return this.Type.Equals(Enum.GetName(typeof(QuickPayResponseConsts), thatType), StringComparison.InvariantCultureIgnoreCase);
        }
    }   
}