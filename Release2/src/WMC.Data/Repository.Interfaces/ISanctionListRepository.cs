using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMC.Data.Repository.Interfaces
{
    public interface ISanctionListRepository
    {
        IQueryable<SanctionsList> GetSanctionsLists(string userName);
        SanctionsList GetSanctionsListsByDob(string userName, DateTime? dob);
    }
}
