namespace WMC.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Order")]
    public partial class Order : ILockDomain//: OrderWFObject
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            AuditTrails = new List<AuditTrail>();
            OrderKycfiles = new List<OrderKycfile>();
            Transactions = new List<Transaction>();
        }

        public long Id { get; set; }

        [Required]
        [StringLength(8)]
        public string Number { get; set; }
        public long UserId { get; set; }

        [NotMapped]
        private long _status;
        // public DateTime? DateOfBirth { get; set; }

        public long Status
        {
            get { return _status; }
            set
            {
                if (((this as ILockDomain).Locker != null && (this as ILockDomain).Locker.Usable && (this as ILockDomain).Locker.LockKey == this.LockKey))
                {

                }
                else if (this.IsLocked())
                {
                    throw new Exception("Order is locked for modification");
                }

                _status = value;
            }
        }
        [NotMapped]
        DomainStateLock ILockDomain.Locker { get; set; }

        public long Type { get; set; }

        public long PaymentType { get; set; }

        public string RequestInfo { get; set; }

        public DateTime? TermsIsAgreed { get; set; }

        public DateTime? Quoted { get; set; }

        public decimal? Rate { get; set; }

        [StringLength(50)]
        public string QuoteSource { get; set; }

        public decimal? Amount { get; set; }

        public decimal? BTCAmount { get; set; }

        public long CurrencyId { get; set; }

        public decimal? CommissionProcent { get; set; }

        [StringLength(20)]
        public string CardNumber { get; set; }

        [StringLength(50)]
        public string CryptoAddress { get; set; }

        [StringLength(35)]
        public string AccountNumber { get; set; }

        [StringLength(11)]
        public string SwiftBIC { get; set; }

        [StringLength(35)]
        public string RecieverName { get; set; }

        [StringLength(20)]
        public string RecieverRef { get; set; }

        [StringLength(35)]
        public string RecieverText { get; set; }

        [StringLength(3)]
        public string CurrencyCode { get; set; }

        [StringLength(2)]
        public string WireType { get; set; }

        [StringLength(1)]
        public string WireCost { get; set; }

        public string Note { get; set; }

        [StringLength(50)]
        public string ExtRef { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(20)]
        public string IP { get; set; }

        public string TransactionHash { get; set; }

        public long? SiteId { get; set; }

        [StringLength(50)]
        public string PaymentGatewayType { get; set; }

        public decimal? RateBase { get; set; }

        public decimal? RateHome { get; set; }

        public decimal? RateBooks { get; set; }

        public DateTime? Approved { get; set; }

        public long? ApprovedBy { get; set; }

        [StringLength(100)]
        public string CountryCode { get; set; }

        [StringLength(4)]
        public string TxSecret { get; set; }

        public DateTime? CardApproved { get; set; }

        public decimal? RiskScore { get; set; }

        [StringLength(2)]
        public string IpCode { get; set; }

        public Guid? CreditCardUserIdentity { get; set; }

        public long? TxSecrectVerificationAttempts { get; set; }

        public string Referrer { get; set; }

        public string Origin { get; set; }

        public decimal? MinersFee { get; set; }

        [StringLength(50)]
        public string BccAddress { get; set; }

        [StringLength(50)]
        public string PartnerId { get; set; }

        public bool? Direction { get; set; }

        [StringLength(100)]
        public string IBAN { get; set; }

        [StringLength(50)]
        public string Reg { get; set; }

        [StringLength(50)]
        public string CCRG { get; set; }

        public long? CouponId { get; set; }

        public decimal? FixedFee { get; set; }

        public decimal? DiscountAmount { get; set; }

        public decimal? OurFee { get; set; }

        public decimal? FxMarkUp { get; set; }

        public decimal? Spread { get; set; }

        public long CryptoCurrencyId { get; set; }
        public string LockKey { get; set; }
        public DateTime? LockUntil { get; set; }

        public string ReferenceId { get; set; }

        public string MerchantCode { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<AuditTrail> AuditTrails { get; set; }

        public virtual Coupon Coupon { get; set; }

        public virtual Currency CryptoCurrency { get; set; }

        public virtual Currency Currency { get; set; }

        public virtual OrderType OrderType { get; set; }

        public virtual PaymentType PaymentTypeRef { get; set; }

        public virtual Site Site { get; set; }

        public virtual OrderStatus OrderStatus { get; set; }

        public virtual User User { get; set; }

        public virtual User ApprovedUser { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<OrderKycfile> OrderKycfiles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<Transaction> Transactions { get; set; }

    }
}
