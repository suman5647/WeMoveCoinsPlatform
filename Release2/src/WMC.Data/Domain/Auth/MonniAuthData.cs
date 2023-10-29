namespace WMC.Data.Domain.Auth
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MonniAuthData : DbContext
    {
        public MonniAuthData()
            : base("name=MonniDB")
        {
        }

        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<Claim> Claims { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Token> Tokens { get; set; }
        public virtual DbSet<AuthUser> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Claim>()
                .HasMany(e => e.Roles)
                .WithMany(e => e.Claims)
                .Map(m => m.ToTable("RoleClaim", "auth").MapLeftKey("ClaimId").MapRightKey("RoleId"));

            modelBuilder.Entity<Role>()
                .HasMany(e => e.Users)
                .WithRequired(e => e.Role)
                .HasForeignKey(e => e.UserRoleID);
        }
    }
}
