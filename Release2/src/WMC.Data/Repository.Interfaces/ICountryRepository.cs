using System.Collections.Generic;
using WMC.Data.Interfaces;
using WMC.Data.Models;
using WMC.Data;
using System;
using WMC.Data.Enums;

namespace WMC.Data.Repository.Interfaces
{
    public interface ICountryRepository : IRepository<Country>
    {
        string GetCurrencyCodeByCountryCode(string countryCode);
        string GetCultureCodeByCountryCode(string countryCode);
        string GetCultureCodeByCurrency(string currency);
        UserSettings GetUserSettings(string countryCode);
        List<Tuple<long, string, string, string, int>> GetPhoneCodes();
    }
}
