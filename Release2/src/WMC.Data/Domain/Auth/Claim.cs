namespace WMC.Data.Domain.Auth
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("auth.Claim")]
    public partial class Claim
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Claim()
        {
            Roles = new List<Role>();
        }

        public long ClaimId { get; set; }

        [Required]
        [StringLength(50)]
        public string ClaimCode { get; set; }

        [Required]
        [StringLength(50)]
        public string ClaimName { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<Role> Roles { get; set; }
    }
}
