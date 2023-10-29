using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WMC.Data.Interfaces;

namespace WMC.Logic
{
    public class PurchaseLogic
    {
        private IDataUnitOfWork Uow;

        public PurchaseLogic(IDataUnitOfWork uow)
        {
            this.Uow = uow;
        }

        public PaymentDetails GetBankPaymentDetailsForBuy(string currencyCode, string defaultConfig = null)
        {
            var bankPaymentSettings = (defaultConfig == null) ? SettingsManager.GetDefault().Get("BuyBankPaymentSettings").GetJsonData<PaymentDetails[]>() : JsonConvert.DeserializeObject<PaymentDetails[]>(defaultConfig);
            PaymentDetails paymentDetails = bankPaymentSettings.FirstOrDefault(x => x.Currency.Equals(currencyCode, StringComparison.InvariantCultureIgnoreCase));
            return paymentDetails;
        }

        public Dictionary<string, string> GetBankPaymentDetailsForBuyAsLangDictionary(string currencyCode, string defaultConfig = null)
        {
            var paymentDetails = GetBankPaymentDetailsForBuy(currencyCode, defaultConfig);
            if (paymentDetails != null)
            {
                Dictionary<string, string> res = new Dictionary<string, string>()
                {
                    // Basic Bank Details //
                    //{  "Currency", paymentDetails.Currency },
                    // {  "Bank_BankName", paymentDetails.Bank },
                };

                if (!string.IsNullOrEmpty(paymentDetails.Beneficiary))
                    res.Add("Bank_Beneficiary", paymentDetails.Beneficiary);

                // IBAN //
                if (!string.IsNullOrEmpty(paymentDetails.IBAN))
                    res.Add("Bank_IBAN", paymentDetails.IBAN);
                if (!string.IsNullOrEmpty(paymentDetails.BICORSWIFT))
                    res.Add("Bank_BICORSWIFT", paymentDetails.BICORSWIFT);
                // IBAN //

                // Account Number //
                if (!string.IsNullOrEmpty(paymentDetails.UKSortCode))
                    res.Add("Bank_UKSortCode", paymentDetails.UKSortCode);
                if (!string.IsNullOrEmpty(paymentDetails.BSBCode))
                    res.Add("Bank_BSBCode", paymentDetails.BSBCode);
                if (!string.IsNullOrEmpty(paymentDetails.RegNumber))
                    res.Add("Bank_DKK_RegNumber", paymentDetails.RegNumber);

                if (!string.IsNullOrEmpty(paymentDetails.AccountNumber))
                    res.Add("Bank_AccountNumber", paymentDetails.AccountNumber);
                // Account Number //

                // Other Details //
                if (!string.IsNullOrEmpty(paymentDetails.BeneficiaryAddress))
                    res.Add("Bank_BeneficiaryAddress", paymentDetails.BeneficiaryAddress);
                if (!string.IsNullOrEmpty(paymentDetails.BankPaymentInstitution))
                    res.Add("Bank_BankPaymentInstitution", paymentDetails.BankPaymentInstitution);
                if (!string.IsNullOrEmpty(paymentDetails.MandatoryReference))
                    res.Add("Bank_MandatoryReference", paymentDetails.MandatoryReference);
                // Other Details //

                return res;
            }

            return null;
        }

        public SellPaymentDetails GetBankPaymentDetailsForSell(string currencyCode)
        {
            SellPaymentDetails result = null;
            var bankPaymentSettings = SettingsManager.GetDefault().Get("SellBankPaymentSettings").GetJsonData<SellPaymentConfig[]>();
            var bankPaymentDataConfig = bankPaymentSettings.FirstOrDefault(x => x.Currency.Equals(currencyCode, StringComparison.InvariantCultureIgnoreCase));
            if (bankPaymentDataConfig != null)
            {
                result = new SellPaymentDetails(currencyCode);

                if (bankPaymentDataConfig.BICORSWIFT) { result.Value1LabelResourceName = "Bank_BICORSWIFT"; }
                if (bankPaymentDataConfig.RegNumber) { result.Value1LabelResourceName = "Bank_DKK_RegNumber"; }
                if (bankPaymentDataConfig.UKSortCode) { result.Value1LabelResourceName = "Bank_UKSortCode"; }
                if (bankPaymentDataConfig.BSBCode) { result.Value1LabelResourceName = "Bank_BSBCode"; }

                if (bankPaymentDataConfig.IBAN) { result.Value2LabelResourceName = "Bank_IBAN"; }
                if (bankPaymentDataConfig.AccountNumber) { result.Value2LabelResourceName = "Bank_AccountNumber"; }
            }

            return result;
        }

