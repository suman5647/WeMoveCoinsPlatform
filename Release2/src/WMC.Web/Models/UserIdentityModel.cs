using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMC.Web.Models
{
    public class UserSessionModel
    {
        public DateTime? DateOfBirth;

        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string IPAddress { get; set; }
        public string CryptoAddress { get; set; }
        public decimal OrderAmount { get; set; }
        public long CurrencyId { get; set; }
        public long CryptoCurrencyId { get; set; }
        public string CultureInfo { get; set; }
        public decimal CommissionPercent { get; set; }
        public decimal? OurFee { get; set; }
        public decimal Btc2LocalCurrencyRate { get; set; }
        public string KycRequirement { get; set; }
        public bool PhoneNumberVerified { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentResponse { get; set; }
        public long OrderId { get; set; }
        public Guid PaymentValidationKey { get; set; }
        public long Type { get; set; }
        public string CouponCode { get; set; }
        public decimal CouponDiscount { get; set; }
        public decimal? FixedFee { get; set; }
        public int PhoneVerificationCodeCounter { get; set; }
        public string PaymentId { get; set; }
    }
}