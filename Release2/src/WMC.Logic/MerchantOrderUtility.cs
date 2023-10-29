using WMC.Data;
using WMC.Data.Repositories;

namespace WMC.Logic
{
    public class MerchantOrderUtility
    {
        public static MerchantRepsonse GetMerchantOrder(long orderId)
        {
            var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
            return DataUnitOfWork.MerchantsOrder.GetMerchantResposne(orderId);
        }
    }
}
