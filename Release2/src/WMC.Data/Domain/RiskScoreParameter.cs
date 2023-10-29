namespace WMC.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RiskScoreParameter")]
    public partial class RiskScoreParameter
    {
        public long Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Code { get; set; }

        public string Description { get; set; }

        public decimal Value { get; set; }
    }
}
