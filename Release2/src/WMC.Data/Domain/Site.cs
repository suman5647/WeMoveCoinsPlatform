namespace WMC.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Site")]
    public partial class Site
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Site()
        {
            Orders = new List<Order>();
        }

        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Text { get; set; }

        [StringLength(50)]
        public string Url { get; set; }

        public long? CurrencyId { get; set; }

        [StringLength(20)]
        public string GoogleTagManagerId { get; set; }

        public string SMTPServerSettings { get; set; }

        [StringLength(50)]
        public string TrustPilotAddress { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<Order> Orders { get; set; }
    }
}
