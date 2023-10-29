using System;
using System.Collections.Generic;
using WMC.Data.Interfaces;
using WMC.Data;

namespace WMC.Data.Repository.Interfaces
{
    public interface IKycFileRepository : IRepository<KycFile>
    {
        bool CheckKycFile(long userId, long type);
    }
}
