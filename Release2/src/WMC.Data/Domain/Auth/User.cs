namespace WMC.Data.Domain.Auth
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("auth.User")]
    public partial class AuthUser
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AuthUser()
        {
            Tokens = new List<Token>();
        }

        public long UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        public long UserRoleID { get; set; }

        [Required]
        [StringLength(250)]
        public string Email { get; set; }

        [StringLength(25)]
        public string PhoneNumber { get; set; }

        [Required]
        public string EncryptedPassword { get; set; }

        [Required]
        public string PasswordSalt { get; set; }

        public bool IsApproved { get; set; }

        public bool IsLockedOut { get; set; }

        public int PasswordFailuresSinceLastSuccess { get; set; }

        public DateTime CreateDate { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string MiddleName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        public string Comment { get; set; }

        public bool? EmailVerified { get; set; }

        public bool? PhoneVerified { get; set; }

        public DateTimeOffset? LastActivityDate { get; set; }

        public DateTimeOffset? LastLockoutDate { get; set; }

        public DateTimeOffset? LastLoginDate { get; set; }

        public DateTimeOffset? LastPasswordChangedDate { get; set; }

        public DateTimeOffset? LastPasswordFailureDate { get; set; }

        public DateTimeOffset? LastApprovedDate { get; set; }

        public DateTimeOffset? EmailVerifiedDate { get; set; }

        public DateTimeOffset? PhoneVerifiedDate { get; set; }

        public virtual Role Role { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<Token> Tokens { get; set; }
    }
}
