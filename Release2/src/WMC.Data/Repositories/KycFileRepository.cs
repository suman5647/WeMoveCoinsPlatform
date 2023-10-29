using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMC.Data.Repository.Interfaces;


namespace WMC.Data.Repositories
{
    public class KycFileRepository : DataRepository<KycFile>, IKycFileRepository
    {
        public KycFileRepository(DbContext context)
            : base(context)
        {
        }

        public bool CheckKycFile(long userId, long type)
        {
            var kycFiles = Data.Where(q => q.UserId == userId && q.Type == type);
            if (kycFiles.Count() > 0)
            {
                if (kycFiles.Any(q => q.Approved.HasValue))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}
