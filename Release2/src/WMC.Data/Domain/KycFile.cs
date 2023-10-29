namespace WMC.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("KycFile")]
    public partial class KycFile
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KycFile()
        {
            OrderKycfiles = new List<OrderKycfile>();
        }

        public long Id { get; set; }

        public long UserId { get; set; }

        public long Type { get; set; }

        public string Note { get; set; }

        [Required]
        [StringLength(200)]
        public string UniqueFilename { get; set; }

        [Required]
        [StringLength(200)]
        public string OriginalFilename { get; set; }

        public DateTime? Requested { get; set; }

        public DateTime? Uploaded { get; set; }

        public DateTime? Rejected { get; set; }

        public long? RejectedBy { get; set; }

        public DateTime? Approved { get; set; }

        public long? ApprovedBy { get; set; }

        public DateTime? Obsolete { get; set; }

        public long? ObsoleteBy { get; set; }

        public string SessionId { get; set; }

        public long? FaceTecStatus { get; set; }

        public virtual KycType KycType { get; set; }

        public virtual User User { get; set; }

        public virtual User KYCApprovedUser { get; set; }

        public virtual User KycObsoleteUser { get; set; }

        public virtual User KycRejectedUser { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<OrderKycfile> OrderKycfiles { get; set; }
    }
}
