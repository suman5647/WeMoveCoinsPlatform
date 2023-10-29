namespace WMC.Data.Domain.Auth
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("auth.Application")]
    public partial class Application
    {
        public long Id { get; set; }

        public string Description { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Password_RequiredParameter1 { get; set; }

        [Required]
        public string Password_RequiredParameter2 { get; set; }

        [Required]
        public string Password_RequiredParameter3 { get; set; }

        [Required]
        public string Password_RequiredParameter4 { get; set; }

        public long Password_RequiredMinimumLength { get; set; }

        public long Password_RequiredMaximumLength { get; set; }

        public long Password_ResetTokenExpirationMinutes { get; set; }

        public long Login_MaximumInvalidAttempts { get; set; }

        public long Login_UnlockDurationMinutes { get; set; }

        public long UserName_RequiredMinimumLength { get; set; }

        public long UserName_RequiredMaximumLength { get; set; }
    }
}
