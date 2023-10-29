namespace WMC.Data
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MonniData : DbContext
    {
        public MonniData()
            : base("name=MonniDB")
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<AppSetting> AppSettings { get; set; }
        public virtual DbSet<AuditTrail> AuditTrails { get; set; }
        public virtual DbSet<AuditTrailLevel> AuditTrailLevels { get; set; }
        public virtual DbSet<AuditTrailStatus> AuditTrailStatus { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Coupon> Coupons { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<CurrencyType> CurrencyTypes { get; set; }
        public virtual DbSet<KycFile> KycFiles { get; set; }
        public virtual DbSet<KycType> KycTypes { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<LanguageResource> LanguageResources { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderKycfile> OrderKycfiles { get; set; }
        public virtual DbSet<OrderStatus> OrderStatus { get; set; }
        public virtual DbSet<OrderType> OrderTypes { get; set; }
        public virtual DbSet<PaymentType> PaymentTypes { get; set; }
        public virtual DbSet<RiskScoreParameter> RiskScoreParameters { get; set; }
        public virtual DbSet<Site> Sites { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<TransactionMethod> TransactionMethods { get; set; }
        public virtual DbSet<TransactionType> TransactionTypes { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<UserType> UserTypes { get; set; }
        public virtual DbSet<SanctionsList> SanctionsList { get; set; }
        public virtual DbSet<Merchant> Merchants { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .HasMany(e => e.FromAccountTransactions)
                .WithOptional(e => e.FromAccountRef)
                .HasForeignKey(e => e.FromAccount);

            modelBuilder.Entity<Account>()
                .HasMany(e => e.ToAccountTransactions)
                .WithOptional(e => e.ToAccountRef)
                .HasForeignKey(e => e.ToAccount);

            modelBuilder.Entity<AuditTrailStatus>()
                .HasMany(e => e.AuditTrails)
                .WithRequired(e => e.AuditTrailStatusRef)
                .HasForeignKey(e => e.Status)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Country>()
                .Property(e => e.TrustValue)
                .HasPrecision(18, 8);

            modelBuilder.Entity<Country>()
                .Property(e => e.CardFee)
                .HasPrecision(18, 8);

            modelBuilder.Entity<Coupon>()
                .Property(e => e.Discount)
                .HasPrecision(18, 8);

            modelBuilder.Entity<Coupon>()
                .Property(e => e.MinTxnLimit)
                .HasPrecision(18, 8);

            modelBuilder.Entity<Coupon>()
                .Property(e => e.MaxTxnLimit)
                .HasPrecision(18, 8);

            modelBuilder.Entity<Coupon>()
                .Property(e => e.MaxTotalTxnLimit)
                .HasPrecision(18, 8);

            modelBuilder.Entity<Currency>()
                .Property(e => e.FXMarkUp)
                .HasPrecision(18, 8);

            modelBuilder.Entity<Currency>()
                .HasMany(e => e.CryptoCurrencyOrders)
                .WithRequired(e => e.CryptoCurrency)
                .HasForeignKey(e => e.CryptoCurrencyId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Currency>()
                .HasMany(e => e.CurrencyOrders)
                .WithRequired(e => e.Currency)
                .HasForeignKey(e => e.CurrencyId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Currency>()
                .HasMany(e => e.Transactions)
                .WithRequired(e => e.CurrencyRef)
                .HasForeignKey(e => e.Currency)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<KycFile>()
                .HasMany(e => e.OrderKycfiles)
                .WithRequired(e => e.KycFile)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<KycType>()
                .HasMany(e => e.KycFiles)
                .WithRequired(e => e.KycType)
                .HasForeignKey(e => e.Type)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.Rate)
                .HasPrecision(18, 8);

            modelBuilder.Entity<Order>()
                .Property(e => e.Amount)
                .HasPrecision(18, 8);

            modelBuilder.Entity<Order>()
                .Property(e => e.BTCAmount)
                .HasPrecision(18, 8);

            modelBuilder.Entity<Order>()
                .Property(e => e.CommissionProcent)
                .HasPrecision(18, 8);

            modelBuilder.Entity<Order>()
                .Property(e => e.CardNumber)
                .IsFixedLength();

            modelBuilder.Entity<Order>()
                .Property(e => e.RateBase)
                .HasPrecision(18, 8);

            modelBuilder.Entity<Order>()
                .Property(e => e.RateHome)
                .HasPrecision(18, 8);

            modelBuilder.Entity<Order>()
                .Property(e => e.RateBooks)
                .HasPrecision(18, 8);

            modelBuilder.Entity<Order>()
                .Property(e => e.RiskScore)
                .HasPrecision(18, 8);

            modelBuilder.Entity<Order>()
                .Property(e => e.MinersFee)
                .HasPrecision(18, 8);

            modelBuilder.Entity<Order>()
                .Property(e => e.BccAddress)
                .IsUnicode(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.PartnerId)
                .IsUnicode(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.FixedFee)
                .HasPrecision(18, 8);

            modelBuilder.Entity<Order>()
                .Property(e => e.DiscountAmount)
                .HasPrecision(18, 8);

            modelBuilder.Entity<Order>()
                .Property(e => e.OurFee)
                .HasPrecision(18, 8);

            modelBuilder.Entity<Order>()
                .Property(e => e.FxMarkUp)
                .HasPrecision(18, 8);

            modelBuilder.Entity<Order>()
                .Property(e => e.Spread)
                .HasPrecision(18, 8);

            modelBuilder.Entity<Order>()
                .HasMany(e => e.OrderKycfiles)
                .WithRequired(e => e.Order)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OrderStatus>()
                .HasMany(e => e.Orders)
                .WithRequired(e => e.OrderStatus)
                .HasForeignKey(e => e.Status)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OrderType>()
                .HasMany(e => e.Orders)
                .WithRequired(e => e.OrderType)
                .HasForeignKey(e => e.Type)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PaymentType>()
                .HasMany(e => e.Orders)
                .WithRequired(e => e.PaymentTypeRef)
                .HasForeignKey(e => e.PaymentType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RiskScoreParameter>()
                .Property(e => e.Value)
                .HasPrecision(18, 8);

            modelBuilder.Entity<Transaction>()
                .Property(e => e.Amount)
                .HasPrecision(18, 8);

            modelBuilder.Entity<TransactionMethod>()
                .HasMany(e => e.Transactions)
                .WithRequired(e => e.TransactionMethod)
                .HasForeignKey(e => e.MethodId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TransactionType>()
                .HasMany(e => e.Transactions)
                .WithRequired(e => e.TransactionType)
                .HasForeignKey(e => e.Type)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Commission)
                .HasPrecision(18, 8);

            modelBuilder.Entity<User>()
                .HasMany(e => e.KycFiles)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ApprovedKycFiles)
                .WithOptional(e => e.KYCApprovedUser)
                .HasForeignKey(e => e.ApprovedBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ObsoleteKycFiles)
                .WithOptional(e => e.KycObsoleteUser)
                .HasForeignKey(e => e.ObsoleteBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.RejectedKycFiles)
                .WithOptional(e => e.KycRejectedUser)
                .HasForeignKey(e => e.RejectedBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Orders)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ApprovedOrders)
                .WithOptional(e => e.ApprovedUser)
                .HasForeignKey(e => e.ApprovedBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.PredecessorUsers)
                .WithOptional(e => e.PredecessorUser)
                .HasForeignKey(e => e.Predecessor);

            modelBuilder.Entity<User>()
                .HasMany(e => e.TrustedByUsers)
                .WithOptional(e => e.TrustedByUser)
                .HasForeignKey(e => e.TrustedBy);

            modelBuilder.Entity<UserRole>()
                .HasMany(e => e.Users)
                .WithRequired(e => e.UserRole)
                .HasForeignKey(e => e.RoleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserType>()
                .HasMany(e => e.Users)
                .WithRequired(e => e.UserType1)
                .HasForeignKey(e => e.UserType)
                .WillCascadeOnDelete(false);

        }
    }
}
