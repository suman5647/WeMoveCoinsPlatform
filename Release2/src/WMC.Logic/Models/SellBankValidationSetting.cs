namespace WMC.Logic
{
    public class SellBankValidationSetting
    {
        public string LabelName { get; set; }
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        public string Type { get; set; }
        public string Regex { get; set; }
        public bool RequiresSpecialValidation { get; set; }
    }
}
