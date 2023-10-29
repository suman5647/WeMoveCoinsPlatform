using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMC.Data.Interfaces;

namespace WMC.Data.Repository.Interfaces
{
    public interface ILanguageResourceRepository : IRepository<LanguageResource>
    {
        string GetLanguageResource(string Key, string language);
    }
}
