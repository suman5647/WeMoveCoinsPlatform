using System;
using System.Data.Entity;
using WMC.Data.Repository.Interfaces;
using System.Linq;
using WMC.Data;
using System.Collections.Generic;
using WMC.Data.Models;
using WMC.Data.Enums;

namespace WMC.Data.Repositories
{
    public class CountryRepository : DataRepository<Country>, ICountryRepository
    {
        public CountryRepository(DbContext context)
            : base(context)
        {
        }

        public string GetCurrencyCodeByCountryCode(string countryCode)
        {
            var country = Data.Include("Currency").Where(q => q.Code == countryCode).FirstOrDefault();
            if (country == null)
            {
                country = Data.Include("Currency").Where(q => q.Code == "DK").FirstOrDefault();
                //throw new Exception("Country '" + countryCode + "' does not exist in the system's database. Please contact the administrator.");
            }
            if (country.Currency == null)
                throw new Exception("No Currency associated with the country '" + countryCode + "' in the system's database. Please contact the administrator.");
            return country.Currency.Code;
        }

        public string GetCultureCodeByCountryCode(string countryCode)
        {
            var country = Data.Where(q => q.Code == countryCode).FirstOrDefault();
            if (country == null)
            {
                country = Data.Where(q => q.Code == "DK").FirstOrDefault();
                //throw new Exception("Country '" + countryCode + "' does not exist in the system's database. Please contact the administrator.");
            }
            return country.CultureCode;
        }

        public string GetCultureCodeByCurrency(string currency)
        {
            if (currency == "EUR")
                return "da-DK";
            var dc = new MonniData();
            var cryptoCurrencies = dc.Currencies.Where(curr => curr.CurrencyTypeId == (int)Enums.CurrencyTypes.Digital && curr.IsActive == true).Select(curr => curr.Code).ToList();
            if (cryptoCurrencies.Contains(currency))
                return "en-US";
            var country = Data.Include("Currency").Where(q => q.Currency.Code.ToLower().Trim() == currency.ToLower().Trim()).FirstOrDefault();
            if (country == null)
                throw new Exception("Currency '" + currency + "' does not exist in the system's database. Please contact the administrator.");
            return country.CultureCode;
        }

        public UserSettings GetUserSettings(string countryCode)
        {
            var country = Data.Include("Currency").Where(q => q.Code == countryCode).FirstOrDefault();
            if (country == null)
            {
                // TODO: DK Hardcoded? atleast must be from CONST
                country = Data.Include("Currency").Where(q => q.Code == "DK").FirstOrDefault();
                return new UserSettings { CurrencyCode = "DKK", CultureCode = country.CultureCode, PhoneCodeId = country.Id, PhoneCode = country.PhoneCode, PhoneNumberStyle = country.PhoneNumberStyle, CardFee = country.CardFee.Value };
                //throw new Exception("Country '" + countryCode + "' does not exist in the system's database. Please contact the administrator.");
            }
            return new UserSettings
            {
                CurrencyCode = country.Currency.Code,
                CultureCode = country.CultureCode,
                PhoneCodeId = country.Id,
                PhoneCode = country.PhoneCode,
                PhoneNumberStyle = country.PhoneNumberStyle,
                CardFee = country.CardFee.HasValue ? country.CardFee.Value : 0.0M
            };
        }

        public List<Tuple<long, string, string, string, int>> GetPhoneCodes()
        {
            var result = new List<Tuple<long, string, string, string, int>>();
            Data.Where(q => q.PhoneCode.HasValue)
                .OrderBy(q => q.Text).Select(q => new
                {
                    q.Id,
                    q.Text,
                    q.PhoneCode,
                    q.PhoneNumberStyle,
                    q.PaymentGateWaysAccepted
                }).ToList()
                .ForEach(q => result.Add(new Tuple<long, string, string, string, int>(q.Id, q.Text, "+" + q.PhoneCode.Value.ToString(), q.PhoneNumberStyle, Convert.ToInt16(q.PaymentGateWaysAccepted))));
            return result;
        }
    }
}