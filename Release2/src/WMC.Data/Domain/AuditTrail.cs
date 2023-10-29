namespace WMC.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AuditTrail")]
    public partial class AuditTrail
    {
        public long Id { get; set; }

        public long? OrderId { get; set; }

        public long Status { get; set; }

        public string Message { get; set; }

        public DateTime Created { get; set; }

        public long? AuditTrailLevelId { get; set; }

        public virtual AuditTrailLevel AuditTrailLevel { get; set; }

        public virtual AuditTrailStatus AuditTrailStatusRef { get; set; }

        public virtual Order Order { get; set; }
    }
}
