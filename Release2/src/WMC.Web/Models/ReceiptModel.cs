using System;
using System.Globalization;
using WMC.Data;

namespace WMC.Web.Models
{
    public class ReceiptModel
    {
        public ReceiptModel()
        {
        }

        private CultureInfo culture = new CultureInfo("en-US");
        private long otype = 1;
        public ReceiptModel(string culture = "en-US", long otype = 1)
        {
            if (!string.IsNullOrEmpty(culture)) this.culture = new CultureInfo(culture);
            this.otype = otype;
        }

        public decimal Amount { get; set; }
        public string AmountStr
        {
            get
            {
                return this.Amount.ToString((otype == 2 ? "N8" : "N2"), culture);
            }
            set { }
        }
        public decimal PayoutAmount { get; set; }
        public string PayoutAmountStr
        {
            get
            {
                return this.PayoutAmount.ToString("N2", culture);
            }
            set { }
        }
        public string Currency { get; set; }
        public string CryptoCurrencyCode { get; set; }
        public string SellCurrency { get; set; }
        public decimal Commission { get; set; }
        public string CommissionStr
        {
            get
            {
                return this.Commission.ToString("N2", culture);
            }
            set { }
        }
        public decimal OurFee { get; set; }
        public string OurFeeStr
        {
            get
            {
                return this.OurFee.ToString("N2", culture);
            }
            set { }
        }
        public decimal FixedFee { get; set; } //used only for Bank Sell.
        public string FixedFeeStr
        {
            get
            {
                return this.FixedFee.ToString("N2", culture);
            }
            set { }
        }
        public decimal Rate { get; set; }
        public string RateStr
        {
            get
            {
                return this.Rate.ToString("N2", culture);
            }
            set { }
        }
        public string BitcoinAddress { get; set; }
        public string CardHolderName { get; set; }
        public string CreditCardNumber { get; set; }
        public string TransactionId { get; set; }
        public string TransactionHash { get; set; }
        public string OrderNumber { get; set; }
        public string IBAN { get; set; }
        public string CommissionPercent { get; set; }
        public string GoogleTagManagerId { get; set; }
        public decimal OurFeeValue { get; internal set; }
        public string OurFeeValueStr
        {
            get
            {
                return this.OurFeeValue.ToString("N2", culture);
            }
            set { }
        }
        public decimal CardFee { get; internal set; }
        public string CardFeeStr
        {
            get
            {
                return this.CardFee.ToString("N2", culture);
            }
            set { }
        }
        public decimal MinerFee { get; internal set; }
        public string MinerFeeStr
        {
            get
            {
                return this.MinerFee.ToString("N8", culture);
            }
            set { }
        }

        public string Country { get; internal set; }
        public virtual Coupon Coupon { get; internal set; }
        public long Type { get; internal set; }
        public string paymentMethod { get; set; }
        public string ReferenceId { get; set; }
        public string OrderStatus { get; set; }
        public DateTime? OrderDate { get; set; }
        public long OrderId { get; set; }
    }
}