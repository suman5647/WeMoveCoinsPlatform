using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMC.Data.Repository.Interfaces;

namespace WMC.Data.Repositories
{
    public class MerchantRespository : DataRepository<MerchantOrderModel>, IMerchantRespository
    {
        public MerchantRespository(DbContext context)
           : base(context)
        {
        }
        private  MerchantOrderModel GetMerchantOrder(long orderId)
        {
            var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
            //Get Order details
            var getOrder = DataUnitOfWork.Orders.Get(q => q.Id == orderId).Select(x =>
            new MerchantOrderModel
            {
                Id = x.Id,
                Quoted= x.Quoted,
                Rate = x.Rate,
                Amount= x.Amount,
                OrderStatus = DataUnitOfWork.OrderStatus.GetById(x.Status).Text,
                ReferenceId= x.ReferenceId,
            }).FirstOrDefault();

            return getOrder;
        }

        private string GetMerchantTransaction(long orderId)
        {
            var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
            //Get Transaction Details
            var getTx = DataUnitOfWork.Transactions.Get(q => q.OrderId == orderId).Select(p => p.ExtRef).FirstOrDefault();
            return getTx;
        }

        public MerchantRepsonse GetMerchantResposne(long orderId)
        {
            var getOrder = GetMerchantOrder(orderId);
            var getTx = GetMerchantTransaction(orderId);
            MerchantRepsonse results = new MerchantRepsonse
            {
                Id = getOrder.Id,
                Quoted = getOrder.Quoted,
                Rate = getOrder.Rate,
                Amount = getOrder.Amount,
                OrderStatus = getOrder.OrderStatus,
                ReferenceId = getOrder.ReferenceId,
                ExtRef = getTx
            };
            return results;
        }
    }

    public class MerchantRepsonse
    {
        public long Id { get; set; }

        public DateTime? Quoted { get; set; }

        public decimal? Rate { get; set; }

        public decimal? Amount { get; set; }

        public string OrderStatus { get; set; }

        public string ReferenceId { get; set; }

        public string ExtRef { get; set; }
    }


    public class MerchantOrderModel
    {
        public long Id { get; set; }

        public DateTime? Quoted { get; set; }

        public decimal? Rate { get; set; }

        public decimal? Amount { get; set; }

        public string OrderStatus { get; set; }

        public string ReferenceId { get; set; }
    }
}
