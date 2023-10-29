namespace WMC.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LanguageResources")]
    public partial class LanguageResource
    {
        public long Id { get; set; }

        [Required]
        [StringLength(60)]
        public string Key { get; set; }

        [Required]
        [StringLength(1000)]
        public string Value { get; set; }

        [Required]
        [StringLength(10)]
        public string Language { get; set; }

        public string Usages { get; set; }

        public string ValueParams { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
