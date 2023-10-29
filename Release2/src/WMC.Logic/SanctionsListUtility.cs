using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using WMC.Data;
using WMC.Logic;

namespace WMC.Logic
{
    public class SanctionsListUtility
    {
        public static IQueryable<SanctionsList> SearchByName(string userName) 
        {
            var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));            
            return DataUnitOfWork.SanctionsList.GetSanctionsLists(userName);
        }

        public static SanctionsList SearchByDob(string userName, DateTime? dob)
        {
            var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
            return DataUnitOfWork.SanctionsList.GetSanctionsListsByDob(userName, dob);
        } 
    }
}
