namespace WMC.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AppSettings")]
    public partial class AppSetting
    {
        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ConfigKey { get; set; }

        [Required]
        public string ConfigValue { get; set; }

        [StringLength(100)]
        public string ConfigDescription { get; set; }
        public bool IsEncrypted { get; set; }
    }
}
