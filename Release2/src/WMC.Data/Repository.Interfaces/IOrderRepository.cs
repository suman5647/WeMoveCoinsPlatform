using System.Collections.Generic;
using WMC.Data.Interfaces;
using WMC.Data;
using System;
using System.Linq.Expressions;
//using WMC.Web.Models;

namespace WMC.Data.Repository.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        bool OrderNumberExists(long orderNumber);
        bool OrderTxSecretExists(long txSecret);
        Order GetOrderAndCurrencyById(long id);
        Order GetOrderAndSiteById(long id);
        Order GetOrderByUserIdentity(Guid userIdentity);
        DateTime GetOldestOrderQuotedDate(string cardNumber);
        List<Order> GetOrderByUserId(long userId);
        decimal GetOrderDiscount(long id);
        bool IsLocked(long orderId);
        StateLock<Order> LockAndGet(Expression<Func<Order, bool>> filter = null);
        StateLock<Order> LockAndGet(long orderId);
        DomainStateLock Lock(long orderId);
    }
}
