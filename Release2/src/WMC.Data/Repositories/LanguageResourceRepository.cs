using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using WMC.Data.Repository.Interfaces;

namespace WMC.Data.Repositories
{
    class LanguageResourceRepository:DataRepository<LanguageResource>, ILanguageResourceRepository
    {
        public LanguageResourceRepository(DbContext context)
            :base(context)
        {
        }

        public string GetLanguageResource(string key, string language)
        {
            var errorMessage = Data.Where(q => q.Key == key && q.Language == language).FirstOrDefault();
            return errorMessage.Value;
        }
    }
}
