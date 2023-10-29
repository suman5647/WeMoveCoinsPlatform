using WMC.Data.Interfaces;
using WMC.Data;

namespace WMC.Data.Repository.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        User GetUserByPhoneNumber(string phoneNumber);
    }
}
