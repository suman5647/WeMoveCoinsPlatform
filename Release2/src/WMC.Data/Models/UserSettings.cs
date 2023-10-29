namespace WMC.Data.Models
{
    public class UserSettings
    {
        public string CurrencyCode { get; set; }
        public string CultureCode { get; set; }
        public decimal CardFee { get; set; }
        public int? PhoneCode { get; set; }
        public long? PhoneCodeId { get; set; }
        public string PhoneNumberStyle { get; set; }
    }
}