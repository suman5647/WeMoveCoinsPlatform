
namespace WMC.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [Table("SanctionsList")]
    public partial class SanctionsList
    {
        public long Id { get; set; }

        public string Name1 { get; set; }

        public string Name2 { get; set; }

        public string Name3 { get; set; }

        public string Name4 { get; set; }

        public string Name5 { get; set; }

        public string Name6 { get; set; }

        public DateTime? DOB { get; set; }

        public string CountryOfResidance { get; set; }

        public string Summary { get; set; }

        public int FromSource { get; set; }

        [NotMapped]
        public string FromSourceValue { get
            {
                return FromSource == 1 ? "EU_SanctionsFiles" : FromSource == 2 ? "HMT_UK_SanctionsFiles" : FromSource == 3 ? "OFAC_SanctionsFiles" : "UN_SanctionsFiles";
            } }


    }
}