        public static bool ValidateAccountDetailsForSell(string currencyCode, string SwiftCode, string IBAN, string Reg, string Account, out List<string> errors)
        {
            SellPaymentDetails result = null;
            errors = new List<string>();
            var bankPaymentSettings = SettingsManager.GetDefault().Get("SellBankPaymentSettings").GetJsonData<SellPaymentConfig[]>();
            var bankPaymentDataConfig = bankPaymentSettings.FirstOrDefault(x => x.Currency.Equals(currencyCode, StringComparison.InvariantCultureIgnoreCase));
            if (bankPaymentDataConfig != null)
            {
                result = new SellPaymentDetails(currencyCode);
                if (bankPaymentDataConfig.BICORSWIFT) {
                    result.Value1LabelResourceName = "Bank_BICORSWIFT";
                    if (String.IsNullOrWhiteSpace(SwiftCode))
                    {
                        errors.Add("Bank_BICORSWIFTNullMessage");
                    }
                }
                if (bankPaymentDataConfig.RegNumber) {
                    result.Value1LabelResourceName = "Bank_DKK_RegNumber";
                    if (String.IsNullOrWhiteSpace(Reg))
                    {
                        errors.Add("RegEmptyMessage");
                    }
                }
                if (bankPaymentDataConfig.UKSortCode) {
                    result.Value1LabelResourceName = "Bank_UKSortCode";
                    if (String.IsNullOrWhiteSpace(Account))
                    {
                        errors.Add("Bank_UKSortCodeNullMessage");
                    }
                }
                if (bankPaymentDataConfig.BSBCode) {
                    result.Value1LabelResourceName = "Bank_BSBCode";
                    if (String.IsNullOrWhiteSpace(Account))
                    {
                        errors.Add("Bank_BSBCodeNullMessage");
                    }
                }

                if (bankPaymentDataConfig.IBAN) {
                    result.Value2LabelResourceName = "Bank_IBAN";
                    if (String.IsNullOrWhiteSpace(IBAN))
                    {
                        errors.Add("IBANEmptyMessage");
                    }
                }
                if (bankPaymentDataConfig.AccountNumber) {
                    result.Value2LabelResourceName = "Bank_AccountNumber";
                    if (String.IsNullOrWhiteSpace(Account))
                    {
                        errors.Add("AccountEmptyMessage");
                    }
                }
            }

            return errors.Count > 0;
        }
    }

    public class PaymentDetails
    {
        public string Currency { get; set; }
        public string Bank { get; set; }
        public string Beneficiary { get; set; }
        public string IBAN { get; set; }
        public string BICORSWIFT { get; set; }
        public string BeneficiaryAddress { get; set; }
        public string BankPaymentInstitution { get; set; }
        public string MandatoryReference { get; set; }
        public string UKSortCode { get; set; }
        public string BSBCode { get; set; }
        public string RegNumber { get; set; }
        public string AccountNumber { get; set; }
    }

    public class SellPaymentDetails
    {
        public SellPaymentDetails(string currency)
        {
            this.Currency = currency;
            this.Value1 = "";
            this.Value2 = "";
        }

        public string Currency { get; set; }
        public string Value1LabelResourceName { get; set; }
        public string Value1 { get; set; }
        public string Value2LabelResourceName { get; set; }
        public string Value2 { get; set; }
    }

    public class SellPaymentConfig
    {
        public string Currency { get; set; }
        public bool RegNumber { get; set; }
        public bool AccountNumber { get; set; }
        public bool BICORSWIFT { get; set; }
        public bool IBAN { get; set; }
        public bool UKSortCode { get; set; }
        public bool BSBCode { get; set; }
        public List<SellBankValidationSetting> ValidationSetting { get; set; }
    }
}
