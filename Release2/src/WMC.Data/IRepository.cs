using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WMC.Data.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        IEnumerable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "");
        T GetById(long id);
        T GetByIdDetached(long id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void DeleteById(long id);
        TResult Max<TResult>(Func<T, TResult> selector);
        TResult Min<TResult>(Func<T, TResult> selector);
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
