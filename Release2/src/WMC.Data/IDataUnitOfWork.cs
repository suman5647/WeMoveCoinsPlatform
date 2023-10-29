using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using WMC.Data.Repository.Interfaces;

namespace WMC.Data.Interfaces
{
    public interface IDataUnitOfWork
    {
        void Commit();
        void RefreshContext(RefreshMode refreshMode, DbUpdateConcurrencyException ex);
        //Ie_customerRepository Customers { get; }
        //IRepository<e_product> Products { get; }

        IRepository<Account> Accounts { get; }
        IRepository<AuditTrail> AuditTrails { get; }
        IRepository<AuditTrailStatus> AuditTrailStatus { get; }
        ICountryRepository Countries { get; }
        IRepository<Coupon> Coupons { get; }
        IRepository<Currency> Currencies { get; }
        IKycFileRepository KycFiles { get; }
        IRepository<KycType> KycTypes { get; }
        IRepository<Language> Languages { get; }
        IOrderRepository Orders { get; }
        IRepository<OrderKycfile> OrderKycfiles { get; }
        IRepository<OrderStatus> OrderStatus { get; }
        IRepository<OrderType> OrderTypes { get; }
        IRepository<PaymentType> PaymentTypes { get; }
        IRepository<Site> Sites { get; }
        IRepository<Transaction> Transactions { get; }
        IRepository<TransactionMethod> TransactionMethods { get; }
        IRepository<TransactionType> TransactionTypes { get; }
        IUserRepository Users { get; }
        IRepository<UserRole> UserRoles { get; }
        IRepository<UserType> UserTypes { get; }
        IRepository<AppSetting> AppSettings { get; }
        ISanctionListRepository SanctionsList { get; }
        ILanguageResourceRepository LanguageResource { get; }
        IRepository<Merchant> Merchants { get; }
    }
}