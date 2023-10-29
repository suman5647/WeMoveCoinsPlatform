namespace WMC.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using WMC.Data.Enums;

    [Table("User")]
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            KycFiles = new List<KycFile>();
            ApprovedKycFiles = new List<KycFile>();
            ObsoleteKycFiles = new List<KycFile>();
            RejectedKycFiles = new List<KycFile>();
            Orders = new List<Order>();
            ApprovedOrders = new List<Order>();
            PredecessorUsers = new List<User>();
            TrustedByUsers = new List<User>();
        }

        public long Id { get; set; }

        public long RoleId { get; set; }

        public long UserType { get; set; }

        [StringLength(30)]
        public string Login { get; set; }

        [NotMapped]
        public string PasswordText { get; set; }
        [StringLength(256)]
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        [Required]
        [StringLength(50)]
        public string Fname { get; set; }

        [StringLength(30)]
        public string Lname { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        public long? PhoneVerificationCode { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(30)]
        public string Address { get; set; }

        [StringLength(30)]
        public string Address2 { get; set; }

        [StringLength(10)]
        public string Zip { get; set; }

        [StringLength(30)]
        public string City { get; set; }

        [StringLength(30)]
        public string Region { get; set; }

        public long? CountryId { get; set; }

        public long? LanguageId { get; set; }

        public decimal? Commission { get; set; }

        public string KycNote { get; set; }

        public DateTime Created { get; set; }

        public bool? Newsletter { get; set; }

        public string PaymentMethodDetails { get; set; }

        public long? Predecessor { get; set; }

        public DateTime? Trusted { get; set; }

        public long? TrustedBy { get; set; }

        public bool? Blocked { get; set; }

        [StringLength(50)]
        public string BlockedBy { get; set; }

        public CustomerTier Tier { get; set; }

        public DateTime? TierTwoApproved { get; set; }

        public long? TierTwoApprovedBy { get; set; }

        public DateTime? TierThreeApproved { get; set; }

        public long? TierThreeApprovedBy { get; set; }

        

        public string TransactionLimitsDetails { get; set; }

        public string CreditCardLimitsDetails { get; set; }

        public UserRiskLevelType UserRiskLevel { get; set; }

        public string SellPaymentMethodDetails { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? SanctionListChecked { get; set; }
        public DateTime? SanctionListCheckedForDob { get; set; }
        public virtual Country Country { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<KycFile> KycFiles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<KycFile> ApprovedKycFiles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<KycFile> ObsoleteKycFiles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<KycFile> RejectedKycFiles { get; set; }

        public virtual Language Language { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<Order> Orders { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<Order> ApprovedOrders { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<User> PredecessorUsers { get; set; }

        public virtual User PredecessorUser { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<User> TrustedByUsers { get; set; }

        public virtual User TrustedByUser { get; set; }

        public virtual UserRole UserRole { get; set; }

        public virtual UserType UserType1 { get; set; }

       
    }
}
