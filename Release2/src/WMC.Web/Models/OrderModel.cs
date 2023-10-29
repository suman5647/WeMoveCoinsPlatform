using System;
using System.Collections.Generic;
using System.Globalization;

namespace WMC.Web.Models
{
    public class OrderModel
    {
        public OrderSizeBoundary OrderSizeBoundary { get; set; }
        public decimal? BuyAmount { get; set; }
        public string Commission { get; set; }
        public decimal? CardFee { get; set; }
        public string ExpectedRate { get; set; }
        public string ApproximateReceivableBTC { get; set; }
        public string Btc2LocalCurrency { get; internal set; }
        public decimal Btc2LocalCurrencyNumeric { get; internal set; }
        public decimal Btc2SellCurrencyNumeric { get; internal set; }
        public string Btc2LocalBuyTicker { get; internal set; }
        public decimal Btc2LocalCurrencyBuyNumeric { get; internal set; }
        public string Btc2LocalSellTicker { get; internal set; }
        public decimal Btc2LocalCurrencySellNumeric { get; internal set; }
        public string CultureCode { get; set; }
        public List<Tuple<string, int>> Currencies { get; set; }
        public List<string> DigitalCurrencies { get; set; }
        public string ForCurrency { get; set; }
        public string ForSellCurrency { get; set; }
        public string ForBuyCCCurrency { get; set; }
        public String ForDigitalCurrency { get; set; }
        public PaymentBoundaryDetails PaymentMethodDetail { get; set; }
        public PaymentBoundaryDetails SellPaymentMethodDetail { get; set; }
        public CommissionMethod CommissionMethodDetail { get; set; }
        public string BitcoinAddress { get; set; }
        public string FullName { get; set; }
        public string EMail { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PhoneCode { get; set; }
        public long PhoneCodeId { get; set; }
        public List<Tuple<long, string, string, string, int>> PhoneCodes { get; set; }
        public string Mobile { get; set; }
        public string MobileNumberFormat { get; set; }
        public string ReturnUrl { get; set; }
        public PaymentMethod BuyPaymentMethods { get; set; }
        public PaymentMethod SellPaymentMethods { get; set; }
        public decimal MinersFee { get; set; }
        public decimal EuroBtcRate { get; set; }
        public decimal EuroCurrencyRate { get; set; }
        public string BccAddress { get; set; }
        public string PartnerId { get; set; }
        public bool Compact { get; set; }
        public bool SkipOrderForm { get; set; } = false;
        public bool ShowOnlyTopOfOrderForm { get; set; } = false;
        public bool ShowOnlyBottomOfOrderForm { get; set; } = false;
        public int Type { get; set; } = 1;
        public string OperationalStatus { get; set; }
        public string ReCaptchaStatus { get; set; }
        public string ReCaptchaPublicKey { get; set; }
        public string ReCaptchaPrivateKey { get; set; }
        public string CouponCode { get; set; }
        public string Reg { get; set; }
        public string IBAN { get; set; }
        public string SwiftCode { get; set; }
        public string AccountNumber { get; set; }
        public ReceiptModel ReceiptModel { get; set; }
        public string SellBankCurrency { get; set; }
        public bool IsBitgoTest { get; set; }
    }

    //[Obsolete]
    //public class Method
    //{
    //    public string Name { get; set; }
    //    public string DisplayName { get; set; }
    //    public string Fee { get; set; }
    //    public decimal? FixedFee { get; set; }
    //    public decimal? Commission { get; set; }
    //    public OrderSizeBoundary OrderSizeBoundary { get; set; }
    //}
    public class CommissionMethod
    {
        public string Name { get; set; }
        public int SellSpread { get; set; }
        public int BuySpread { get; set; }
        public double Sellcommission { get; set; }
        public int Buycommission { get; set; }
    }
    public class CommissionMethodDetail
    {
        public long SiteId { get; set; }
        public List<CommissionMethod> Methods { get; set; }
    }
    //[Obsolete]
    //public class PaymentMethodDetail
    //{
    //    public long SiteId { get; set; }
    //    public decimal Spread { get; set; }
    //    public List<Method> Methods { get; set; }
    //}
    public class PaymentBoundaryDetails
    {
        public decimal Spread { get; set; }
        public MethodsCommission BankCommission { get; set; }
        public MethodsCommission CCCommission { get; set; }
        public List<PaymentMethod> PaymentMethods { get; set; }
        public OrderSizeBoundary CCOrderSizeBoundary { get; set; }
        /// <summary>
        /// Key represent Currency and Value represent boundaries
        /// </summary>
        public Dictionary<string, OrderSizeBoundary> Boundaries { get; set; }
    }

    public class MethodsCommission
    {
        public string Name { get; set; }
        public string Fee { get; set; }
        public decimal? FixedFee { get; set; }
        public decimal? Commission { get; set; }
    }
    public class CurrencyBoundary
    {
        public OrderSizeBoundary BankOrderSizeBoundary { get; set; }
        public OrderSizeBoundary CCOrderSizeBoundary { get; set; }
    }
    /// <summary>
    /// Values will be always in base currency
    /// </summary>
    public class OrderSizeBoundary
    {
        private string culture = "en-US";
        private string numberFormat = "N2";
        public OrderSizeBoundary(string culture = "en-US", string numberFormat = "N2")
        {
            this.culture = culture;
            this.numberFormat = numberFormat;
        }

        public OrderSizeBoundary()
        {

        }

        public string MinStr
        {
            get
            {
                return this.Min.ToString(this.numberFormat, new CultureInfo(this.culture));
            }
            set { }
        }
        public string MaxStr
        {
            get
            {
                return this.Max.ToString(this.numberFormat, new CultureInfo(this.culture));
            }
            set { }
        }

        public decimal Min { get; set; }
        public decimal Max { get; set; }
    }

    public class TransactionLimitsDetails
    {
        public int DayTransactionLimit { get; set; }
        public int PerTransactionAmountLimit { get; set; }
        public int DayTransactionAmountLimit { get; set; }
        public int MonthTransactionAmountLimit { get; set; }
    }


    public class PaymentMethod
    {
        public string Name { get; set; }
        /// <summary>
        /// Localized name
        /// </summary>
        public string DisplayName { get; set; }
    }

}