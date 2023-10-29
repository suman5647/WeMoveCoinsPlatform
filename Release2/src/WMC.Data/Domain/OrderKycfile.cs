namespace WMC.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OrderKycfile")]
    public partial class OrderKycfile
    {
        public long Id { get; set; }

        public long OrderId { get; set; }

        public long KycfileId { get; set; }

        public virtual KycFile KycFile { get; set; }

        public virtual Order Order { get; set; }
    }
}
