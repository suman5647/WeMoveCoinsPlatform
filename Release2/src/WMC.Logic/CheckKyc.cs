using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMC.Data;

namespace WMC.Logic
{
    public class CheckKyc
    {
        public static bool isKYCApproved(long orderId)
        {
            var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));

            var order = DataUnitOfWork.Orders.GetById(orderId);
            var user = DataUnitOfWork.Users.GetById(order.UserId);

            //check one of the PhotoId is approved
            bool type1KycFiles = DataUnitOfWork.KycFiles.CheckKycFile(user.Id, (long)WMC.Data.Enums.KYCFileTypes.PhotoID);
            //check one of the SelfieID is approved
            bool type4KycFiles = DataUnitOfWork.KycFiles.CheckKycFile(user.Id, (long)WMC.Data.Enums.KYCFileTypes.SelfieID);

            if(order.Type == (int)Data.Enums.OrderPaymentType.Bank)
            {
                if (type1KycFiles)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            if (order.Type == (int)Data.Enums.OrderPaymentType.CreditCard)
            {
                if (type1KycFiles && type4KycFiles)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }
}
