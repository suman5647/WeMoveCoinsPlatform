using WMC.Data.Repositories;

namespace WMC.Data.Repository.Interfaces
{
    public interface IMerchantRespository
    {
        MerchantRepsonse GetMerchantResposne(long orderId);
    }
}
