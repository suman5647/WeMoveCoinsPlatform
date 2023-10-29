using WMC.Data.Interfaces;
using System;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using WMC.Data.Repositories;
using WMC.Data.Repository.Interfaces;
using System.Data.Entity.Validation;
using System.Text;
using System.Data.Entity;
using System.Data;

namespace WMC.Data
{
    public static class DbContextEx
    {
        public static int SaveChangesWithErrors(this DbContext context)
        {
            try
            {
                return context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException(
                    "Entity Validation Failed - errors follow:\n" +
                    sb.ToString(), ex
                ); // Add the original exception as the innerException
            }
        }
    }

    public class DataUnitOfWork : IDataUnitOfWork, IDisposable
    {
        public DataUnitOfWork(IRepositoryProvider repositoryProvider)
        {
            CreateDbContext();
            repositoryProvider.DbContext = DbContext;
            RepositoryProvider = repositoryProvider;
        }

        private IDbTransaction dbTransaction = default;
        public DataUnitOfWork(IRepositoryProvider repositoryProvider, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            CreateDbContext();
            repositoryProvider.DbContext = DbContext;
            RepositoryProvider = repositoryProvider;
            this.dbTransaction = BeginTransaction(isolationLevel);
        }

        private IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            var transaction = RepositoryProvider.DbContext.Database.BeginTransaction(isolationLevel);
            var db_transaction = transaction.UnderlyingTransaction;
            return db_transaction;
        }

        public string ConnectionString { get; set; }

        // Data repositories
        //public Ie_customerRepository Customers { get { return GetRepo<e_customerRepository>(); } }
        //public IRepository<e_product> Products { get { return GetStandardRepo<e_product>(); } }

        /// <summary>
        /// Save pending changes to the database
        /// </summary>
        public void Commit()
        {
            if (dbTransaction != default)
            {
                // DbContext.SaveChangesWithErrors();
                dbTransaction.Commit();
            }
            else
            {
                DbContext.SaveChangesWithErrors();
            }
        }

        public void Rollback()
        {
            if (dbTransaction != default)
            {
                // DbContext.SaveChangesWithErrors();
                dbTransaction.Rollback();
            }
        }

        public void RefreshContext(RefreshMode refreshMode, DbUpdateConcurrencyException ex)
        {
            var objContext = ((IObjectContextAdapter)DbContext).ObjectContext;
            // Get failed entry
            //var entry = ex.Entries.Single();
            // Now call refresh on ObjectContext
            objContext.Refresh(refreshMode, ex.Entries.Select(e => e.Entity));
        }

        protected void CreateDbContext()
        {
            DbContext = new MonniData();

            //if (!DbContext.Database.CompatibleWithModel(false))
            //    throw new Exception("Database In Incompatible with Model, Please run Migration or Create");

            // Do NOT enable proxied entities, else serialization fails
            DbContext.Configuration.ProxyCreationEnabled = false;

            // Load navigation properties explicitly (avoid serialization trouble)
            DbContext.Configuration.LazyLoadingEnabled = false;

            // Because Web API will perform validation, we don't need/want EF to do so
            //DbContext.Configuration.ValidateOnSaveEnabled = false;

            //DbContext.Configuration.AutoDetectChangesEnabled = false;
            // We won't use this performance tweak because we don't need 
            // the extra performance and, when autodetect is false,
            // we'd have to be careful. We're not being that careful.
        }

        protected IRepositoryProvider RepositoryProvider { get; set; }

        private IRepository<T> GetStandardRepo<T>() where T : class
        {
            return RepositoryProvider.GetRepositoryForEntityType<T>();
        }
        private T GetRepo<T>() where T : class
        {
            return RepositoryProvider.GetRepository<T>();
        }

        private MonniData DbContext { get; set; }
        public IRepository<Account> Accounts { get { return GetStandardRepo<Account>(); } }
        public IRepository<AuditTrail> AuditTrails { get { return GetStandardRepo<AuditTrail>(); } }
        public IRepository<AuditTrailStatus> AuditTrailStatus { get { return GetStandardRepo<AuditTrailStatus>(); } }
        public ICountryRepository Countries { get { return GetRepo<CountryRepository>(); } }
        public IRepository<Coupon> Coupons { get { return GetStandardRepo<Coupon>(); } }
        public IRepository<Currency> Currencies { get { return GetStandardRepo<Currency>(); } }
        public IKycFileRepository KycFiles { get { return GetRepo<KycFileRepository>(); } }
        public IRepository<KycType> KycTypes { get { return GetStandardRepo<KycType>(); } }
        public IRepository<Language> Languages { get { return GetStandardRepo<Language>(); } }
        public IOrderRepository Orders { get { return GetRepo<OrderRepository>(); } }
        public IRepository<OrderStatus> OrderStatus { get { return GetStandardRepo<OrderStatus>(); } }
        public IRepository<OrderType> OrderTypes { get { return GetStandardRepo<OrderType>(); } }
        public IRepository<PaymentType> PaymentTypes { get { return GetStandardRepo<PaymentType>(); } }
        public IRepository<Site> Sites { get { return GetStandardRepo<Site>(); } }
        public IRepository<Transaction> Transactions { get { return GetStandardRepo<Transaction>(); } }
        public IRepository<TransactionMethod> TransactionMethods { get { return GetStandardRepo<TransactionMethod>(); } }
        public IRepository<TransactionType> TransactionTypes { get { return GetStandardRepo<TransactionType>(); } }
        public IUserRepository Users { get { return GetRepo<UserRepository>(); } }
        public IRepository<UserRole> UserRoles { get { return GetStandardRepo<UserRole>(); } }
        public IRepository<UserType> UserTypes { get { return GetStandardRepo<UserType>(); } }
        public IRepository<AppSetting> AppSettings { get { return GetStandardRepo<AppSetting>(); } }
        public IRepository<OrderKycfile> OrderKycfiles { get { return GetStandardRepo<OrderKycfile>(); } }
        public ISanctionListRepository SanctionsList { get { return GetRepo<SanctionListRepository>(); } }

        public ILanguageResourceRepository LanguageResource { get { return GetRepo<LanguageResourceRepository>(); } }

        public IRepository<Merchant> Merchants { get { return GetStandardRepo<Merchant>(); } }  //IMerchantRespository

        public IMerchantRespository MerchantsOrder { get { return GetRepo<MerchantRespository>(); } }
        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.dbTransaction != null)
                {
                    this.dbTransaction.Dispose();
                }

                if (DbContext != null)
                {
                    DbContext.Dispose();
                }
            }
        }
        #endregion
    }
}
