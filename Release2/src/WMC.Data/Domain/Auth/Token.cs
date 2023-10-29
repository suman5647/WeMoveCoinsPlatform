namespace WMC.Data.Domain.Auth
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("auth.Token")]
    public partial class Token
    {
        public long TokenId { get; set; }

        public long UserId { get; set; }

        [Required]
        public string JwtToken { get; set; }

        [Required]
        public string TokenType { get; set; }

        public string Scope { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public virtual AuthUser User { get; set; }
    }
}
