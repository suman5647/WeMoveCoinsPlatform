using WMC.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMC.Data
{
    public class DataRepository<T> : IRepository<T> where T : class
    {
        protected DbContext Context { get; set; }
        protected DbSet<T> Data { get; set; }
        public DataRepository(DbContext dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException("dbContext");
            Context = dbContext;
            Data = Context.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return Data;
        }

        public IEnumerable<T> Get(System.Linq.Expressions.Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            IQueryable<T> query = Data;

            if (filter != null)
                query = query.Where(filter);

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(includeProperty);

            if (orderBy != null)
                return orderBy(query).ToList();
            else
                return query.ToList();
        }

        public T GetById(long id)
        {
            return Data.Find(id);
        }

        public T GetByIdDetached(long id)
        {
            var entity = Data.Find(id);
            DbEntityEntry dbEntityEntry = Context.Entry(entity);
            if (dbEntityEntry.State != EntityState.Detached)
                dbEntityEntry.State = EntityState.Detached;
            return entity;
        }

        public void Add(T entity)
        {
            DbEntityEntry dbEntityEntry = Context.Entry(entity);
            if (dbEntityEntry.State != EntityState.Detached)
                dbEntityEntry.State = EntityState.Added;
            else
                Data.Add(entity);
        }

        public void Update(T entity)
        {
            DbEntityEntry dbEntityEntry = Context.Entry(entity);
            if (dbEntityEntry.State == EntityState.Detached)
                Data.Attach(entity);
            dbEntityEntry.State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            DbEntityEntry dbEntityEntry = Context.Entry(entity);
            if (dbEntityEntry.State != EntityState.Deleted)
                dbEntityEntry.State = EntityState.Deleted;
            else
            {
                Data.Attach(entity);
                Data.Remove(entity);
            }
        }

        public void DeleteById(long id)
        {
            var entity = GetById(id);
            if (entity == null) return; // not found; assume already deleted.
            Delete(entity);
        }

        public TResult Max<TResult>(Func<T, TResult> selector)
        {
            return Data.Select(selector).Max();
        }

        public TResult Min<TResult>(Func<T, TResult> selector)
        {
            return Data.Select(selector).Min();
        }

        public int SaveChanges()
        {
            return Context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await Context.SaveChangesAsync();
        }
    }
}