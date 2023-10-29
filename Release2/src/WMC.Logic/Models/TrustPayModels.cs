using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMC.Logic.Models
{
    public class Result
    {
        public string code { get; set; }
        public string description { get; set; }
    }
    
    public class ResultDetails
    {
        public string AcquirerResponse { get; set; }
        public string ConnectorTxID1 { get; set; }
        public string ConnectorTxID3 { get; set; }
        public string ConnectorTxID2 { get; set; }
        public string ExtendedDescription { get; set; }
    }

    public class Card
    {
        public string bin { get; set; }
        public string last4Digits { get; set; }
        public string holder { get; set; }
        public string expiryMonth { get; set; }
        public string expiryYear { get; set; }
    }

    public class ThreeDSecure
    {
        public string eci { get; set; }
        public string verificationId { get; set; }
        public string xid { get; set; }
        public string paRes { get; set; }
    }

    public class CustomParameters
    {
        public string SHOPPER_EndToEndIdentity { get; set; }
        public string CTPE_DESCRIPTOR_TEMPLATE { get; set; }
        public string SHOPPER_OrderNumber { get; set; }
    }

    public class Risk
    {
        public string score { get; set; }
    }

    public class CheckOutResponse
    {
        public Result result { get; set; }
        public string buildNumber { get; set; }
        public string timestamp { get; set; }
        public string ndc { get; set; }
        public string id { get; set; }
    }

    public class PaymentStatus
    {
        public string id { get; set; }
        public string paymentType { get; set; }
        public string paymentBrand { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string descriptor { get; set; }
        public Result result { get; set; }
        public ResultDetails resultDetails { get; set; }
        public Card card { get; set; }
        public ThreeDSecure threeDSecure { get; set; }
        public CustomParameters customParameters { get; set; }
        public Risk risk { get; set; }
        public string buildNumber { get; set; }
        public string timestamp { get; set; }
        public string ndc { get; set; }
    }

    public class Payment
    {
        public string id { get; set; }
        public string paymentType { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string descriptor { get; set; }
        public Result result { get; set; }
        public ResultDetails resultDetails { get; set; }
        public string buildNumber { get; set; }
        public string timestamp { get; set; }
        public string ndc { get; set; }
    }

    public class TrustPayPaymentModel
    {
        public string CheckoutId { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string CardHolderName { get; set; }
    }
}
