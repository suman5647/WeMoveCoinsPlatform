using WMC.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace WMC.Data
{
    public class RepositoryFactories
    {
        private IDictionary<Type, Func<DbContext, object>> GetRepositoryFactories()
        {
            return new Dictionary<Type, Func<DbContext, object>>
            {
                {typeof(CountryRepository), dbContext => new CountryRepository(dbContext)},
                {typeof(OrderRepository), dbContext => new OrderRepository(dbContext)},
                {typeof(UserRepository), dbContext => new UserRepository(dbContext)},
                {typeof(SanctionListRepository), dbContext => new SanctionListRepository(dbContext)},
                {typeof(KycFileRepository), dbContext => new KycFileRepository(dbContext)},
                {typeof(LanguageResourceRepository), dbContext => new LanguageResourceRepository(dbContext)},
                {typeof(MerchantRespository), dbContext => new MerchantRespository(dbContext)}
            };
        }

        public RepositoryFactories()
        {
            _repositoryFactories = GetRepositoryFactories();
        }

        public RepositoryFactories(IDictionary<Type, Func<DbContext, object>> factories)
        {
            _repositoryFactories = factories;
        }

        public Func<DbContext, object> GetRepositoryFactory<T>()
        {
            Func<DbContext, object> factory;
            _repositoryFactories.TryGetValue(typeof(T), out factory);
            return factory;
        }

        public Func<DbContext, object> GetRepositoryFactoryForEntityType<T>() where T : class
        {
            return GetRepositoryFactory<T>() ?? DefaultEntityRepositoryFactory<T>();
        }

        protected virtual Func<DbContext, object> DefaultEntityRepositoryFactory<T>() where T : class
        {
            return dbContext => new DataRepository<T>(dbContext);
        }

        private readonly IDictionary<Type, Func<DbContext, object>> _repositoryFactories;
    }
}