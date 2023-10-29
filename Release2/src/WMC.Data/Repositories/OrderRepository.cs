using System.Data.Entity;
using WMC.Data.Repository.Interfaces;
using WMC.Data;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace WMC.Data.Repositories
{
    public class OrderRepository : DataRepository<Order>, IOrderRepository
    {
        public OrderRepository(DbContext context)
            : base(context)
        {
        }

        public bool OrderNumberExists(long orderNumber)
        {
            return Data.Where(q => q.Number == orderNumber.ToString()).Select(x => x.Number).Any();
        }

        public bool OrderTxSecretExists(long txSecret)
        {
            return Data.Where(q => q.TxSecret == txSecret.ToString()).Any();
        }

        public Order GetOrderAndCurrencyById(long id)
        {
            return Data.Include("Currency").Where(q => q.Id == id).FirstOrDefault();
        }

        public Order GetOrderAndSiteById(long id)
        {
            return Data.Include("Site").Where(q => q.Id == id).FirstOrDefault();
        }

        public Order GetOrderByUserIdentity(Guid userIdentity)
        {
            return Data.Where(q => q.CreditCardUserIdentity == userIdentity).FirstOrDefault();
        }

        public List<Order> GetOrderByUserId(long userId)
        {
            return Data.Where(q => q.UserId == userId).ToList();
        }

        public DateTime GetOldestOrderQuotedDate(string cardNumber)
        {
            return Data.Where(q => q.CardNumber == cardNumber).OrderBy(q => q.Quoted).FirstOrDefault().Quoted.Value;
        }

        public decimal GetOrderDiscount(long id)
        {
            decimal discount = 0;
            try
            {
                var coupon = Data.Where(q => q.Id == id).Include("Coupon").FirstOrDefault().Coupon;
                if (coupon != null)
                    discount = 1 - (coupon.Discount / 100);
                else
                    discount = 1;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetOrderDiscount(" + id + ")", ex);
            }
            return discount;
        }

        public StateLock<Order> LockAndGet(Expression<Func<Order, bool>> filter = null)
        {
            var orderObj = this.Get(filter).FirstOrDefault();
            return LockAndGet(orderObj);
        }

        public StateLock<Order> LockAndGet(long orderId)
        {
            var orderObj = this.GetById(orderId);
            return LockAndGet(orderObj);
        }

        private StateLock<Order> LockAndGet(Order orderObj)
        {
            if (orderObj.IsLocked())
            {
                return new StateLock<Order>(orderObj, false, UnLock);
            }
            else
            {
                // usable
                return LockObject(orderObj, true);
            }
        }

        public bool IsLocked(long orderId)
        {
            return Data.Where(q => q.Id == orderId && q.LockKey != null && q.LockUntil != null && q.LockUntil >= DateTime.UtcNow).Select(x => x.Id).Any();
        }

        public DomainStateLock Lock(long orderId)
        {
            string lockKey;
            if (IsLocked(orderId))
            {
                var order = Data.Where(q => q.Id == orderId && q.LockKey != null && q.LockUntil != null && q.LockUntil >= DateTime.UtcNow).Select(x => new { x.LockKey, x.LockUntil, x.Id }).FirstOrDefault();
                lockKey = order.LockKey;
                return new DomainStateLock(orderId, lockKey, false, UnLock);
            }
            else
            {
                // usable
                return LockById(orderId, true);
            }
        }

        #region Private Helpers
        private DomainStateLock LockById(long orderId, bool usable)
        {
            Order orderUpdate = new Order() { Id = orderId };
            return LockObject(orderUpdate, usable);
        }

        private StateLock<Order> LockObject(Order order, bool usable)
        {
            string lockKey = Guid.NewGuid().ToString("N");
            order.LockKey = lockKey;
            order.LockUntil = DateTime.UtcNow.AddHours(3);
            Update(order);
            SaveChanges();
            return new StateLock<Order>(order, usable, UnLock);
        }

        private void UnLock(long orderId, string key)
        {
            var keyExists = Data.Where(q => q.Id == orderId && q.LockKey == key).FirstOrDefault();
            if (keyExists == null) throw new Exception("Lock missused");
            //Order orderUpdate = new Order() { Id = orderId, LockKey = null, LockUntil = null };
            //Update(orderUpdate);
            keyExists.LockKey = null;
            keyExists.LockUntil = null;
            SaveChanges();
        }
        #endregion
    }
}