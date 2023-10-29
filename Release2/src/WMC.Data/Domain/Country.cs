namespace WMC.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using WMC.Data.Enums;

    [Table("Country")]
    public partial class Country
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Country()
        {
            Users = new List<User>();
        }

        public long Id { get; set; }

        [Required]
        [StringLength(2)]
        public string Code { get; set; }

        [Required]
        [StringLength(50)]
        public string Text { get; set; }

        public int? PhoneCode { get; set; }

        public long? CurrencyId { get; set; }

        [StringLength(20)]
        public string PhoneNumberStyle { get; set; }

        [StringLength(10)]
        public string CultureCode { get; set; }

        public decimal? TrustValue { get; set; }

        public bool? AlphaSupport { get; set; }

        [StringLength(200)]
        public string Note { get; set; }

        public decimal? CardFee { get; set; }

        public DateTime? AddTx { get; set; }

        public virtual Currency Currency { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<User> Users { get; set; }

        public CurrencyAcceptance PaymentGateWaysAccepted { get; set; }
    }
}
