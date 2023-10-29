using System;
using System.Data.Entity;
using System.Linq;
using WMC.Data.Repository.Interfaces;
using WMC.Data;

namespace WMC.Data.Repositories
{
    public class UserRepository : DataRepository<User>, IUserRepository
    {
        public UserRepository(DbContext context)
            : base(context)
        {
        }

        public User GetUserByPhoneNumber(string phoneNumber)
        {
            //var user = Data.Include("Country").Include("Country.Currency").Include("KycFiles").Include("KycFiles.KycType").Where(q => q.Phone == phoneNumber).FirstOrDefault();
            var user = Data.Include("Country").Include("Country.Currency").Where(q => q.Phone == phoneNumber).FirstOrDefault();
            if (user == null)
                throw new Exception("User with phonenumber'" + phoneNumber + "' does not exist in the system. Please contact the administrator.");
            return user;
        }

        //public User GetUserByPhoneNumber(string phoneNumber)
        //{
        //    var user = Data.Include("Orders").Include("Orders.Currency").Include("Country").Include("Country.Currency").Include("KycFiles").Include("KycFiles.KycType").Where(q => q.Phone == phoneNumber).FirstOrDefault();
        //    if (user == null)
        //        throw new Exception("User with phonenumber'" + phoneNumber + "' does not exist in the system. Please contact the administrator.");
        //    return user;
        //}
    }
}