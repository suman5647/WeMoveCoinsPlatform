namespace WMC.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Coupon")]
    public partial class Coupon
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Coupon()
        {
            Orders = new List<Order>();
        }

        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string CouponCode { get; set; }

        [Required]
        [StringLength(150)]
        public string Description { get; set; }

        public decimal Discount { get; set; }

        public long? MaxTxnCount { get; set; }

        public decimal? MinTxnLimit { get; set; }

        public decimal? MaxTxnLimit { get; set; }

        public decimal? MaxTotalTxnLimit { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        [StringLength(10)]
        public string Region { get; set; }

        [StringLength(10)]
        public string CryptoCurrency { get; set; }

        [StringLength(10)]
        public string Type { get; set; }

        [StringLength(50)]
        public string ReferredBy { get; set; }

        public bool? IsActive { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<Order> Orders { get; set; }
    }
}
