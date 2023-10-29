namespace WMC.Data
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Merchant")]
    public partial class Merchant
    {
        public long Id { get; set; }
        public string MerchantName { get; set; }
        public string MerchantCode { get; set; }
        public string MerchantWebhookUrl { get; set; }
    }
}
