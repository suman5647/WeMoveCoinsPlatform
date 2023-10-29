namespace WMC.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AuditTrailLevel")]
    public partial class AuditTrailLevel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AuditTrailLevel()
        {
            AuditTrails = new List<AuditTrail>();
        }

        public long Id { get; set; }

        [StringLength(20)]
        public string Text { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<AuditTrail> AuditTrails { get; set; }
    }
}
