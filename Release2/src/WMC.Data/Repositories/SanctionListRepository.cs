using System;
using System.Data.Entity;
using System.Linq;
using WMC.Data.Repository.Interfaces;

namespace WMC.Data.Repositories
{
    class SanctionListRepository :DataRepository<SanctionsList>, ISanctionListRepository
    {
        public SanctionListRepository(DbContext context)
            : base(context)
        {
        }

        public IQueryable<SanctionsList> GetSanctionsLists(string userName)
        {
            return (IQueryable<SanctionsList>)Data.Where(q =>(q.Name1 != null)? q.Name1 == userName.Substring(0,q.Name1.Length) :true || (q.Name2 != null) ? q.Name2 == userName.Substring(0, q.Name2.Length) : true || (q.Name3 != null) ? q.Name3 == userName.Substring(0, q.Name3.Length) : true || (q.Name4 != null) ?  q.Name4 == userName.Substring(0, q.Name4.Length) : true || (q.Name5 != null) ? q.Name5 == userName.Substring(0, q.Name5.Length) : true || (q.Name6 != null) ? q.Name6 == userName.Substring(0, q.Name6.Length) : true);
        }

        public SanctionsList GetSanctionsListsByDob(string userName, DateTime? dob)
        {
            return Data.Where(q =>(q.Name1 != null)? q.Name1 == userName.Substring(0, q.Name1.Length) : true || (q.Name2 != null) ? q.Name2 == userName.Substring(0, q.Name2.Length) : true || (q.Name3 != null) ? q.Name3 == userName.Substring(0, q.Name3.Length) : true || (q.Name4 != null) ? q.Name4 == userName.Substring(0, q.Name4.Length) : true || (q.Name5 != null) ? q.Name5 == userName.Substring(0, q.Name5.Length) : true || (q.Name6 != null) ? q.Name6 == userName.Substring(0, q.Name6.Length) : true && (q.DOB == dob)).FirstOrDefault();
        }
    }
}
